using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class SelectionMenuSceneInstaller : SceneInitializeInstaller
    {
        protected override void ExecuteInstaller()
        {
            Container.Bind<ISelectionMenuHandlerPresenter>().To<SelectionMenuHandlerPresenter>().AsSingle();

            BindModel<ISelectionMenuHandler, SelectionMenuHandler>();
        }
    }
}