using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class DirectionFlagWallHandlerPresenter : IDirectionFlagWallHandlerPresenter
    {
        [Inject] private IViewManager viewManager;

        private IDirectionFlagWallHandler model;
        private IDirectionFlagWallHandlerView view;

        public void BindModel(IDirectionFlagWallHandler model)
        {
            this.model = model;
            SetEventRegister(true);
        }

        public void BindView(IDirectionFlagWallHandlerView view)
        {
            this.view = view;
        }

        public void UnbindView()
        {
            view = null;
        }

        private void Init()
        {
            OpenView();
            CreateWall();
        }

        private void SetEventRegister(bool isListen)
        {
            model.OnInit -= Init;
            model.OnRelease -= Release;

            if (isListen)
            {
                model.OnInit += Init;
                model.OnRelease += Release;
            }
        }

        private void CreateWall()
        {
            view.Spawn();
        }

        private void OpenView()
        {
            viewManager.OpenView<DirectionFlagWallHandlerView>(this);
        }

        private void Release()
        {
            SetEventRegister(false);
            UnbindModel();
        }

        private void UnbindModel()
        {
            model = null;
        }
    }
}