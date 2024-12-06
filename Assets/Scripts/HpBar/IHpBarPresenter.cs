namespace GameCore
{
    public interface IHpBarPresenter
    {
        void UpdateFrame();
        void BindModel(IHpBarModel model);
        void BindView(IHpBarView view);
        void UnbindView();
    }
}