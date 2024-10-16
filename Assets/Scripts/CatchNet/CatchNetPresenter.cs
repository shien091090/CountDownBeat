using SNShien.Common.AdapterTools;

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

        public void ColliderTriggerEnter2D(ICollider2DAdapter col)
        {
            IScoreBallView scoreBall = col.GetComponent<IScoreBallView>();
            if (scoreBall == null)
                return;

            int currentCountDownValue = scoreBall.GetCurrentCountDownValue;
            if (model.TryTriggerCatch(currentCountDownValue))
                scoreBall.TriggerCatch();
        }

        public void ColliderTriggerExit2D(ICollider2DAdapter col)
        {
        }

        public void ColliderTriggerStay2D(ICollider2DAdapter col)
        {
        }

        public void CollisionEnter2D(ICollision2DAdapter col)
        {
        }

        public void CollisionExit2D(ICollision2DAdapter col)
        {
        }
    }
}