using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBall
    {
        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;

        public int StartCountDownValue { get; private set; }
        public int CurrentCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        private bool IsCountDownInProcess => CurrentState == ScoreBallState.InCountDown;

        public ScoreBall(IEventRegister eventRegister, IEventInvoker eventInvoker)
        {
            this.eventRegister = eventRegister;
            this.eventInvoker = eventInvoker;
        }

        public void Init(int startCountDownValue)
        {
            if (startCountDownValue <= 0)
                return;

            StartCountDownValue = startCountDownValue;
            CurrentCountDownValue = StartCountDownValue;
            CurrentState = ScoreBallState.InCountDown;

            RegisterEvent();
        }

        public void DragAndFreeze()
        {
            CurrentState = ScoreBallState.Freeze;
        }

        private void CheckDamageAndHide()
        {
            if (CurrentCountDownValue > 0)
                return;

            CurrentCountDownValue = 0;
            CurrentState = ScoreBallState.Hide;
            eventInvoker.SendEvent(new DamageEvent());
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

            CurrentCountDownValue--;
            CheckDamageAndHide();
        }
    }
}