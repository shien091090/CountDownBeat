using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using SNShien.Common.TesterTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCore
{
    public class StageSettingMenuEditorWindow : OdinMenuEditorWindow
    {
        private const string DEBUGGER_KEY = "StageSettingMenuEditorWindow";

        private static StageSettingBaseWindow stageSettingBaseWindow;
        private static Debugger debugger;

        [ShowIf("@stageSetting != null")] [VerticalGroup("Split/Left")] [InfoBox("先設定EventReference、填入BPM，再按下'自動生成'")] [LabelWidth(140)] [ReadOnly]
        public string eventReferenceName;

        [ShowIf("@stageSetting != null")] [LabelWidth(140)] [ReadOnly] [VerticalGroup("Split/Left")]
        public string timeLength;

        [ShowIf("@stageSetting != null")] [LabelWidth(140)] [ReadOnly] [VerticalGroup("Split/Left")] [GUIColor("#6FF06D")]
        public int beatAmount;

        [ShowIf("@stageSetting != null")]
        [HorizontalGroup("Split", Width = 350)]
        [LabelWidth(140)]
        [BoxGroup("Split/Left/基本設定", CenterLabel = true)]
        [OnValueChanged("OnSetEventReference")]
        [Required]
        public EditorEventRef fmodEventReference;

        [ShowIf("@stageSetting != null")] [BoxGroup("Split/Left/基本設定")] [LabelWidth(140)] [MinValue(1)] [OnValueChanged("OnSetBpm")]
        public int bpm;

        [ShowIf("@stageSetting != null")]
        [HorizontalGroup("Split", Width = 140)]
        [VerticalGroup("Split/Right")]
        [BoxGroup("Split/Right/節奏生成分數球設定", CenterLabel = true)]
        [TableMatrix(DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16)]
        public bool[,] spawnBeatSettingData;

        private OdinMenuTree tree;

        [MenuItem("SNTool/Stage Setting Editor")]
        private static void OpenWindow()
        {
            GetWindow<StageSettingMenuEditorWindow>().Show();
            debugger = new Debugger(DEBUGGER_KEY);
        }

#if UNITY_EDITOR // Editor-related code must be excluded from builds
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

#endif

        protected override void OnDisable()
        {
            debugger.ShowLog("OnDisable");
            base.OnDisable();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            debugger.ShowLog("BuildMenuTree");

            InitMenuTree();
            AddBaseSettingInTree();
            return tree;
        }

        private void InitMenuTree()
        {
            tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;
        }

        public void RefreshMenuTree()
        {
            if (tree == null)
            {
                debugger.ShowLog("tree is null", true);
                return;
            }

            if (stageSettingBaseWindow.CheckStageSetting == false)
            {
                debugger.ShowLog("check stage setting failed", true);
                RemoveMenuTreeChildSelection();
                AddBaseSettingInTree(true);
                tree.MenuItems[0].Select();
                return;
            }

            StageSettingScriptableObject stageSetting = stageSettingBaseWindow.GetStageSetting;
            List<string> titles = stageSetting.StageTitles;

            string titleJoin = string.Join(", ", titles.ToArray());
            debugger.ShowLog($"titleList: {titleJoin}", true);

            foreach (string title in titles)
            {
                tree.Add($"基本設定/{title}", new SingleStageSettingWindow());
            }

            debugger.ShowLog("RefreshMenuTree");
            tree.UpdateMenuTree();
        }

        [ShowIf("@stageSetting != null")]
        [VerticalGroup("Split/Left")]
        [Button("自動生成")]
        [EnableIf("@beatAmount > 0")]
        public void AudoCreateSpawnBeatSetting()
        {
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

        private void AddBaseSettingInTree(bool isRefresh = false)
        {
            stageSettingBaseWindow ??= new StageSettingBaseWindow(this);
            tree.Add("基本設定", stageSettingBaseWindow);
        }

        private void RemoveMenuTreeChildSelection(bool isRefresh = false)
        {
            List<OdinMenuItem> treeMenuItems = tree.MenuItems;
            treeMenuItems[0].Remove();
            tree.UpdateMenuTree();
        }

        private void RefreshBeatAmount()
        {
            if (fmodEventReference == null || bpm == 0)
                return;

            beatAmount = ConvertBeatAmount(fmodEventReference.Length, bpm);
        }

        private void OnSetBpm()
        {
            RefreshBeatAmount();
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
        }
    }

    public class SingleStageSettingWindow
    {}

    public class StageSettingBaseWindow
    {
        [Required("請先建立一個StageSettingScriptableObject")] [OnValueChanged("OnSetStageSetting")]
        public StageSettingScriptableObject stageSetting;

        private readonly StageSettingMenuEditorWindow mainEditor;
        public bool CheckStageSetting => stageSetting != null && stageSetting.stageContentList != null && stageSetting.stageContentList.Count > 0;
        public StageSettingScriptableObject GetStageSetting => stageSetting;

        public StageSettingBaseWindow(StageSettingMenuEditorWindow mainEditor)
        {
            this.mainEditor = mainEditor;
        }

        [Button("新增關卡")]
        [ShowIf("@stageSetting != null")]
        public void AddStage()
        {
            if (stageSetting == null)
                return;

            stageSetting.AddStage();
            OnSetStageSetting();
        }

        private void OnSetStageSetting()
        {
            mainEditor.RefreshMenuTree();
        }
    }
}