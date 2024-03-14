﻿using FreelanceBotBase.Bot.Commands.Factory;
using FreelanceBotBase.Bot.Commands.Interface;
using FreelanceBotBase.Bot.Services.ChatState;
using FreelanceBotBase.Domain.FSM;
using FreelanceBotBase.Domain.States;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Payments;

namespace FreelanceBotBase.Bot.Handlers.Update
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly CommandFactory _commandFactory;
        private readonly ChatStateService _chatStateService;

        public UpdateHandler(ITelegramBotClient botClient,
            ILogger<UpdateHandler> logger,
            CommandFactory commandFactory,
            ChatStateService chatStateService)
        {
            _botClient = botClient;
            _logger = logger;
            _commandFactory = commandFactory;
            _chatStateService = chatStateService;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message?.Chat?.Id ?? update.CallbackQuery?.Message?.Chat?.Id ?? 0;
            var stateMachine = _chatStateService.GetOrCreateStateMachine(chatId);

            var handler = update switch
            {
                { Message: { SuccessfulPayment: { } } msg } => BotOnSuccessfulPaymentReceived(msg, cancellationToken),
                { Message: { } message } => BotOnMessageReceived(message, stateMachine, cancellationToken),
                { EditedMessage: { } message } => BotOnMessageReceived(message, stateMachine, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, stateMachine, cancellationToken),
                { InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
                { ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),
                { PreCheckoutQuery: { } preCheckoutQuery } => BotOnPreCheckoutQueryReceived(preCheckoutQuery, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }
        #region Bot Processors
        private async Task BotOnMessageReceived(Message message, BotStateMachine stateMachine, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive message type: {MessageType}", message.Type);
            if (message.Text is not { } messageText)
                return;

            if (stateMachine.CurrentState == State.WaitingForUserInput)
            {
                await stateMachine.ExecuteWaitingCommand(message, cancellationToken);
                stateMachine.ChangeState(Trigger.UserInputReceived);
            }
            else
            {
                ITextCommand command = _commandFactory.CreateCommand(messageText.Split(' ')[0]);
                Message sentMessage = await command.ExecuteAsync(message, cancellationToken);
                _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
            }
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, BotStateMachine stateMachine, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline keyboard callback from {CallbackQueryId}", callbackQuery.Id);

            if (stateMachine.CurrentState == State.WaitingForUserInput)
            {
                await stateMachine.ExecuteWaitingCommand(callbackQuery, cancellationToken);
                stateMachine.ChangeState(Trigger.UserInputReceived);
            }
            else
            {
                ICallbackCommand command = _commandFactory.CreateCallbackCommand(callbackQuery.Data!, stateMachine);
                await command.ExecuteAsync(callbackQuery, cancellationToken);
            }
        }

        private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline query from {InlineQueryFromId}", inlineQuery.From.Id);

            InlineQueryResult[] results =
            {
                new InlineQueryResultArticle(
                    id: "1",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent("hello"))
            };

            await _botClient.AnswerInlineQueryAsync(
                inlineQueryId: inlineQuery.Id,
                results: results,
                cacheTime: 0,
                isPersonal: true,
                cancellationToken: cancellationToken);
        }

        private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline result: {ChosedInlineResultId}", chosenInlineResult.ResultId);

            await _botClient.SendTextMessageAsync(
                chatId: chosenInlineResult.From.Id,
                text: $"You chose result with Id: {chosenInlineResult.ResultId}",
                cancellationToken: cancellationToken);
        }

        private async Task BotOnPreCheckoutQueryReceived(PreCheckoutQuery preCheckoutQuery, CancellationToken cancellationToken)
        {
            await _botClient.AnswerPreCheckoutQueryAsync(
                preCheckoutQueryId: preCheckoutQuery.Id,
                cancellationToken: cancellationToken);
        }

        private async Task BotOnSuccessfulPaymentReceived(Message message, CancellationToken cancellationToken)
        {
            // надо будет реализовывать EF Core транзакции. в случае невыполнения транзакции оставить контакты поддержки в Telegram (админа бота)

            ITextCommand command = _commandFactory.CreateSuccessfulPaymentCommand();

            await command.ExecuteAsync(message, cancellationToken);
        }

        private Task UnknownUpdateHandlerAsync(Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }

        #endregion
        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

            // Cooldown in case of network troubles.
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
}
