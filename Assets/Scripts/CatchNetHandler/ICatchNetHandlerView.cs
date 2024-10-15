using SNShien.Common.MonoBehaviorTools;

namespace GameCore
{
    public interface ICatchNetHandlerView : IArchitectureView
    {
        void Spawn(ICatchNetPresenter presenter);
    }
}