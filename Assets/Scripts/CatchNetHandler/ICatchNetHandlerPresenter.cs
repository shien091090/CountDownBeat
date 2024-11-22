namespace GameCore
{
    public interface ICatchNetHandlerPresenter
    {
        void Init();
        void SpawnCatchNet(ICatchNetPresenter catchNetPresenter);
        void BindModel(ICatchNetHandler model);
        void BindView(ICatchNetHandlerView view);
        void OpenView();
        void FreeUpPosAndRefreshCurrentCount(int spawnIndex);
        void UnbindView();
        void UnbindModel();
        ICatchNetView Spawn();
    }
}