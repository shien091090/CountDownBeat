using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class CountDownBeatGameModel : ICountDownBeatGameModel
    {
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IViewManager viewManager;

        public void ExecuteModelInit()
        {
            ScoreBallPresenter presenter = new ScoreBallPresenter(eventRegister, eventInvoker, gameSetting);
            viewManager.OpenView<CountDownBeatGameView>(presenter);
        }
    }
}