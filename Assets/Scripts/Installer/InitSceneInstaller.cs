using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using Zenject;

namespace GameCore
{
    public class InitSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAudioManager>().To<FmodAudioManager>().AsSingle();
            Container.Bind<IDeltaTimeGetter>().FromInstance(new DeltaTimeGetter()).AsSingle();
        }
    }
    
}