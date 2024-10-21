using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using SNShien.Common.TesterTools;
using UnityEditor;

namespace GameCore
{
    public class StageSettingMenuEditorWindow : OdinMenuEditorWindow
    {
        private const string DEBUGGER_KEY = "StageSettingMenuEditorWindow";

        private static StageSettingBaseWindow stageSettingBaseWindow;
        private static Debugger debugger;

        private OdinMenuTree tree;

        [MenuItem("SNTool/Stage Setting Editor")]
        private static void OpenWindow()
        {
            GetWindow<StageSettingMenuEditorWindow>().Show();
            debugger = new Debugger(DEBUGGER_KEY);
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

            for (int i = 0; i < titles.Count; i++)
            {
                string title = titles[i];
                tree.Add($"基本設定/{title}", new SingleStageSettingWindow(stageSetting, stageSetting.stageContentList[i]));
            }

            debugger.ShowLog("RefreshMenuTree");
            tree.UpdateMenuTree();
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
    }
}