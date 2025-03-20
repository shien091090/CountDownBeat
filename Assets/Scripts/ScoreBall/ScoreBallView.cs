using System;
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
    [RequireComponent(typeof(TrajectoryAngleRecorder))]
    public class ScoreBallView : MonoBehaviour, IScoreBallView
    {
        private const string PREFAB_NAME_BEAT_EFFECT = "BeatEffect";

        [Inject] private IDeltaTimeGetter deltaTimeGetter;
        [Inject] private IEventInvoker eventInvoker;

        [Header("FlagColor")] [SerializeField] private Color flag1Color;
        [SerializeField] private Color flag2Color;

        [Header("Reference")] [SerializeField] private ObjectPoolManager objectPool;
        [SerializeField] private TextMeshProUGUI tmp_countDownNum;
        [SerializeField] private Image img_back;
        [SerializeField] private GameObject go_directionFlagLeftToRight;
        [SerializeField] private GameObject go_directionFlagRightToLeft;

        public int CurrentFlagNumber => presenter.CurrentFlagNumber;

        private IScoreBallPresenter presenter;
        private OperableUI operableUI;
        private Collider2DAdapterComponent colliderComponent;
        private Animator animator;
        private ComputableCollider computableCollider;
        private TrajectoryAngleRecorder trajectoryAngleRecorder;
        
        private Debugger debugger = new Debugger(DebuggerKeyConst.SCORE_BALL_VIEW);

        public void SetCountDownNumberText(string text)
        {
            tmp_countDownNum.text = text;
        }

        public void SetFrameColor(int colorNum)
        {
            if (colorNum == 1)
                img_back.color = flag1Color;

            if (colorNum == 2)
                img_back.color = flag2Color;
        }

        public void SetDirectionFlag(int directionFlagNum)
        {
            go_directionFlagLeftToRight.SetActive(directionFlagNum == 1);
            go_directionFlagRightToLeft.SetActive(directionFlagNum == 2);
        }

        public void ClearTrajectoryNode()
        {
            trajectoryAngleRecorder.ClearData();
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

        public void HideAllDirectionFlag()
        {
            go_directionFlagLeftToRight.SetActive(false);
            go_directionFlagRightToLeft.SetActive(false);
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

        public void CrossDirectionFlagWall(string crossDirectionKey)
        {
            if (Enum.TryParse(crossDirectionKey, out TriggerFlagMergingType type))
                presenter.CrossDirectionFlagWall(type);
        }

        private void Awake()
        {
            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);

            animator = GetComponent<Animator>();
            operableUI = gameObject.GetComponent<OperableUI>();
            computableCollider = GetComponent<ComputableCollider>();
            trajectoryAngleRecorder = GetComponent<TrajectoryAngleRecorder>();
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