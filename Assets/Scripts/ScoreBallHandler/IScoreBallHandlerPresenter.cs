namespace GameCore
{
    public interface IScoreBallHandlerPresenter
    {
        void Spawn(IScoreBallPresenter scoreBallPresenter);
        void BindView(IScoreBallHandlerView view);
        void BindModel(IScoreBallHandler model);
        void OpenView();
    }
}