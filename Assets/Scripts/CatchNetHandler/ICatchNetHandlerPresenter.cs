namespace GameCore
{
    public interface ICatchNetHandlerPresenter
    {
        int CurrentCatchNetCount { get; }
        void Init();
        void SpawnCatchNet(ICatchNetPresenter catchNetPresenter);
        void BindModel(ICatchNetHandler model);
        void BindView(ICatchNetHandlerView view);
        void OpenView();
    }
}