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

        [VerticalGroup("Split/Left")] [InfoBox("先設定EventReference、填入BPM，再按下'自動生成'")] [LabelWidth(140)] [ReadOnly]
        public string eventReferenceName;

        [LabelWidth(140)] [ReadOnly] [VerticalGroup("Split/Left")] public string timeLength;

        [LabelWidth(140)] [ReadOnly] [VerticalGroup("Split/Left")] [GUIColor("#6FF06D")]
        public int beatAmount;

        [LabelWidth(140)] [ReadOnly] [VerticalGroup("Split/Left")] [GUIColor("#6FF06D")]
        public int spawnScoreBallAmount;

        [HorizontalGroup("Split", Width = 350)] [LabelWidth(140)] [BoxGroup("Split/Left/基本設定", CenterLabel = true)] [OnValueChanged("OnSetEventReference")] [Required]
        public EditorEventRef fmodEventReference;

        [BoxGroup("Split/Left/基本設定")] [LabelWidth(140)] [Required] [OnValueChanged("OnSetAudioKey")]
        public string audioKey;

        [BoxGroup("Split/Left/基本設定")] [LabelWidth(140)] [MinValue(1)] [OnValueChanged("OnSetBpm")]
        public int bpm;

        [HorizontalGroup("Split", Width = 140)]
        [VerticalGroup("Split/Right")]
        [BoxGroup("Split/Right/節奏生成分數球設定", CenterLabel = true)]
        [TableMatrix(DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16)]
        [OnValueChanged("OnChangeSpawnBeatSettingData")]
        public bool[,] spawnBeatSettingData;

        private readonly StageSettingScriptableObject stageSetting;
        private readonly StageSettingContent settingContent;
        private readonly Debugger debugger;

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
            fmodEventReference = settingContent.FmodEventReference;
            audioKey = settingContent.AudioKey;

            if (fmodEventReference != null)
                timeLength = ConvertTimeLength(fmodEventReference.Length);

            bpm = settingContent.Bpm;
            RefreshBeatAmount();

            AutoCreateSpawnBeatSetting();
            ParseSpawnBeatSetting(settingContent.SpawnBeatIndexList);
        }

        [VerticalGroup("Split/Left")]
        [Button("自動生成")]
        [EnableIf("@beatAmount > 0")]
        public void AutoCreateSpawnBeatSetting()
        {
            debugger.ShowLog("AutoCreateSpawnBeatSetting");
            spawnBeatSettingData = new bool[1, beatAmount];
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

        private void ParseSpawnBeatSetting(List<int> spawnBeatIndexList)
        {
            if (spawnBeatIndexList == null || spawnBeatIndexList.Count == 0)
                return;

            foreach (int index in spawnBeatIndexList)
            {
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

        private void RefreshBeatAmount()
        {
            if (fmodEventReference == null || bpm == 0)
                return;

            beatAmount = ConvertBeatAmount(fmodEventReference.Length, bpm);
        }

        private void OnSetAudioKey()
        {
            settingContent.SetAudioKey(audioKey);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnChangeSpawnBeatSettingData()
        {
            ParseSpawnScoreBallAmount();
        }

        private void OnSetBpm()
        {
            RefreshBeatAmount();
            settingContent.SetBpm(bpm);
            EditorUtility.SetDirty(stageSetting);
        }

        private void OnSetEventReference()
        {
            if (fmodEventReference == null)
            {
                eventReferenceName = string.Empty;
                timeLength = string.Empty;
                beatAmount = 0;
            }
            else
            {
                eventReferenceName = fmodEventReference.name;
                timeLength = ConvertTimeLength(fmodEventReference.Length);
                RefreshBeatAmount();
            }

            settingContent.SetFmodEventReference(fmodEventReference);
            EditorUtility.SetDirty(stageSetting);
        }
    }
}