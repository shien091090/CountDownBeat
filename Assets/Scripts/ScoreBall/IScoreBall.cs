namespace GameCore
{
    public interface IScoreBall
    {
        int CurrentCountDownValue { get; }
        void SetFreezeState(bool isFreeze);
        void ResetToBeginning();
        void SuccessSettle();
    }
}