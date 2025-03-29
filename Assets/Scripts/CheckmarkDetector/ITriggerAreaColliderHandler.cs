using SNShien.Common.AdapterTools;

namespace GameCore
{
    public partial class CheckmarkDetectorPresenter
    {
        public interface ITriggerAreaColliderHandler : ICollider2DHandler
        {
            bool IsTypeMatch(ICheckmarkDetectorTriggerArea triggerAreaComponent);
        }
    }
}