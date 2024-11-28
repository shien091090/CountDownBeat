using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class InitSceneInstaller : MonoInstaller
    {
        [SerializeField] private SceneProcessManager sceneProcessManager;
        [SerializeField] private ViewManager viewManager;

        public override void InstallBindings()
        {
            Container.Bind<IGameObjectSpawner>().To<GameObjectSpawner>().AsSingle();
            Container.Bind<IAudioManager>().To<FmodAudioManager>().AsSingle();
            Container.Bind<IDeltaTimeGetter>().FromInstance(new DeltaTimeGetter()).AsSingle();
            Container.Bind(typeof(IEventInvoker), typeof(IEventRegister)).To<ArchitectureEventHandler>().AsSingle();
            Container.Bind<ISceneProcessManager>().FromInstance(sceneProcessManager).AsSingle();
            Container.Bind<IViewManager>().FromInstance(viewManager).AsSingle();
            Container.Bind<IAppProcessor>().To<AppProcessor>().AsSingle();
        }
    }
}