using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class SelectionMenuHandler : ISelectionMenuHandler
    {
        [Inject] private ISelectionMenuHandlerPresenter presenter;
        [Inject] private IAppProcessor appProcessor;

        public void ExecuteModelInit()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        public void Release()
        {
            presenter.UnbindModel();
        }

        public void EnterStage(int stageIndex)
        {
            string audioKey = string.Empty;

            switch (stageIndex)
            {
                case 0:
                    audioKey = AudioNameConst.AUDIO_NAME_BGM_1;
                    break;
            }

            appProcessor.ExecuteEnterStage(audioKey);
        }
    }
}