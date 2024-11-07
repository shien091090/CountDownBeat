using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class SelectionMenuSceneInstaller : SceneInitializeInstaller
    {
        [Inject] private IAppProcessor appProcessor;

        protected override void ExecuteInstaller()
        {
            RegisterEvent();

            Container.Bind<ISelectionMenuHandlerPresenter>().To<SelectionMenuHandlerPresenter>().AsSingle();

            BindModel<ISelectionMenuHandler, SelectionMenuHandler>();
        }

        private void RegisterEvent()
        {
            OnPreStartInitModel -= appProcessor.EnterSelectionMenu;
            OnPreStartInitModel += appProcessor.EnterSelectionMenu;
        }
    }
}