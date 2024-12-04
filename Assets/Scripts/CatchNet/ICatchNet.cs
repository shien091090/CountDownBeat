using System;

namespace GameCore
{
    public interface ICatchNet : IMVPModel
    {
        event Action<CatchNetState> OnUpdateState;
        event Action OnCatchNetBeat;
        int TargetNumber { get; }
        bool TryTriggerCatch(int number);
    }
}