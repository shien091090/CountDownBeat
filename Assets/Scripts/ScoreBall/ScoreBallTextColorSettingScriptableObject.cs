using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class ScoreBallTextColorSettingScriptableObject : SerializedScriptableObject, IScoreBallTextColorSetting
    {
        [SerializeField] [OnValueChanged("OnMaxValueChanged")] private int maxValue;
        [SerializeField] private List<ScoreBallTextColorDefine> textColorDefines;

        public Color ConvertToColor(int countDownValue)
        {
            foreach (ScoreBallTextColorDefine define in textColorDefines)
            {
                if (define.IsValueMatch(countDownValue))
                    return define.TextColor;
            }

            return default;
        }

        private void InitAllDefineMaxValue()
        {
            foreach (ScoreBallTextColorDefine define in textColorDefines)
            {
                define.MaxValue = maxValue;
            }
        }

        private void OnMaxValueChanged()
        {
            InitAllDefineMaxValue();
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (textColorDefines == null || textColorDefines.Count == 0)
                return;

            InitAllDefineMaxValue();
        }
    }
}