using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class CatchNet : ICatchNet
    {
        public int CatchFlagNumber { get; private set; }

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

        public event Action<int> OnUpdateCatchFlagNumber;
        public event Action<CatchNetState> OnUpdateState;
        public event Action OnCatchNetBeat;

        public bool TryTriggerCatch(int flagNumber)
        {
            if (CurrentState != CatchNetState.Working)
                return false;

            if (flagNumber != CatchFlagNumber)
                return false;

            UpdateState(CatchNetState.SuccessSettle);
            catchNetHandler.SettleCatchNet(this);
            return true;
        }

        public void BindPresenter(IMVPPresenter presenter)
        {
            this.presenter = (ICatchNetPresenter)presenter;
            presenter.BindModel(this);
        }

        public void Init(int catchFlagNumber)
        {
            UpdateCatchFlagNumber(catchFlagNumber);
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

        private void UpdateCatchFlagNumber(int targetFlagNumber)
        {
            CatchFlagNumber = targetFlagNumber;
            OnUpdateCatchFlagNumber?.Invoke(CatchFlagNumber);
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