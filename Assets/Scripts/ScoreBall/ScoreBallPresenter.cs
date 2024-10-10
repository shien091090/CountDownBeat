using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;
        private readonly IGameSetting gameSetting;

        private ScoreBall scoreBall;
        private ScoreBallView view;

        public ScoreBallPresenter(IEventRegister eventRegister, IEventInvoker eventInvoker, IGameSetting gameSetting)
        {
            this.eventRegister = eventRegister;
            this.gameSetting = gameSetting;
            this.eventInvoker = eventInvoker;
        }

        public void UpdateCountDownNumber(int value)
        {
            view.SetCountDownNumberText(value.ToString());
        }

        public void BindView(ScoreBallView view)
        {
            this.view = view;
            scoreBall = new ScoreBall(this, eventRegister, eventInvoker);
            scoreBall.Init(gameSetting.ScoreBallStartCountDownValue);
        }
    }
}