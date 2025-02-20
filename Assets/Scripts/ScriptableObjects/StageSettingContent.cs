using System.Collections.Generic;
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
        [SerializeField] private List<int> spawnBeatIndexList;
        [SerializeField] private int scoreBallStartCountDownValue;
        [SerializeField] private Dictionary<int, Dictionary<int, int>> scoreBallFlagWeightSettingDict;
        [SerializeField] private int successSettleScore;
        [SerializeField] private Dictionary<int, int> catchNetLimitByFeverStageSetting;
        [SerializeField] private int[] feverEnergyBarSetting;
        [SerializeField] private int feverEnergyIncrease;
        [SerializeField] private int feverEnergyDecrease;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public int SuccessSettleScore => successSettleScore;
        public Dictionary<int, int> CatchNetLimitByFeverStageSetting => catchNetLimitByFeverStageSetting;
        public int[] FeverEnergyBarSetting => feverEnergyBarSetting;
        public int FeverEnergyIncrease => feverEnergyIncrease;
        public int FeverEnergyDecrease => feverEnergyDecrease;
        public List<int> SpawnBeatIndexList => spawnBeatIndexList;
        public int CountDownBeatFreq => countDownBeatFreq;
        public string AudioKey => audioKey;
        public float HpDecreasePerSecond => hpDecreasePerSecond;
        public float HpIncreasePerCatch => hpIncreasePerCatch;
        public string StageTitle => stageTitle;
        public EventReference FmodEventReference => fmodEventReference;
        public int Bpm => bpm;

        public Dictionary<int, int> GetScoreBallFlagWeightSetting(int feverStage)
        {
            if (scoreBallFlagWeightSettingDict.TryGetValue(feverStage, out Dictionary<int, int> result))
            {
                return result ?? new Dictionary<int, int>();
            }
            else
                return new Dictionary<int, int>();
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
    }
}