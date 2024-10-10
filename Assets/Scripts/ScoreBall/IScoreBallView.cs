namespace GameCore
{
    public interface IScoreBallView
    {
        void InitData();
        void SetCountDownNumberText(string text);
        void SetInCountDownColor();
        void SetFreezeColor();
        void Close();
    }
}