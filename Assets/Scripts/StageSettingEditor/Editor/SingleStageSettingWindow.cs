using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using SNShien.Common.TesterTools;
using UnityEditor;
using UnityEngine;

namespace GameCore
{
    public class SingleStageSettingWindow : OdinEditorWindow
    {
        private const string DEBUGGER_KEY = "SingleStageSettingWindow";

        [VerticalGroup("Split/Left")]
        [ShowInInspector]
        [ReadOnly]
        private StageSettingScriptableObject stageSetting;

        [BoxGroup("Split/Left/曲目設定", CenterLabel = true)]
        [TitleGroup("Split/Left/曲目設定/自動連動設定")]
        [LabelWidth(140)]
        [ReadOnly]
        public string timeLength;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/自動連動設定")]
        [LabelWidth(140)]
        [GUIColor("#6FF06D")]
        [ReadOnly]
        public int beatAmount;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/自動連動設定")]
        [LabelWidth(140)]
        [GUIColor("#6FF06D")]
        [ReadOnly]
        public int spawnScoreBallAmount;

        [HorizontalGroup("Split", Width = 350)]
        [BoxGroup("Split/Left/曲目設定", CenterLabel = true)]
        [TitleGroup("Split/Left/曲目設定/手動設定")]
        [LabelWidth(140)]
        [Required]
        [OnValueChanged("OnSetEventReference")]
        public EventReference fmodEventReference;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/手動設定")]
        [LabelWidth(140)]
        [Required]
        [OnValueChanged("OnSetAudioKey")]
        public string audioKey;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/手動設定")]
        [LabelWidth(140)]
        [MinValue(1)]
        [OnValueChanged("OnSetBpm")]
        public int bpm;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/手動設定")]
        [LabelWidth(140)]
        [MinValue(1)]
        [OnValueChanged("OnSetCountDownBeatFreq")]
        public int countDownBeatFreq;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/手動設定")]
        [LabelWidth(140)]
        [OnValueChanged("OnSetHpDecreasePerSecond")]
        public float hpDecreasePerSecond;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/手動設定")]
        [LabelWidth(140)]
        [OnValueChanged("OnSetHpIncreasePerCatch")]
        public float hpIncreasePerCatch;

        [BoxGroup("Split/Left/曲目設定")]
        [TitleGroup("Split/Left/曲目設定/手動設定")]
        [EnableIf("@beatAmount > 0")]
        [Button("自動生成", ButtonSizes.Large, Icon = SdfIconType.PlusCircleFill)]
        public void AutoCreateSpawnBeatSetting()
        {
            debugger.ShowLog("AutoCreateSpawnBeatSetting");
            spawnBeatSettingData = new bool[1, beatAmount];
        }
        
        [BoxGroup("Split/Left/捕獲旗標設定", CenterLabel = true)]
        [TitleGroup("Split/Left/捕獲旗標設定/設定檔")]
        [LabelWidth(140)]
        [Required]
        [OnValueChanged("OnSetCatchFlagMergeSetting")]
        public CatchFlagMergeScriptableObject catchFlagMergeSetting;

        [BoxGroup("Split/Left/難度設定", CenterLabel = true)]
        [TitleGroup("Split/Left/難度設定/分數球設定")]
        [LabelWidth(140)]
        [MinValue(1)]
        [OnValueChanged("OnSetStartCountDownValue")]
        public int startCountDownValue;

        [BoxGroup("Split/Left/難度設定")]
        [TitleGroup("Split/Left/難度設定/分數球設定")]
        [LabelWidth(140)]
        [MinValue(0)]
        [OnValueChanged("OnSetSuccessSettleScore")]
        public int successSettleScore;

        [BoxGroup("Split/Left/難度設定")]
        [TitleGroup("Split/Left/難度設定/Fever能量條設定")]
        [LabelWidth(140)]
        [RequiredListLength(1, null)]
        [OnValueChanged("OnSetFeverEnergyBarPhaseSetting")]
        public int[] feverEnergyPhaseSettings;

