using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    [CreateAssetMenu]
    public class ExternalSettingInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private SceneProcessScriptableObject sceneProcessSetting;
        [SerializeField] private GameSettingScriptableObject gameSetting;
        [SerializeField] private ViewPrefabScriptableObject viewPrefabSetting;
        [SerializeField] private ArchitectureModelSettingScriptableObject modelSetting;

        public override void InstallBindings()
        {
            Container.Bind<ISceneProcessSetting>().FromInstance(sceneProcessSetting);
            Container.Bind<IGameSetting>().FromInstance(gameSetting);
            Container.Bind<IViewPrefabSetting>().FromInstance(viewPrefabSetting);
            Container.Bind<IArchitectureModelSetting>().FromInstance(modelSetting);
        }
    }
}