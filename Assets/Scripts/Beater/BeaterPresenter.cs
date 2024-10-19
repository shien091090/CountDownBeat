using SNShien.Common.TesterTools;

namespace GameCore
{
    public class BeaterPresenter : IBeaterPresenter
    {
        private readonly IBeaterModel model;
        private readonly Debugger debugger;

        private IBeaterView view;

        public BeaterPresenter(IBeaterModel model)
        {
            debugger = new Debugger(GameConst.DEBUGGER_KEY_BEATER_PRESENTER);
            this.model = model;
        }

        public void BindView(IBeaterView view)
        {
            this.view = view;
            view.SetBeatHintActive(false);
        }

        public void PlayBeatAnimation()
        {
            view.PlayBeatAnimation();
        }
    }
}