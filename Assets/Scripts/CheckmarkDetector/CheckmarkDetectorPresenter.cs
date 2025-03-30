using System.Collections.Generic;
using System.Linq;
using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public partial class CheckmarkDetectorPresenter : ICheckmarkDetectorPresenter
    {
        [Inject] private IViewManager viewManager;
        [Inject] private IEventRegister eventRegister;

        private readonly Debugger debugger = new Debugger("CheckmarkDetectorPresenter");

        private ICheckmarkDetectorView view;
        private CheckmarkFirstTriggerAreaType firstTriggerAreaType;
        private List<ITriggerAreaColliderHandler> triggerAreaColliderHandlerList;
        private IScoreBallView draggingScoreBall;

        private bool IsAlreadyTriggerFirstArea => firstTriggerAreaType != CheckmarkFirstTriggerAreaType.None;

        public ICollider2DHandler GetColliderHandler(ICheckmarkDetectorTriggerArea triggerAreaComponent)
        {
            if (triggerAreaColliderHandlerList == null)
                triggerAreaColliderHandlerList = new List<ITriggerAreaColliderHandler>();

            ITriggerAreaColliderHandler match = triggerAreaColliderHandlerList.FirstOrDefault(handler => handler.IsTypeMatch(triggerAreaComponent));
            if (match == null)
            {
                TriggerAreaColliderHandler colliderHandler = new TriggerAreaColliderHandler(triggerAreaComponent, this);
                triggerAreaColliderHandlerList.Add(colliderHandler);
                return colliderHandler;
            }
            else
                return match;
        }

        public void BindView(ICheckmarkDetectorView view)
        {
            this.view = view;
        }

        public void UnbindView()
        {
            view = null;
        }

        public void ColliderTriggerEnter(ICheckmarkDetectorTriggerArea triggerAreaComponent, IScoreBallView scoreBall)
        {
            if (IsDraggingScoreBall(scoreBall) == false)
                return;

            if (IsAlreadyTriggerFirstArea)
            {
                if (IsTriggerCheckmark(triggerAreaComponent))
                    scoreBall.TriggerCheckmark();

                firstTriggerAreaType = CheckmarkFirstTriggerAreaType.None;
                RefreshTriggerAreaView();
            }
            else
            {
                firstTriggerAreaType = triggerAreaComponent.FirstTriggerAreaType();
                RefreshTriggerAreaView();
            }
        }

        public void ExecuteModelInit()
        {
            ClearData();
            OpenView();
            view.InitTriggerArea();
            SetEventRegister(true);
            RefreshTriggerAreaView();
        }

        public void Release()
        {
            ClearData();
            SetEventRegister(false);
        }

        private bool IsDraggingScoreBall(IScoreBallView scoreBall)
        {
            if (draggingScoreBall == null)
                return false;

            return draggingScoreBall.GetHashCode() == scoreBall.GetHashCode();
        }

        private bool IsTriggerCheckmark(ICheckmarkDetectorTriggerArea triggerAreaComponent)
        {
            CheckmarkSecondTriggerAreaType secondTriggerAreaType = triggerAreaComponent.SecondTriggerAreaType();
            if (secondTriggerAreaType == CheckmarkSecondTriggerAreaType.None)
                return false;

            switch (firstTriggerAreaType)
            {
                case CheckmarkFirstTriggerAreaType.Right:
                    return secondTriggerAreaType == CheckmarkSecondTriggerAreaType.RightToLeft;

                case CheckmarkFirstTriggerAreaType.Left:
                    return secondTriggerAreaType == CheckmarkSecondTriggerAreaType.LeftToRight;

                case CheckmarkFirstTriggerAreaType.Up:
                    return secondTriggerAreaType == CheckmarkSecondTriggerAreaType.UpToDown;

                case CheckmarkFirstTriggerAreaType.Down:
                    return secondTriggerAreaType == CheckmarkSecondTriggerAreaType.DownToUp;
            }

            return false;
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<ScoreBallOperateEvent>(OnDragScoreBall);

            if (isListen)
            {
                eventRegister.Register<ScoreBallOperateEvent>(OnDragScoreBall);
            }
        }

        private void ClearData()
        {
            firstTriggerAreaType = CheckmarkFirstTriggerAreaType.None;
            draggingScoreBall = null;
        }

        private void RefreshTriggerAreaView()
        {
            view.HideAllFirstTriggerArea();
            view.HideAllSecondTriggerArea();

            if (draggingScoreBall == null)
                return;

            if (IsAlreadyTriggerFirstArea)
            {
                switch (firstTriggerAreaType)
                {
                    case CheckmarkFirstTriggerAreaType.Right:
                        view.SetSecondTriggerAreaActive(CheckmarkSecondTriggerAreaType.RightToLeft, true);
                        break;

                    case CheckmarkFirstTriggerAreaType.Left:
                        view.SetSecondTriggerAreaActive(CheckmarkSecondTriggerAreaType.LeftToRight, true);
                        break;

                    case CheckmarkFirstTriggerAreaType.Up:
                        view.SetSecondTriggerAreaActive(CheckmarkSecondTriggerAreaType.UpToDown, true);
                        break;

                    case CheckmarkFirstTriggerAreaType.Down:
                        view.SetSecondTriggerAreaActive(CheckmarkSecondTriggerAreaType.DownToUp, true);
                        break;
                }
            }
            else
            {
                view.ShowAllFirstTriggerArea();
            }
        }

        private void OpenView()
        {
            viewManager.OpenView<CheckmarkDetectorView>(this);
        }

        private void OnDragScoreBall(ScoreBallOperateEvent eventInfo)
        {
            debugger.ShowLog($"OnDragScoreBall, IsStartDrag: {eventInfo.IsStartDrag}");
            
            if (eventInfo.IsStartDrag)
            {
                draggingScoreBall = eventInfo.Target;
                firstTriggerAreaType = CheckmarkFirstTriggerAreaType.None;
            }
            else
            {
                ClearData();
            }

            RefreshTriggerAreaView();
        }
    }
}