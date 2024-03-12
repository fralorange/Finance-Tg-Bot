using FreelanceBotBase.Bot.Commands.Interface;
using FreelanceBotBase.Domain.States;

namespace FreelanceBotBase.Domain.FSM
{
    public class BotStateMachine
    {
        public State CurrentState { get; private set; }
        public ICommand<object>? WaitingForCommand { get; private set; }

        public Dictionary<State, Dictionary<Trigger, State>> Transitions { get; }

        public BotStateMachine()
        {
            CurrentState = State.Idle;
            Transitions = new()
            {
                {
                    State.Idle, new()
                    {
                        { Trigger.AwaitUserInput, State.WaitingForUserInput }
                    }
                },
                {
                    State.WaitingForUserInput, new()
                    {
                        { Trigger.UserInputReceived, State.Idle }
                    }
                }
            };
        }

        public void ChangeState(Trigger trigger)
        {
            if (Transitions.TryGetValue(CurrentState, out var stateTransitions) && stateTransitions.TryGetValue(trigger, out var newState))
            {
                CurrentState = newState;
            }
        }

        public void SetWaitingForCommand<T>(ICommand<T> command) where T : class
        {
            WaitingForCommand = command as ICommand<object>;
        }
    }
}
