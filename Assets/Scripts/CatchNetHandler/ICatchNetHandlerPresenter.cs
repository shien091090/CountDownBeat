namespace GameCore
{
    public interface ICatchNetHandlerPresenter
    {
        bool TryOccupyPos(out int posIndex, out CatchNetSpawnFadeInMode fadeInMode);
        void BindModel(ICatchNetHandler model);
        void BindView(ICatchNetHandlerView view);
        void UnbindView();
        ICatchNetView Spawn(int spawnPosIndex);
    }
}