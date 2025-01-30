using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBallOperateEvent : IArchitectureEvent
    {
        public bool IsStartDrag { get; }
        public ComputableCollider Target { get; }

        public ScoreBallOperateEvent(bool isStartDrag, ComputableCollider target)
        {
            IsStartDrag = isStartDrag;
            Target = target;
        }
    }
}