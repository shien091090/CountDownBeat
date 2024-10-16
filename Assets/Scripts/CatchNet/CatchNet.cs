using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class CatchNet : ICatchNet
    {
        public int TargetNumber { get; private set; }
        
        private readonly IEventInvoker eventInvoker;
        private readonly IGameSetting gameSetting;
        private readonly ICatchNetPresenter presenter;

        public CatchNetState CurrentState { get; private set; }

        public CatchNet(ICatchNetPresenter presenter, IEventInvoker eventInvoker, IGameSetting gameSetting)
        {
            this.presenter = presenter;
            this.eventInvoker = eventInvoker;
            this.gameSetting = gameSetting;

            CurrentState = CatchNetState.None;
            presenter.BindModel(this);
        }

        public void Init(int targetNumber)
        {
            TargetNumber = targetNumber;

            UpdateState(CatchNetState.Working);
            presenter.RefreshCatchNumber();
        }

        public bool TryTriggerCatch(int number)
        {
            if (CurrentState != CatchNetState.Working)
                return false;

            if (number != TargetNumber)
                return false;

            UpdateState(CatchNetState.SuccessSettle);
            eventInvoker.SendEvent(new GetScoreEvent(gameSetting.SuccessSettleScore));
            return true;
        }

        private void UpdateState(CatchNetState newState)
        {
            CurrentState = newState;
            presenter.UpdateState(CurrentState);
        }
    }
}