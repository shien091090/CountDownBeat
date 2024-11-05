namespace GameCore
{
    public interface ISelectionMenuHandlerPresenter
    {
        void OpenView();
        void BindView(ISelectionMenuHandlerView view);
        void BindModel(ISelectionMenuHandler model);
    }
}