namespace GameCore
{
    public class BeaterPresenter : IBeaterPresenter
    {
        private readonly IBeaterModel model;
        private IBeaterView view;

        public BeaterPresenter(IBeaterModel model)
        {
            this.model = model;
        }

        public void UpdatePerFrame()
        {
            model.UpdatePerFrame();
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