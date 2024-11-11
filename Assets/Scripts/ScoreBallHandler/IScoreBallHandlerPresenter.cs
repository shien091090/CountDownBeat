namespace GameCore
{
    public interface IScoreBallHandlerPresenter
    {
        IScoreBallView Spawn();
        void BindView(IScoreBallHandlerView view);
        void BindModel(IScoreBallHandler model);
        void OpenView();
        void UnbindView();
        void UnbindModel();
    }
}