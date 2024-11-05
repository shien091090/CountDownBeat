using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class InitSceneInstaller : MonoInstaller
    {
        [SerializeField] private SceneProcessManager sceneProcessManager;

        public override void InstallBindings()
        {
            Container.Bind<IAudioManager>().To<FmodAudioManager>().AsSingle();
            Container.Bind<IDeltaTimeGetter>().FromInstance(new DeltaTimeGetter()).AsSingle();
            Container.Bind(typeof(IEventInvoker), typeof(IEventRegister)).To<ArchitectureEventHandler>().AsSingle();
            Container.Bind<ISceneProcessManager>().FromInstance(sceneProcessManager).AsSingle();
        }
    }
}