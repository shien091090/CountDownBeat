using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class CatchNet : ICatchNet
    {
        public int TargetNumber { get; private set; }

        private readonly IEventInvoker eventInvoker;
        private readonly IGameSetting gameSetting;
        private readonly ICatchNetHandlerPresenter catchNetHandlerPresenter;
        private ICatchNetPresenter presenter;

        public CatchNetState CurrentState { get; private set; }

        public CatchNet(ICatchNetHandlerPresenter catchNetHandlerPresenter, IEventInvoker eventInvoker, IGameSetting gameSetting)
        {
            this.catchNetHandlerPresenter = catchNetHandlerPresenter;
            this.eventInvoker = eventInvoker;
            this.gameSetting = gameSetting;

            CurrentState = CatchNetState.None;
        }

        public bool TryTriggerCatch(int number)
        {
            if (CurrentState != CatchNetState.Working)
                return false;

            if (number != TargetNumber)
                return false;

            UpdateState(CatchNetState.SuccessSettle);
            eventInvoker.SendEvent(new GetScoreEvent(gameSetting.SuccessSettleScore));
            catchNetHandlerPresenter.FreeUpPosAndRefreshCurrentCount(presenter.SpawnPosIndex);
            return true;
        }

        public void Init(int targetNumber)
        {
            TargetNumber = targetNumber;

            UpdateState(CatchNetState.Working);
            presenter.RefreshCatchNumber();
        }

        public void BindPresenter(IMVPPresenter presenter)
        {
            this.presenter = (ICatchNetPresenter)presenter;
            presenter.BindModel(this);
        }

        private void UpdateState(CatchNetState newState)
        {
            CurrentState = newState;
            presenter.UpdateState(CurrentState);
        }
    }
}