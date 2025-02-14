using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBall : IScoreBall
    {
        public int CurrentFlagNumber { get; }

        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;
        private readonly IGameSetting gameSetting;

        private IScoreBallPresenter presenter;
        public int CurrentCountDownValue { get; private set; }

        public int StartCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        private bool IsCountDownInProcess => CurrentState == ScoreBallState.InCountDown;

        public ScoreBall(IEventRegister eventRegister, IEventInvoker eventInvoker, IGameSetting gameSetting)
        {
            this.eventRegister = eventRegister;
            this.eventInvoker = eventInvoker;
            this.gameSetting = gameSetting;
        }

        public event Action OnInit;
        public event Action<ScoreBallState> OnUpdateState;
        public event Action<int> OnUpdateCountDownValue;
        public event Action OnScoreBallBeat;
        public event Action OnScoreBallHalfBeat;

        public void SetFreezeState(bool isFreeze)
        {
            UpdateCurrentState(isFreeze ?
                ScoreBallState.Freeze :
                ScoreBallState.InCountDown);
        }

        public void SuccessSettle()
        {
            Hide();
        }

        public void ResetToBeginning()
        {
            if (CurrentState == ScoreBallState.Hide)
                return;

            UpdateCurrentCountDownValue(StartCountDownValue);
        }

        public void BindPresenter(IMVPPresenter mvpPresenter)
        {
            presenter = (IScoreBallPresenter)mvpPresenter;
        }

        public void Init()
        {
            StartCountDownValue = gameSetting.ScoreBallStartCountDownValue;
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);

            OnInit?.Invoke();
        }

        public void Reactivate()
        {
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);

            OnInit?.Invoke();
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);
            eventRegister.Unregister<HalfBeatEvent>(OnHalfBeat);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeat);
                eventRegister.Register<HalfBeatEvent>(OnHalfBeat);
            }
        }

        private void CheckDamageAndHide()
        {
            if (CurrentCountDownValue > 0)
                return;

            Hide();
            eventInvoker.SendEvent(new DamageEvent());
        }

        private void CheckChangeEventRegisterState(ScoreBallState beforeState, ScoreBallState afterState)
        {
            if ((beforeState == ScoreBallState.None || beforeState == ScoreBallState.Hide) &&
                afterState == ScoreBallState.InCountDown)
                SetEventRegister(true);

            if ((beforeState == ScoreBallState.InCountDown || beforeState == ScoreBallState.Freeze) &&
                afterState == ScoreBallState.Hide)
                SetEventRegister(false);
        }

        private void UpdateCurrentState(ScoreBallState newState)
        {
            if (CurrentState == newState)
                return;

            CheckChangeEventRegisterState(CurrentState, newState);

            CurrentState = newState;

            OnUpdateState?.Invoke(CurrentState);
        }

        private void UpdateCurrentCountDownValue(int newValue)
        {
            CurrentCountDownValue = newValue;
            OnUpdateCountDownValue?.Invoke(CurrentCountDownValue);
        }

        private void Hide()
        {
            CurrentCountDownValue = 0;
            UpdateCurrentState(ScoreBallState.Hide);
        }

        private void OnBeat(BeatEvent eventInfo)
        {
            OnScoreBallBeat?.Invoke();

            if (IsCountDownInProcess == false ||
                eventInfo.isCountDownBeat == false)
                return;

            UpdateCurrentCountDownValue(CurrentCountDownValue - 1);
            CheckDamageAndHide();
        }

        private void OnHalfBeat(HalfBeatEvent eventInfo)
        {
            OnScoreBallHalfBeat?.Invoke();
        }
    }
}