using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class StageSettingScriptableObject : SerializedScriptableObject, IStageSetting
    {
        [SerializeField] public List<StageSettingContent> stageContentList;

        public List<string> StageTitles
        {
            get
            {
                if (stageContentList == null || stageContentList.Count == 0)
                    return new List<string>();

                int noneTitleCount = 0;
                List<string> titleList = new List<string>();
                foreach (StageSettingContent stageContent in stageContentList)
                {
                    if (string.IsNullOrEmpty(stageContent.StageTitle))
                    {
                        titleList.Add($"new stage {noneTitleCount + 1}");
                        noneTitleCount++;
                    }
                    else
                        titleList.Add(stageContent.StageTitle);
                }

                return titleList;
            }
        }

        public StageSettingContent GetStageSettingContent(string audioKey)
        {
            if (stageContentList == null || stageContentList.Count == 0)
                return null;
            else
                return stageContentList.FirstOrDefault(x => x.AudioKey == audioKey);
        }

        public void AddStage()
        {
            if (stageContentList == null)
                stageContentList = new List<StageSettingContent>();

            stageContentList.Add(new StageSettingContent());
        }
    }
}