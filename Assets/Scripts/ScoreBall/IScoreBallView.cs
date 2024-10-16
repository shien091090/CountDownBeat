namespace GameCore
{
    public interface IScoreBallView
    {
        int GetCurrentCountDownValue { get; }
        void Init();
        void SetCountDownNumberText(string text);
        void SetInCountDownColor();
        void SetFreezeColor();
        void Close();
        void TriggerCatch();
    }
}