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
        [Inject] private IGameObjectPool objectPoolManager;

        public void ExecuteModelInit()
        {
            ScoreBallView scoreBall = objectPoolManager.SpawnGameObject<ScoreBallView>(GameConst.PREFAB_NAME_SCORE_BALL);
            ScoreBallPresenter presenter = new ScoreBallPresenter(eventRegister, eventInvoker, gameSetting);
            scoreBall.BindPresenter(presenter);
        }
    }
}