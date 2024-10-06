using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;

namespace GameCore
{
    public class GameSceneInstaller : SceneInitializeInstaller
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;

        protected override void ExecuteInstaller()
        {
            Container.Bind<IGameObjectPool>().FromInstance(objectPoolManager).AsSingle();
            Container.Bind<ICountDownBeatGameModel>().To<CountDownBeatGameModel>().AsSingle();
        }
    }
}