using SNShien.Common.AudioTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class AppProcessor : IAppProcessor
    {
        private const string DEBUGGER_KEY = "InitProcessorModel";

        [Inject] private IAudioManager audioManager;

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);
        private bool isInit;
        private string enterStageAudioKey;

        public void ExecuteModelInit()
        {
            if (isInit)
            {
                debugger.ShowLog("is already init", true);
                return;
            }

            audioManager.InitCollectionFromProject();

            isInit = true;
        }

        public void SetEnterStageAudioKey(string audioKey)
        {
            enterStageAudioKey = audioKey;
        }
    }
}