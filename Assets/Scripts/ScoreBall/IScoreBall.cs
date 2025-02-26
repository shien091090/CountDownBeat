using System;

namespace GameCore
{
    public interface IScoreBall : IMVPModel
    {
        event Action OnInit;
        event Action<ScoreBallState> OnUpdateState;
        event Action<int> OnUpdateCountDownValue;
        event Action OnScoreBallBeat;
        event Action OnScoreBallHalfBeat;
        event Action<int> OnUpdateCatchFlagNumber;
        int CurrentFlagNumber { get; }
        void SetFreezeState(bool isFreeze);
        void ResetToBeginning();
        void SuccessSettle();
        void ChangeFlagTo(int newFlagNumber);
    }
}