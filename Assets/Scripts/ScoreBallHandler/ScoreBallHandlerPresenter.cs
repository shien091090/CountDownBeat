using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class ScoreBallHandlerPresenter : IScoreBallHandlerPresenter
    {
        [Inject] private IViewManager viewManager;

        private IScoreBallHandler model;
        private IScoreBallHandlerView view;

        public IScoreBallView Spawn()
        {
            return view?.Spawn();
        }

        public void BindView(IScoreBallHandlerView view)
        {
            this.view = view;
        }

        public void BindModel(IScoreBallHandler model)
        {
            this.model = model;

            SetEventRegister(true);
        }

        public void UnbindView()
        {
            view = null;
        }

        private void Init()
        {
            OpenView();
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

        private void Release()
        {
            SetEventRegister(false);
            UnbindModel();
        }

        private void OpenView()
        {
            viewManager.OpenView<ScoreBallHandlerView>(this);
        }

        private void UnbindModel()
        {
            model = null;
        }
    }
}