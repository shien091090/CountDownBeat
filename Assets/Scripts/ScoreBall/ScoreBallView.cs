using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    [RequireComponent(typeof(ComputableCollider))]
    [RequireComponent(typeof(TrajectoryAngleCalculator))]
    public class ScoreBallView : MonoBehaviour, IScoreBallView
    {
        private const string PREFAB_NAME_BEAT_EFFECT = "BeatEffect";

        [Inject] private IDeltaTimeGetter deltaTimeGetter;
        [Inject] private IEventInvoker eventInvoker;

        [SerializeField] private ObjectPoolManager objectPool;
        [SerializeField] private TextMeshProUGUI tmp_countDownNum;
        [SerializeField] private Image img_back;

        public int CurrentFlagNumber => presenter.CurrentFlagNumber;

        private Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_SCORE_BALL_VIEW);
        private IScoreBallPresenter presenter;
        private OperableUI operableUI;
        private Collider2DAdapterComponent colliderComponent;
        private Animator animator;
        private ComputableCollider computableCollider;
        private TrajectoryAngleCalculator trajectoryAngleCalculator;

        public void SetCountDownNumberText(string text)
        {
            tmp_countDownNum.text = text;
        }

        public void SetFrameColor(Color color)
        {
            img_back.color = color;
        }

        public void RecordTrajectoryNode()
        {
            trajectoryAngleCalculator.RecordPositionNode();
        }

        public void ClearTrajectoryNode()
        {
            trajectoryAngleCalculator.ClearData();
        }

        public void SetTextColor(Color color)
        {
            tmp_countDownNum.color = color;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void TriggerCatch()
        {
            presenter.TriggerCatch();
        }

        public void BindPresenter(IMVPPresenter mvpPresenter)
        {
            presenter = (IScoreBallPresenter)mvpPresenter;
        }

        public void UnbindPresenter()
        {
            presenter = null;
        }

        public void CreateBeatEffectPrefab()
        {
            objectPool.SpawnGameObject(PREFAB_NAME_BEAT_EFFECT);
        }

        public void PlayAnimation(string animKey)
        {
            animator.Play(animKey, 0, 0);
        }

        public void Init()
        {
            operableUI.Init();
            RegisterEvent();
        }

        private void Update()
        {
            operableUI.UpdatePerFrame(deltaTimeGetter.deltaTime);
        }

        public void CrossResetWall()
        {
            presenter.CrossResetWall();
        }

        private void Awake()
        {
            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);

            animator = GetComponent<Animator>();
            operableUI = gameObject.GetComponent<OperableUI>();
            computableCollider = GetComponent<ComputableCollider>();
            trajectoryAngleCalculator = GetComponent<TrajectoryAngleCalculator>();
        }

        private void RegisterEvent()
        {
            operableUI.OnStartDragEvent -= StartDrag;
            operableUI.OnStartDragEvent += StartDrag;

            operableUI.OnDragOverEvent -= DragOver;
            operableUI.OnDragOverEvent += DragOver;
        }

        private void DragOver()
        {
            presenter.DragOver();
            eventInvoker.SendEvent(new ScoreBallOperateEvent(false, computableCollider));
        }

        private void StartDrag()
        {
            presenter.StartDrag();
            eventInvoker.SendEvent(new ScoreBallOperateEvent(true, computableCollider));
        }
    }
}