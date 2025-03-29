using SNShien.Common.AdapterTools;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface ICheckmarkDetectorPresenter : IArchitectureModel, ICollider2DHandler
    {
        ICollider2DHandler GetColliderHandler(ICheckmarkDetectorTriggerArea triggerAreaComponent);
        void BindView(ICheckmarkDetectorView view);
        void UnbindView();
        void ColliderTriggerEnter(ICheckmarkDetectorTriggerArea triggerAreaComponent, IScoreBallView scoreBall);
    }
}