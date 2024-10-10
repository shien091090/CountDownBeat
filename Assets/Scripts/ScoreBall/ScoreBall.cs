using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBall
    {
        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;
        private readonly IScoreBallPresenter presenter;

        public int StartCountDownValue { get; private set; }
        public int CurrentCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        private bool IsCountDownInProcess => CurrentState == ScoreBallState.InCountDown;

        public ScoreBall(IScoreBallPresenter presenter, IEventRegister eventRegister, IEventInvoker eventInvoker)
        {
            this.presenter = presenter;
            this.eventRegister = eventRegister;
            this.eventInvoker = eventInvoker;
        }

        public void Init(int startCountDownValue)
        {
            if (startCountDownValue <= 0)
                return;

            StartCountDownValue = startCountDownValue;
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);

            RegisterEvent();
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

        private void CheckDamageAndHide()
        {
            if (CurrentCountDownValue > 0)
                return;

            Hide();
            eventInvoker.SendEvent(new DamageEvent());
        }

        private void UpdateCurrentState(ScoreBallState newState)
        {
            if (CurrentState == newState)
                return;

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

        private void RegisterEvent()
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);
            eventRegister.Register<BeatEvent>(OnBeat);
        }

        private void OnBeat(BeatEvent eventInfo)
        {
            if (IsCountDownInProcess == false)
                return;

            UpdateCurrentCountDownValue(CurrentCountDownValue - 1);
            CheckDamageAndHide();
        }
    }
}