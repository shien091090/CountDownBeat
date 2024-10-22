using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class StageSettingContent
    {
        [SerializeField] private string stageTitle;
        [SerializeField] private EditorEventRef fmodEventReference;
        [SerializeField] private int bpm;
        [SerializeField] private string audioKey;
        [SerializeField] private List<int> spawnBeatIndexList;

        public string StageTitle => stageTitle;
        public EditorEventRef FmodEventReference => fmodEventReference;
        public int Bpm => bpm;
        public string AudioKey => audioKey;
        public List<int> SpawnBeatIndexList => spawnBeatIndexList;

        public void SetFmodEventReference(EditorEventRef fmodEventReference)
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
    }
}