namespace GameCore
{
    public interface ISelectionMenuHandlerPresenter
    {
        void ClickEnterStage(int stageIndex);
        void OpenView();
        void BindView(ISelectionMenuHandlerView view);
        void BindModel(ISelectionMenuHandler model);
    }
}