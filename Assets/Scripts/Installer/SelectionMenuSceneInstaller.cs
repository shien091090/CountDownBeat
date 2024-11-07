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

        private void CheckInit()
        {
            appProcessor.CheckInit();
        }

        private void RegisterEvent()
        {
            OnPreStartInitModel -= CheckInit;
            OnPreStartInitModel += CheckInit;
        }
    }
}