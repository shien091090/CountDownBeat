using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class CatchNet : ICatchNet
    {
        public int TargetNumber { get; private set; }

        private readonly IEventRegister eventRegister;

        private readonly ICatchNetHandler catchNetHandler;
        private ICatchNetPresenter presenter;

        public CatchNetState CurrentState { get; private set; }

        public CatchNet(ICatchNetHandler catchNetHandler, IEventRegister eventRegister)
        {
            this.catchNetHandler = catchNetHandler;
            this.eventRegister = eventRegister;

            CurrentState = CatchNetState.None;
        }

        public event Action<CatchNetState> OnUpdateState;
        public event Action OnCatchNetBeat;

        public bool TryTriggerCatch(int number)
        {
            if (CurrentState != CatchNetState.Working)
                return false;

            if (number != TargetNumber)
                return false;

            catchNetHandler.SettleCatchNet(this);
            UpdateState(CatchNetState.SuccessSettle);
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
            OnUpdateState?.Invoke(CurrentState);

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
            OnCatchNetBeat?.Invoke();
        }
    }
}