using SNShien.Common.AdapterTools;
using SNShien.Common.TesterTools;

namespace GameCore
{
    public partial class CheckmarkDetectorPresenter
    {
        public class TriggerAreaColliderHandler : ITriggerAreaColliderHandler
        {
            private readonly ICheckmarkDetectorPresenter presenter;
            private readonly ICheckmarkDetectorTriggerArea triggerAreaComponent;
            private readonly Debugger debugger = new Debugger("TriggerAreaColliderHandler");

            private CheckmarkFirstTriggerAreaType FirstTriggerAreaType => triggerAreaComponent?.FirstTriggerAreaType() ?? CheckmarkFirstTriggerAreaType.None;
            private CheckmarkSecondTriggerAreaType SecondTriggerAreaType => triggerAreaComponent?.SecondTriggerAreaType() ?? CheckmarkSecondTriggerAreaType.None;

            public TriggerAreaColliderHandler(ICheckmarkDetectorTriggerArea triggerAreaComponent, ICheckmarkDetectorPresenter presenter)
            {
                this.triggerAreaComponent = triggerAreaComponent;
                this.presenter = presenter;
            }

            public void ColliderTriggerEnter2D(ICollider2DAdapter col)
            {
                debugger.ShowLog($"ColliderTriggerEnter2D");
                IScoreBallView scoreBall = col.GetComponent<IScoreBallView>();
                if (scoreBall == null)
                    return;

                presenter.ColliderTriggerEnter(triggerAreaComponent, scoreBall);
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

            public bool IsTypeMatch(ICheckmarkDetectorTriggerArea triggerAreaComponent)
            {
                if (triggerAreaComponent.FirstTriggerAreaType() != CheckmarkFirstTriggerAreaType.None)
                    return FirstTriggerAreaType == triggerAreaComponent.FirstTriggerAreaType();

                if (triggerAreaComponent.SecondTriggerAreaType() != CheckmarkSecondTriggerAreaType.None)
                    return SecondTriggerAreaType == triggerAreaComponent.SecondTriggerAreaType();

                return false;
            }
        }
    }
}