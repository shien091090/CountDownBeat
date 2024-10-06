using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;

namespace GameCore
{
    public class GameSceneInstaller : ZenjectGameObjectSpawner
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;

        public override void InstallBindings()
        {
            Container.Bind<IGameObjectPool>().FromInstance(objectPoolManager).AsSingle();
        }
    }
}