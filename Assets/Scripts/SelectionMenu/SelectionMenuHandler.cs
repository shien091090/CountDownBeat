using Zenject;

namespace GameCore
{
    public class SelectionMenuHandler : ISelectionMenuHandler
    {
        [Inject] private ISelectionMenuHandlerPresenter presenter;

        public void ExecuteModelInit()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }
    }
}