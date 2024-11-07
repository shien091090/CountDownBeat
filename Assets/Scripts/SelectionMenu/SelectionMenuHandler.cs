using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class SelectionMenuHandler : ISelectionMenuHandler
    {
        [Inject] private ISelectionMenuHandlerPresenter presenter;
        [Inject] private IAppProcessor appProcessor;
        [Inject] private IEventInvoker eventInvoker;

        public void ExecuteModelInit()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        public void EnterStage(int stageIndex)
        {
            string audioKey = string.Empty;

            switch (stageIndex)
            {
                case 0:
                    audioKey = GameConst.AUDIO_NAME_BGM_1;
                    break;
            }

            appProcessor.ExecuteEnterStage(audioKey);
        }
    }
}