        [BoxGroup("Split/Left/難度設定")]
        [TitleGroup("Split/Left/難度設定/Fever能量條設定")]
        [LabelWidth(140)]
        [MinValue(0)]
        [OnValueChanged("OnSetFeverEnergyIncrease")]
        public int feverEnergyIncrease;

        [BoxGroup("Split/Left/難度設定")]
        [TitleGroup("Split/Left/難度設定/Fever能量條設定")]
        [LabelWidth(140)]
        [MinValue(0)]
        [OnValueChanged("OnSetFeverEnergyDecrease")]
        public int feverEnergyDecrease;

        [BoxGroup("Split/Left/難度設定")]
        [TitleGroup("Split/Left/難度設定/Fever能量條設定")]
        [LabelWidth(140)]
        [RequiredListLength(1, null)]
        [OnValueChanged("OnSetScoreBallFlagWeightSetting")]
        public List<ScoreBallFlagWeightByFeverStageSetting> scoreBallFlagWeightSettings;

        [BoxGroup("Split/Left/難度設定")]
        [TitleGroup("Split/Left/難度設定/Fever能量條設定")]
        [LabelWidth(140)]
        [RequiredListLength(1, null)]
        [OnValueChanged("OnSetCatchNetLimitSetting")]
         public List<CatchNetLimitByFeverStageSetting> catchNetLimitSetting;

        [HorizontalGroup("Split", Width = 140)]
        [VerticalGroup("Split/Right")]
        [BoxGroup("Split/Right/節奏生成分數球設定", CenterLabel = true)]
        [TableMatrix(DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16)]
        [OnValueChanged("OnChangeSpawnBeatSettingData")]
        public bool[,] spawnBeatSettingData;

        private readonly StageSettingContent settingContent;

        private readonly Debugger debugger;

        private EditorEventRef editorEventRef;

        private static bool DrawColoredEnumElement(Rect rect, bool value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            UnityEditor.EditorGUI.DrawRect(rect.Padding(2), value ?
                new Color(0.1f, 0.8f, 0.2f) :
                new Color(0, 0, 0, 0.5f));

            return value;
        }

        public SingleStageSettingWindow(StageSettingScriptableObject stageSetting, StageSettingContent settingContent)
        {
            debugger = new Debugger(DEBUGGER_KEY);
            this.stageSetting = stageSetting;
            this.settingContent = settingContent;
            InitData(settingContent);
        }

        private void InitData(StageSettingContent settingContent)
        {
            InitFieldByScriptableObject(settingContent);
            CheckRefreshBySetEventReference();
            AutoCreateSpawnBeatSetting();
            ParseSpawnBeatSetting(settingContent.SpawnBeatIndexList);
            ParseSpawnScoreBallAmount();
        }

        private void InitFieldByScriptableObject(StageSettingContent settingContent)
        {
            fmodEventReference = settingContent.FmodEventReference;
            editorEventRef = EventManager.EventFromGUID(fmodEventReference.Guid);
            audioKey = settingContent.AudioKey;
            bpm = settingContent.Bpm;
            countDownBeatFreq = settingContent.CountDownBeatFreq;
            hpDecreasePerSecond = settingContent.HpDecreasePerSecond;
            hpIncreasePerCatch = settingContent.HpIncreasePerCatch;
            catchFlagMergeSetting = settingContent.CatchFlagMergeSetting;
            startCountDownValue = settingContent.ScoreBallStartCountDownValue;
            successSettleScore = settingContent.SuccessSettleScore;
            scoreBallFlagWeightSettings = settingContent.ScoreBallFlagWeightSettings;
            catchNetLimitSetting = settingContent.CatchNetLimitByFeverStageSettings;
            feverEnergyPhaseSettings = settingContent.FeverEnergyPhaseSettings;
            feverEnergyIncrease = settingContent.FeverEnergyIncrease;
            feverEnergyDecrease = settingContent.FeverEnergyDecrease;
        }

