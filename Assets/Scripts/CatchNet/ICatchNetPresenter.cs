namespace GameCore
{
    public interface ICatchNetPresenter
    {
        void UpdateState(CatchNetState currentState);
        void RefreshCatchNumber();
        void BindView(ICatchNetView view);
        void BindModel(ICatchNet model);
    }
}