using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class CatchNetHandlerPresenter : ICatchNetHandlerPresenter
    {
        [Inject] private IViewManager viewManager;

        public int CurrentCatchNetCount { get; }

        private ICatchNetHandler model;
        private ICatchNetHandlerView view;

        public void SpawnCatchNet(ICatchNetPresenter catchNetPresenter)
        {
            view.Spawn(catchNetPresenter);
        }

        public void BindModel(ICatchNetHandler model)
        {
            this.model = model;
        }

        public void BindView(ICatchNetHandlerView view)
        {
            this.view = view;
        }

        public void OpenView()
        {
            viewManager.OpenView<CatchNetHandlerView>(this);
        }
    }
}