        private string ConvertTimeLength(int length)
        {
            int minute = length / 60000;
            int second = (length % 60000) / 1000;
            return $"{minute:D2}:{second:D2}";
        }

        private int ConvertBeatAmount(int length, int bpm)
        {
            return (int)(length / 60000f * bpm);
        }

        private List<int> ConvertSpawnBeatIndexList(bool[,] spawnBeatSettingData)
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < spawnBeatSettingData.GetLength(1); i++)
            {
                if (spawnBeatSettingData[0, i])
                    indexList.Add(i);
            }

            return indexList;
        }

        private void ParseSpawnBeatSetting(List<int> spawnBeatIndexList)
        {
            if (spawnBeatIndexList == null ||
                spawnBeatIndexList.Count == 0 ||
                beatAmount == 0)
                return;

            foreach (int index in spawnBeatIndexList)
            {
                if (index >= 0 && index < beatAmount)
                    spawnBeatSettingData[0, index] = true;
            }
        }

        private void ParseSpawnScoreBallAmount()
        {
            if (spawnBeatSettingData == null || spawnBeatSettingData.Length == 0)
                return;

            spawnScoreBallAmount = 0;
            for (int i = 0; i < beatAmount; i++)
            {
                if (spawnBeatSettingData[0, i])
                    spawnScoreBallAmount++;
            }
        }

        private void CheckRefreshBySetEventReference()
        {
            if (editorEventRef == null)
            {
                timeLength = string.Empty;
                beatAmount = 0;
            }
            else
            {
                timeLength = ConvertTimeLength(editorEventRef.Length);
                RefreshBeatAmount();
            }
        }

        private void RefreshBeatAmount()
        {
            if (editorEventRef == null || bpm == 0)
                return;

            beatAmount = ConvertBeatAmount(editorEventRef.Length, bpm);
        }

        private void OnSetCatchFlagMergeSetting()
        {
            settingContent.SetCatchFlagMergeSetting(catchFlagMergeSetting);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetFeverEnergyDecrease()
        {
            settingContent.SetFeverEnergyDecrease(feverEnergyDecrease);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetFeverEnergyIncrease()
        {
            settingContent.SetFeverEnergyIncrease(feverEnergyIncrease);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetFeverEnergyBarPhaseSetting()
        {
            settingContent.SetFeverEnergyBarPhaseSetting(feverEnergyPhaseSettings);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetCatchNetLimitSetting()
        {
            settingContent.SetCatchNetLimitSetting(catchNetLimitSetting);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetScoreBallFlagWeightSetting()
        {
            settingContent.SetScoreBallFlagWeightSetting(scoreBallFlagWeightSettings);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetSuccessSettleScore()
        {
            settingContent.SetSuccessSettleScore(successSettleScore);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetStartCountDownValue()
        {
            settingContent.SetScoreBallStartCountDownValue(startCountDownValue);   
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetCountDownBeatFreq()
        {
            settingContent.SetCountDownBeatFreq(countDownBeatFreq);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetHpDecreasePerSecond()
        {
            settingContent.SetHpDecreasePerSecond(hpDecreasePerSecond);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetHpIncreasePerCatch()
        {
            settingContent.SetHpIncreasePerCatch(hpIncreasePerCatch);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetAudioKey()
        {
            settingContent.SetAudioKey(audioKey);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnChangeSpawnBeatSettingData()
        {
            ParseSpawnScoreBallAmount();
            settingContent.SetSpawnBeatIndexList(ConvertSpawnBeatIndexList(spawnBeatSettingData));
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetBpm()
        {
            RefreshBeatAmount();
            settingContent.SetBpm(bpm);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetEventReference()
        {
            editorEventRef = EventManager.EventFromGUID(fmodEventReference.Guid);
            CheckRefreshBySetEventReference();

            settingContent.SetFmodEventReference(fmodEventReference);
            EditorUtility.SetDirty(stageSetting);
        }
    }
}