using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class StageSettingContent : IStageSettingContent
    {
        [SerializeField] private string stageTitle;
        [SerializeField] private EventReference fmodEventReference;
        [SerializeField] private int bpm;
        [SerializeField] private int countDownBeatFreq;
        [SerializeField] private string audioKey;
        [SerializeField] private float hpDecreasePerSecond;
        [SerializeField] private float hpIncreasePerCatch;
        [SerializeField] private CatchFlagScriptableObject catchFlagSetting;
        [SerializeField] private List<int> spawnBeatIndexList;
        [SerializeField] private int scoreBallStartCountDownValue;
        [SerializeField] private List<ScoreBallFlagWeightByFeverStageSetting> scoreBallFlagWeightSettings;
        [SerializeField] private List<CatchNetLimitByFeverStageSetting> catchNetLimitByFeverStageSettings;
        [SerializeField] private int successSettleScore;
        [SerializeField] private int[] feverEnergyPhaseSettings;
        [SerializeField] private int feverEnergyIncrease;
        [SerializeField] private int feverEnergyDecrease;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public int SuccessSettleScore => successSettleScore;
        public List<CatchNetLimitByFeverStageSetting> CatchNetLimitByFeverStageSettings => catchNetLimitByFeverStageSettings;
        public int FeverEnergyIncrease => feverEnergyIncrease;
        public int FeverEnergyDecrease => feverEnergyDecrease;
        public List<int> SpawnBeatIndexList => spawnBeatIndexList;
        public int CountDownBeatFreq => countDownBeatFreq;
        public string AudioKey => audioKey;
        public float HpDecreasePerSecond => hpDecreasePerSecond;
        public float HpIncreasePerCatch => hpIncreasePerCatch;
        public int[] FeverEnergyPhaseSettings => feverEnergyPhaseSettings;
        public CatchFlagScriptableObject CatchFlagSetting => catchFlagSetting;
        public List<ScoreBallFlagWeightByFeverStageSetting> ScoreBallFlagWeightSettings => scoreBallFlagWeightSettings;
        public string StageTitle => stageTitle;
        public EventReference FmodEventReference => fmodEventReference;
        public int Bpm => bpm;

        public List<ScoreBallFlagWeightDefine> GetScoreBallFlagWeightSetting(int feverStage)
        {
            ScoreBallFlagWeightByFeverStageSetting match = scoreBallFlagWeightSettings.FirstOrDefault(x => x.FeverStage == feverStage);
            if (match == null)
                return new List<ScoreBallFlagWeightDefine>();
            else
                return match.FlagWeightSettings;
        }

        public void SetHpDecreasePerSecond(float decreaseValue)
        {
            hpDecreasePerSecond = decreaseValue;
        }

        public void SetHpIncreasePerCatch(float increaseValue)
        {
            hpIncreasePerCatch = increaseValue;
        }

        public void SetFmodEventReference(EventReference fmodEventReference)
        {
            this.fmodEventReference = fmodEventReference;
        }

        public void SetAudioKey(string audioKey)
        {
            this.audioKey = audioKey;
            this.stageTitle = audioKey;
        }

        public void SetBpm(int bpm)
        {
            this.bpm = bpm;
        }

        public void SetSpawnBeatIndexList(List<int> indexList)
        {
            this.spawnBeatIndexList = indexList;
        }

        public void SetCountDownBeatFreq(int freq)
        {
            countDownBeatFreq = freq;
        }

        public void SetScoreBallStartCountDownValue(int startCountDownValue)
        {
            scoreBallStartCountDownValue = startCountDownValue;
        }

        public void SetSuccessSettleScore(int successSettleScore)
        {
            this.successSettleScore = successSettleScore;
        }

        public void SetScoreBallFlagWeightSetting(List<ScoreBallFlagWeightByFeverStageSetting> flagWeightSetting)
        {
            scoreBallFlagWeightSettings = flagWeightSetting;
        }

        public void SetCatchNetLimitSetting(List<CatchNetLimitByFeverStageSetting> catchNetLimitSetting)
        {
            catchNetLimitByFeverStageSettings = catchNetLimitSetting;
        }

        public void SetFeverEnergyBarPhaseSetting(int[] feverEnergyPhaseSettings)
        {
            this.feverEnergyPhaseSettings = feverEnergyPhaseSettings;
        }

        public void SetFeverEnergyIncrease(int feverEnergyIncrease)
        {
            this.feverEnergyIncrease = feverEnergyIncrease;
        }

        public void SetFeverEnergyDecrease(int feverEnergyDecrease)
        {
            this.feverEnergyDecrease = feverEnergyDecrease;
        }

        public void SetCatchFlagMergeSetting(CatchFlagScriptableObject catchFlagSetting)
        {
            this.catchFlagSetting = catchFlagSetting;
        }
    }
}