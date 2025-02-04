using System;
using UnityEngine;

namespace GameCore
{
    public interface IScoreBall : IMVPModel
    {
        event Action OnInit;
        event Action<ScoreBallState> OnUpdateState;
        event Action<int> OnUpdateCountDownValue;
        event Action OnScoreBallBeat;
        int CurrentCountDownValue { get; }
        Vector2Int PassCountDownValueRange { get; }
        void SetFreezeState(bool isFreeze);
        void ResetToBeginning();
        void SuccessSettle();
        void TriggerExpand();
    }
}