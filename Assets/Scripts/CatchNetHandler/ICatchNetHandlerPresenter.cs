namespace GameCore
{
    public interface ICatchNetHandlerPresenter
    {
        void Init();
        bool TryOccupyPos(out int posIndex, out CatchNetSpawnFadeInMode fadeInMode);
        void BindModel(ICatchNetHandler model);
        void BindView(ICatchNetHandlerView view);
        void OpenView();
        void FreeUpPosAndRefreshCurrentCount(int spawnIndex);
        void UnbindView();
        void UnbindModel();
        ICatchNetView Spawn(int spawnPosIndex);
    }
}