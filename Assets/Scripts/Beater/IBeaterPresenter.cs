namespace GameCore
{
    public interface IBeaterPresenter
    {
        float CurrentTimer { get; }
        void SetHalfBeatTimeOffset(float halfBeatTimeOffset);
        void BindView(IBeaterView view);
        void UnbindView();
        void BindModel(IBeaterModel model);
        void OpenView();
        void PlayBeatAnimation();
        void UnbindModel();
        void TriggerHalfBeat();
    }
}