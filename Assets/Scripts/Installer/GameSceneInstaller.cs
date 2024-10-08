using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;

namespace GameCore
{
    public class GameSceneInstaller : SceneInitializeInstaller
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private ViewManager viewManager;

        protected override void ExecuteInstaller()
        {
            BindModelFromInstance<IGameObjectPool, ObjectPoolManager>(objectPoolManager);
            BindModelFromInstance<IViewManager, ViewManager>(viewManager);
            BindModel<ICountDownBeatGameModel, CountDownBeatGameModel>();
        }
    }
}