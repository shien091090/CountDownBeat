namespace GameCore
{
    public interface IScoreBall : IMVPModel
    {
        int CurrentCountDownValue { get; }
        void SetFreezeState(bool isFreeze);
        void ResetToBeginning();
        void SuccessSettle();
    }
}