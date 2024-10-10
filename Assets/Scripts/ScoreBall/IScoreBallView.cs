namespace GameCore
{
    public interface IScoreBallView
    {
        void Init();
        void SetCountDownNumberText(string text);
        void SetInCountDownColor();
        void SetFreezeColor();
        void Close();
    }
}