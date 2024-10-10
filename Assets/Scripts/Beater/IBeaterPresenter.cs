namespace GameCore
{
    public interface IBeaterPresenter
    {
        void UpdatePerFrame();
        void BindView(IBeaterView view);
    }
}