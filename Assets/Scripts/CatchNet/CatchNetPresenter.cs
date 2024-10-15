namespace GameCore
{
    public class CatchNetPresenter : ICatchNetPresenter
    {
        private ICatchNet model;
        private ICatchNetView view;

        public CatchNetPresenter()
        {
        }

        public void UpdateState(CatchNetState currentState)
        {
        }

        public void RefreshCatchNumber()
        {
            view.SetCatchNumber(model.TargetNumber.ToString("N0"));
        }

        public void BindView(ICatchNetView view)
        {
            this.view = view;
        }

        public void BindModel(ICatchNet model)
        {
            this.model = model;
        }
    }
}