using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class StageSettingContent
    {
        [SerializeField] private string stageTitle;
        [SerializeField] private EventReference fmodEventReference;
        [SerializeField] private int bpm;
        [SerializeField] private int countDownBeatFreq;
        [SerializeField] private string audioKey;
        [SerializeField] private float hpDecreasePerSecond;
        [SerializeField] private float hpIncreasePerCatch;
        [SerializeField] private List<int> spawnBeatIndexList;

        public string StageTitle => stageTitle;
        public EventReference FmodEventReference => fmodEventReference;
        public int Bpm => bpm;
        public string AudioKey => audioKey;
        public List<int> SpawnBeatIndexList => spawnBeatIndexList;
        public int CountDownBeatFreq => countDownBeatFreq;
        public float HpDecreasePerSecond => hpDecreasePerSecond;
        public float HpIncreasePerCatch => hpIncreasePerCatch;

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

        public void SetHpDecreasePerSecond(float decreaseValue)
        {
            hpDecreasePerSecond = decreaseValue;
        }

        public void SetHpIncreasePerCatch(float increaseValue)
        {
            hpIncreasePerCatch = increaseValue;
        }
    }
}