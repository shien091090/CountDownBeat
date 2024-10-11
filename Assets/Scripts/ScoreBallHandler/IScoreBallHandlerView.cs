using SNShien.Common.MonoBehaviorTools;

namespace GameCore
{
    public interface IScoreBallHandlerView : IArchitectureView
    {
        void Spawn(IScoreBallPresenter scoreBallPresenter);
    }
}