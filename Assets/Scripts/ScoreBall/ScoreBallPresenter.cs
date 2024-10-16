using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        public int CurrentCountDownValue => model.CurrentCountDownValue;

        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;
        private readonly IGameSetting gameSetting;

        private ScoreBall model;
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
            model = new ScoreBall(this, eventRegister, eventInvoker);
            model.Init(gameSetting.ScoreBallStartCountDownValue);
        }

        public void StartDrag()
        {
            model.SetFreezeState(true);
        }

        public void DoubleClick()
        {
            model.ResetToBeginning();
        }

        public void TriggerCatch()
        {
            model.SuccessSettle();
        }

        public void DragOver()
        {
            model.SetFreezeState(false);
        }
    }
}