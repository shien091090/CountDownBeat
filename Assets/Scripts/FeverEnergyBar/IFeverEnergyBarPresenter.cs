namespace GameCore
{
    public interface IFeverEnergyBarPresenter
    {
        void BindModel(IFeverEnergyBarModel model);
        void UnbindModel();
        void OpenView();
        void BindView(IFeverEnergyBarView view);
        void UnbindView();
        void Hit();
    }
}