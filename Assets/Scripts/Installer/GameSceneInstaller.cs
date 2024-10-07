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
            BindModelFromInstance<IGameObjectPool, ObjectPoolManager>(objectPoolManager);
            BindModel<ICountDownBeatGameModel, CountDownBeatGameModel>();
        }
    }
}