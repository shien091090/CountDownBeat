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
        [SerializeField] private List<int> spawnBeatIndexList;
        [SerializeField] private float hpDecreasePerBeat;
        [SerializeField] private float hpIncreasePerCatch;

        public string StageTitle => stageTitle;
        public EventReference FmodEventReference => fmodEventReference;
        public int Bpm => bpm;
        public string AudioKey => audioKey;
        public List<int> SpawnBeatIndexList => spawnBeatIndexList;
        public int CountDownBeatFreq => countDownBeatFreq;
        public float HpDecreasePerBeat => hpDecreasePerBeat;
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

        public void SetHpDecreasePerBeat(float decreaseValue)
        {
            hpDecreasePerBeat = decreaseValue;
        }

        public void SetHpIncreasePerCatch(float increaseValue)
        {
            hpIncreasePerCatch = increaseValue;
        }
    }
}