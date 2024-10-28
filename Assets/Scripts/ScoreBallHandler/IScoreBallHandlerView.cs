using SNShien.Common.MonoBehaviorTools;

namespace GameCore
{
    public interface IScoreBallHandlerView : IArchitectureView
    {
        IScoreBallView Spawn();
    }
}