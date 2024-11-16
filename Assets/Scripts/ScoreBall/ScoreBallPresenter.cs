namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        public int CurrentCountDownValue => model.CurrentCountDownValue;

        private IScoreBall model;
        private IScoreBallView view;

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
            view.BindPresenter(this);
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

        public void BindModel(IScoreBall model)
        {
            this.model = model;
        }

        public void PlayBeatEffect()
        {
            view.CreateBeatEffectPrefab();
        }

        public void DragOver()
        {
            model.SetFreezeState(false);
        }
    }
}