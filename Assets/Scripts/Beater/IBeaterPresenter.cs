namespace GameCore
{
    public interface IBeaterPresenter
    {
        void BindView(IBeaterView view);
        void UnbindView();
        void BindModel(IBeaterModel model);
        void OpenView();
        void PlayBeatAnimation();
        void UnbindModel();
    }
}