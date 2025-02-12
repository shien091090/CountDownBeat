using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class GameSceneInstaller : SceneInitializeInstaller
    {
        protected override void ExecuteInstaller()
        {
            Container.Bind<ICatchNetHandlerPresenter>().To<CatchNetHandlerPresenter>().AsSingle();
            Container.Bind<IScoreBoardPresenter>().To<ScoreBoardPresenter>().AsSingle();
            Container.Bind<IScoreBallHandlerPresenter>().To<ScoreBallHandlerPresenter>().AsSingle();
            Container.Bind<IHpBarPresenter>().To<HpBarPresenter>().AsSingle();
            Container.Bind<IBeaterPresenter>().To<BeaterPresenter>().AsSingle();
            Container.Bind<IFeverEnergyBarPresenter>().To<FeverEnergyBarPresenter>().AsSingle();

            BindModel<IScoreBallHandler, ScoreBallHandler>();
            BindModel<IBeaterModel, BeaterModel>();
            BindModel<ICatchNetHandler, CatchNetHandler>();
            BindModel<IScoreBoardModel, ScoreBoardModel>();
            BindModel<IHpBarModel, HpBarModel>();
            BindModel<IFeverEnergyBarModel, FeverEnergyBarModel>();
        }
    }
}