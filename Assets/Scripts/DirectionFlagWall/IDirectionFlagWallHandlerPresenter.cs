namespace GameCore
{
    public interface IDirectionFlagWallHandlerPresenter
    {
        void BindModel(IDirectionFlagWallHandler model);
        void BindView(IDirectionFlagWallHandlerView view);
        void UnbindView();
    }
}