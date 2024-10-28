namespace GameCore
{
    public interface IScoreBallView
    {
        int GetCurrentCountDownValue { get; }
        void Init();
        void SetCountDownNumberText(string text);
        void SetInCountDownColor();
        void SetFreezeColor();
        bool CheckCreatePresenter(out IScoreBallPresenter scoreBallPresenter);
        void Close();
        void TriggerCatch();
    }
}