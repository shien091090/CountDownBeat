using SNShien.Common.MonoBehaviorTools;

namespace GameCore
{
    public class SelectionMenuHandlerView : ArchitectureView, ISelectionMenuHandlerView
    {
        private ISelectionMenuHandlerPresenter presenter;

        public override void UpdateView()
        {
            throw new System.NotImplementedException();
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = (ISelectionMenuHandlerPresenter)parameters[0];
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public override void CloseView()
        {
            presenter.UnbindView();
        }

        public void OnClickEnterStage1()
        {
            presenter.ClickEnterStage(0);
            // eventInvoker.SendEvent(new SwitchSceneEvent(GameConst.SCENE_REPOSITION_ACTION_ENTER_GAME));
        }
    }
}