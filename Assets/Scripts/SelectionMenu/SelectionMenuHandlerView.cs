using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class SelectionMenuHandlerView : MonoBehaviour, ISelectionMenuHandlerView
    {
        private ISelectionMenuHandlerPresenter presenter;

        public void UpdateView()
        {
            throw new System.NotImplementedException();
        }

        public void OpenView(params object[] parameters)
        {
            presenter = (ISelectionMenuHandlerPresenter)parameters[0];
            presenter.BindView(this);
        }

        public void ReOpenView(params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void OnClickEnterStage1()
        {
            presenter.ClickEnterStage(0);
            // eventInvoker.SendEvent(new SwitchSceneEvent(GameConst.SCENE_REPOSITION_ACTION_ENTER_GAME));
        }
    }
}