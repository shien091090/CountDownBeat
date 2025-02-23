using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class ScoreBallFlagWeightDefine
    {
        [SerializeField] private int flagNumber;
        [SerializeField] private int weight;

        public int FlagNumber => flagNumber;
        public int Weight => weight;

        public ScoreBallFlagWeightDefine(int flagNumber, int weight)
        {
            this.flagNumber = flagNumber;
            this.weight = weight;
        }
    }
}