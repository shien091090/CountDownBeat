namespace GameCore
{
    public interface IBeaterPresenter
    {
        void BindView(IBeaterView view);
        void UnbindView();
    }
}