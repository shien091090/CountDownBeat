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
        private IEventRegister eventRegister;

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

        public void BindPresenter(IMVPPresenter presenter)
        {
            this.presenter = (ICatchNetPresenter)presenter;
            presenter.BindModel(this);
        }

        public void Init(int targetNumber)
        {
            TargetNumber = targetNumber;

            UpdateState(CatchNetState.Working);
            presenter.RefreshCatchNumber();
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeat);
            }
        }

        private void UpdateState(CatchNetState newState)
        {
            CurrentState = newState;
            presenter.UpdateState(CurrentState);

            switch (newState)
            {
                case CatchNetState.SuccessSettle:
                case CatchNetState.None:
                    SetEventRegister(false);
                    break;

                case CatchNetState.Working:
                    SetEventRegister(true);
                    break;
            }
        }

        private void OnBeat(BeatEvent eventInfo)
        {
            presenter.PlayBeatEffect();
        }
    }
}