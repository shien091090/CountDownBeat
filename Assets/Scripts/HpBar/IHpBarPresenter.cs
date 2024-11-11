namespace GameCore
{
    public interface IHpBarPresenter
    {
        void RefreshHp(float currentHp);
        void BindModel(IHpBarModel model);
        void OpenView();
        void BindView(IHpBarView view);
        void UnbindView();
        void UnbindModel();
    }
}