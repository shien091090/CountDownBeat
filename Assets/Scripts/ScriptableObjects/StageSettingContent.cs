using UnityEngine;

namespace GameCore
{
    public class StageSettingContent
    {
        [SerializeField] private string stageTitle;

        public string StageTitle => stageTitle;
    }
}