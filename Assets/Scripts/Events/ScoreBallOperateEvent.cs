using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class ScoreBallOperateEvent : IArchitectureEvent
    {
        public IScoreBallView Target { get; }
        public bool IsStartDrag { get; }

        public ScoreBallOperateEvent(bool isStartDrag, IScoreBallView target)
        {
            IsStartDrag = isStartDrag;
            Target = target;
        }
    }
}