using System;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNet : IMVPModel
    {
        event Action<CatchNetState> OnUpdateState;
        event Action OnCatchNetBeat;
        int TargetNumber { get; }
        bool TryTriggerCatch(Vector2Int passNumberRange);
    }
}