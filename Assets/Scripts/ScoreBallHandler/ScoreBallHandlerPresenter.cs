using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class ScoreBallHandlerPresenter : IScoreBallHandlerPresenter
    {
        [Inject] private IViewManager viewManager;

        private IScoreBallHandler model;
        private IScoreBallHandlerView view;

        public IScoreBallView Spawn()
        {
            return view.Spawn();
        }

        public void BindView(IScoreBallHandlerView view)
        {
            this.view = view;
        }

        public void BindModel(IScoreBallHandler model)
        {
            this.model = model;
        }

        public void OpenView()
        {
            viewManager.OpenView<ScoreBallHandlerView>(this);
        }
    }
}