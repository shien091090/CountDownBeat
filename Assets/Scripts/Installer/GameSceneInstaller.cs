using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameSceneInstaller : SceneInitializeInstaller
    {
        [Inject] private IAudioManager audioManager;

        [SerializeField] private ViewManager viewManager;

        protected override void ExecuteInstaller()
        {
            audioManager.InitCollectionFromProject();
            Container.Bind<IGameObjectSpawner>().To<GameObjectSpawner>().AsSingle();
            Container.Bind<ICatchNetHandlerPresenter>().To<CatchNetHandlerPresenter>().AsSingle();
            Container.Bind<IScoreBoardPresenter>().To<ScoreBoardPresenter>().AsSingle();
            Container.Bind<IScoreBallHandlerPresenter>().To<ScoreBallHandlerPresenter>().AsSingle();

            BindModelFromInstance<IViewManager, ViewManager>(viewManager);
            BindModel<IScoreBallHandler, ScoreBallHandler>();
            BindModel<IBeaterModel, BeaterModel>();
            BindModel<ICatchNetHandler, CatchNetHandler>();
            BindModel<IScoreBoardModel, ScoreBoardModel>();
        }
    }
}