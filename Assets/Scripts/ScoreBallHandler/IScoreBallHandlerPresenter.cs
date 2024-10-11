namespace GameCore
{
    public interface IScoreBallHandlerPresenter
    {
        void Spawn(IScoreBallPresenter scoreBallPresenter);
        void BindView(IScoreBallHandlerView view);
    }
}