using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class FeverEnergyBarPresenter : IFeverEnergyBarPresenter
    {
        [Inject] private IViewManager viewManager;

        private IFeverEnergyBarModel model;
        private IFeverEnergyBarView view;

        public void BindModel(IFeverEnergyBarModel model)
        {
            this.model = model;
        }

        public void UnbindModel()
        {
            model = null;
        }

        public void OpenView()
        {
            viewManager.OpenView<FeverEnergyBarView>(this);
        }

        public void BindView(IFeverEnergyBarView view)
        {
            this.view = view;
        }

        public void UnbindView()
        {
            view = null;
        }
    }
}