using System;

namespace GameCore
{
    public interface ICatchNet : IMVPModel
    {
        event Action<CatchNetState> OnUpdateState;
        event Action OnCatchNetBeat;
        int TargetFlagNumber { get; }
        bool TryTriggerCatch(int flagNumber);
    }
}