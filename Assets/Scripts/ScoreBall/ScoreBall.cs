using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBall
    {
        private readonly IEventRegister eventRegister;

        public int StartCountDownValue { get; private set; }
        public int CurrentCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        public ScoreBall(IEventRegister eventRegister)
        {
            this.eventRegister = eventRegister;
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

        private void RegisterEvent()
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);
            eventRegister.Register<BeatEvent>(OnBeat);
        }

        private void OnBeat(BeatEvent eventInfo)
        {
            CurrentCountDownValue--;
        }
    }
}