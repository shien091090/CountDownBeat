using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class ScoreBoardPresenter : IScoreBoardPresenter
    {
        [Inject] private IViewManager viewManager;

        private IScoreBoardModel model;
        private ScoreBoardView view;

        public void RefreshScoreDisplay(long currentScore)
        {
            view?.SetCurrentScoreText(currentScore.ToString("N0"));
        }

        public void BindModel(IScoreBoardModel model)
        {
            this.model = model;
        }

        public void BindView(ScoreBoardView view)
        {
            this.view = view;
        }

        public void OpenView()
        {
            viewManager.OpenView<ScoreBoardView>(this);
        }

        public void UnbindView()
        {
            view = null;
        }

        public void UnbindModel()
        {
            model = null;
        }
    }
}