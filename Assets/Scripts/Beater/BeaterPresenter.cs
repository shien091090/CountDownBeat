using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class BeaterPresenter : IBeaterPresenter
    {
        [Inject] private IViewManager viewManager;

        private IBeaterModel model;
        private readonly Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_BEATER_PRESENTER);

        private IBeaterView view;

        public void BindView(IBeaterView view)
        {
            this.view = view;
            view.SetBeatHintActive(false);
        }

        public void UnbindView()
        {
            view = null;
        }

        public void BindModel(IBeaterModel model)
        {
            this.model = model;
        }

        public void OpenView()
        {
            viewManager.OpenView<BeaterView>(this);
        }

        public void PlayBeatAnimation()
        {
            view?.PlayBeatAnimation();
        }

        public void UnbindModel()
        {
            model = null;
        }
    }
}