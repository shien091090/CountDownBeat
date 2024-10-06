using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameSceneProcessor : MonoBehaviour
    {
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IGameObjectPool objectPoolManager;

        public void Start()
        {
            ScoreBallView scoreBall = objectPoolManager.SpawnGameObject<ScoreBallView>(GameConst.PREFAB_NAME_SCORE_BALL);
            ScoreBallPresenter presenter = new ScoreBallPresenter(eventRegister, eventInvoker, gameSetting);
            scoreBall.BindPresenter(presenter);
        }
    }
}