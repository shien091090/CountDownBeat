using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;

namespace GameCore
{
    public class GameSceneInstaller : SceneInitializeInstaller
    {
        [SerializeField] private ViewManager viewManager;

        protected override void ExecuteInstaller()
        {
            Container.Bind<IGameObjectSpawner>().To<GameObjectSpawner>().AsSingle();
            BindModelFromInstance<IViewManager, ViewManager>(viewManager);
            BindModel<ICountDownBeatGameModel, CountDownBeatGameModel>();
        }
    }
}