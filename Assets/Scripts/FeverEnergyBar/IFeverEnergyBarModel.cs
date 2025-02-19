using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IFeverEnergyBarModel : IArchitectureModel
    {
        event Action<UpdateFeverEnergyBarEvent> OnUpdateFeverEnergyValue;
        event Action<int> OnUpdateFeverStage;
        event Action<BeatAccuracyResult> OnHit;
        int CurrentFeverStage { get; }
        void Hit();
    }
}