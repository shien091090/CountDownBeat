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
            {
                EnergyValue += gameSetting.FeverEnergyIncrease;
            }
        }
    }
}