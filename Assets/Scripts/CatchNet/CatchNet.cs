using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class CatchNet
    {
        private readonly IEventInvoker eventInvoker;
        private readonly IGameSetting gameSetting;

        public int TargetNumber { get; private set; }
        public CatchNetState CurrentState { get; private set; }

        public CatchNet(IEventInvoker eventInvoker, IGameSetting gameSetting)
        {
            this.eventInvoker = eventInvoker;
            this.gameSetting = gameSetting;
            CurrentState = CatchNetState.None;
        }

        public void Init(int targetNumber)
        {
            TargetNumber = targetNumber;
            UpdateState(CatchNetState.Working);
        }

        public void TriggerCatch(int number)
        {
            if (CurrentState != CatchNetState.Working)
                return;
            
            if (number != TargetNumber)
                return;
            
            UpdateState(CatchNetState.SuccessSettle);
            eventInvoker.SendEvent(new GetScoreEvent(gameSetting.SuccessSettleScore));
        }

        private void UpdateState(CatchNetState newState)
        {
            CurrentState = newState;
        }
    }
}