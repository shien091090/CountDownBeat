namespace GameCore
{
    public class ScoreBallHandlerPresenter : IScoreBallHandlerPresenter
    {
        private readonly IScoreBallHandler model;
        private IScoreBallHandlerView view;

        public ScoreBallHandlerPresenter(IScoreBallHandler model)
        {
            this.model = model;
        }

        public void Spawn(IScoreBallPresenter scoreBallPresenter)
        {
            view.Spawn(scoreBallPresenter);
        }

        public void BindView(IScoreBallHandlerView view)
        {
            this.view = view;
        }
    }
}