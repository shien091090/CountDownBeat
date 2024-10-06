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

        public override void InstallBindings()
        {
            Container.Bind<ISceneProcessSetting>().FromInstance(sceneProcessSetting);
            Container.Bind<IGameSetting>().FromInstance(gameSetting);
        }
    }
}