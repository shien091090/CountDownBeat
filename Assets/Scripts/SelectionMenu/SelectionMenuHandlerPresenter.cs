using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class SelectionMenuHandlerPresenter : ISelectionMenuHandlerPresenter
    {
        [Inject] private IViewManager viewManager;

        private ISelectionMenuHandlerView view;
        private ISelectionMenuHandler model;

        public void ClickEnterStage(int stageIndex)
        {
            model.EnterStage(stageIndex);
        }

        public void OpenView()
        {
            viewManager.OpenView<SelectionMenuHandlerView>(this);
        }

        public void BindView(ISelectionMenuHandlerView view)
        {
            this.view = view;
        }

        public void BindModel(ISelectionMenuHandler model)
        {
            this.model = model;
        }

        public void UnbindView()
        {
            view = null;
        }
    }
}