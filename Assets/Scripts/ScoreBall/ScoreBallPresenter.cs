using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;
        private readonly IGameSetting gameSetting;

        private ScoreBall scoreBall;
        private IScoreBallView view;

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

        public void UpdateState(ScoreBallState state)
        {
            switch (state)
            {
                case ScoreBallState.InCountDown:
                    view.SetInCountDownColor();
                    break;

                case ScoreBallState.Hide:
                    view.Close();
                    break;

                case ScoreBallState.Freeze:
                    view.SetFreezeColor();
                    break;
            }
        }

        public void BindView(IScoreBallView view)
        {
            this.view = view;

            view.Init();
            scoreBall = new ScoreBall(this, eventRegister, eventInvoker);
            scoreBall.Init(gameSetting.ScoreBallStartCountDownValue);
        }

        public void StartDrag()
        {
            scoreBall.SetFreezeState(true);
        }

        public void DragOver()
        {
            scoreBall.SetFreezeState(false);
        }
    }
}