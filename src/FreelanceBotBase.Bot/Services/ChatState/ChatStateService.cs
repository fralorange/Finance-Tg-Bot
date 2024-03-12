using FreelanceBotBase.Domain.FSM;
using System.Collections.Concurrent;

namespace FreelanceBotBase.Bot.Services.ChatState
{
    public class ChatStateService
    {
        private readonly ConcurrentDictionary<long, BotStateMachine> _chatStateMachines;

        public ChatStateService() => _chatStateMachines = new();

        public BotStateMachine GetOrCreateStateMachine(long chatId) => _chatStateMachines.GetOrAdd(chatId, _ => new BotStateMachine());
    }
}
