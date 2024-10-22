using Sirenix.OdinInspector;

namespace GameCore
{
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

        [Button("新增關卡", ButtonSizes.Large, Icon = SdfIconType.PlusCircleFill)]
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