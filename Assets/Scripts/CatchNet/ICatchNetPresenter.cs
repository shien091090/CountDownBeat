using SNShien.Common.AdapterTools;

namespace GameCore
{
    public interface ICatchNetPresenter : ICollider2DHandler
    {
        int SpawnPosIndex { get; }
        void Init(int spawnPosIndex, CatchNetSpawnFadeInMode fadeInMode);
        void UpdateState(CatchNetState currentState);
        void RefreshCatchNumber();
        void BindView(ICatchNetView view);
        void BindModel(ICatchNet model);
    }
}