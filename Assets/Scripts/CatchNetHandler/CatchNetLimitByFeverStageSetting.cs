using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchNetLimitByFeverStageSetting
    {
        [SerializeField] private int feverStage;
        [SerializeField] private int limit;

        public int FeverStage => feverStage;
        public int Limit => limit;

        public CatchNetLimitByFeverStageSetting(int feverStage, int limit)
        {
            this.feverStage = feverStage;
            this.limit = limit;
        }
    }
}