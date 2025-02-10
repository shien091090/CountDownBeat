using System.Linq;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class FeverEnergyBarModel
    {
        [Inject] private IBeaterModel beaterModel;
        [Inject] private IGameSetting gameSetting;

        public float EnergyValue { get; private set; }

        public void HitBeat()
        {
            BeatAccuracyResult beatAccuracyResult = beaterModel.DetectBeatAccuracyCurrentTime();
            if (gameSetting.AccuracyPassThreshold >= 1 - beatAccuracyResult.AccuracyValue)
                EnergyValue += gameSetting.FeverEnergyIncrease;
            else
                EnergyValue -= gameSetting.FeverEnergyDecrease;

            EnergyValue = Mathf.Clamp(EnergyValue, 0, GetEnergyBarMaxValue());
        }

        private int GetEnergyBarMaxValue()
        {
            int[] energyBarSetting = gameSetting.FeverEnergyBarSetting;
            return energyBarSetting == null || energyBarSetting.Length == 0 ?
                0 :
                energyBarSetting.Sum();
        }
    }
}