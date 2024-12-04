using System;

namespace GameCore
{
    public interface IScoreBall : IMVPModel
    {
        event Action OnInit;
        event Action<ScoreBallState> OnUpdateState;
        event Action<int> OnUpdateCountDownValue;
        event Action OnScoreBallBeat;
        int CurrentCountDownValue { get; }
        void SetFreezeState(bool isFreeze);
        void ResetToBeginning();
        void SuccessSettle();
    }
}