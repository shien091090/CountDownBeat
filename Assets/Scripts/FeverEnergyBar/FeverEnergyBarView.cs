using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class FeverEnergyBarView : ArchitectureView, IFeverEnergyBarView
    {
        [Inject] private IInputGetter inputGetter;

        private IFeverEnergyBarPresenter presenter;
        private Debugger debugger = new Debugger("FeverEnergyBarView");

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IFeverEnergyBarPresenter;
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            presenter.UnbindView();
        }

        private void Update()
        {
            if (inputGetter.IsClickDown())
                presenter.Hit();
        }
    }
}