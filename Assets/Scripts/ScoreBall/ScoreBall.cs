using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBall : IScoreBall
    {
        public int CurrentCountDownValue { get; private set; }
        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;
        private readonly IScoreBallTextColorSetting scoreBallTextColorSetting;

        private IScoreBallPresenter presenter;

        public int StartCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        private bool IsCountDownInProcess => CurrentState == ScoreBallState.InCountDown;

        public ScoreBall(IEventRegister eventRegister, IEventInvoker eventInvoker, IScoreBallTextColorSetting scoreBallTextColorSetting)
        {
            this.eventRegister = eventRegister;
            this.eventInvoker = eventInvoker;
            this.scoreBallTextColorSetting = scoreBallTextColorSetting;
        }

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

        public void Init(int startCountDownValue)
        {
            if (startCountDownValue <= 0)
                return;

            StartCountDownValue = startCountDownValue;
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);
            presenter.PlayBeatEffect();
        }

        public void BindPresenter(IMVPPresenter mvpPresenter)
        {
            presenter = (IScoreBallPresenter)mvpPresenter;
            presenter.BindModel(this);
            presenter.Init(scoreBallTextColorSetting);
        }

        public void Reactivate()
        {
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);
            presenter.PlayBeatEffect();
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeat);
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
            presenter.UpdateState(newState);
        }

        private void UpdateCurrentCountDownValue(int newValue)
        {
            CurrentCountDownValue = newValue;
            presenter.UpdateCountDownNumber(CurrentCountDownValue);
        }

        private void Hide()
        {
            CurrentCountDownValue = 0;
            UpdateCurrentState(ScoreBallState.Hide);
        }

        private void OnBeat(BeatEvent eventInfo)
        {
            presenter.PlayBeatEffect();

            if (IsCountDownInProcess == false ||
                eventInfo.isCountDownBeat == false)
                return;

            UpdateCurrentCountDownValue(CurrentCountDownValue - 1);
            CheckDamageAndHide();
        }
    }
}