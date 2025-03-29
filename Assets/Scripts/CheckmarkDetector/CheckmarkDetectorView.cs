using System.Collections.Generic;
using System.Linq;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.TesterTools;
using UnityEngine;

namespace GameCore
{
    public class CheckmarkDetectorView : ArchitectureView, ICheckmarkDetectorView
    {
        [SerializeField] private CheckmarkDetectorTriggerArea[] triggerAreas;

        private Debugger debugger = new Debugger("CheckmarkDetectorView");

        private ICheckmarkDetectorPresenter presenter;
        private List<ICheckmarkDetectorTriggerArea> firstTriggerAreaList;
        private List<ICheckmarkDetectorTriggerArea> secondTriggerAreaList;

        public void SetSecondTriggerAreaActive(CheckmarkSecondTriggerAreaType secondTriggerAreaType, bool isActive)
        {
            if (secondTriggerAreaList == null)
                return;

            ICheckmarkDetectorTriggerArea match = secondTriggerAreaList.FirstOrDefault(x => x.SecondTriggerAreaType() == secondTriggerAreaType);
            if (match == null)
                return;

            if (isActive)
                match.Show();
            else
                match.Hide();
        }

        public void HideAllFirstTriggerArea()
        {
            SetAllFirstTriggerAreaActive(false);
        }

        public void HideAllSecondTriggerArea()
        {
            SetAllSecondTriggerAreaActive(false);
        }

        public void ShowAllFirstTriggerArea()
        {
            SetAllFirstTriggerAreaActive(true);
        }

        public void InitTriggerArea()
        {
            firstTriggerAreaList = new List<ICheckmarkDetectorTriggerArea>();
            secondTriggerAreaList = new List<ICheckmarkDetectorTriggerArea>();

            foreach (CheckmarkDetectorTriggerArea triggerArea in triggerAreas)
            {
                triggerArea.Init(presenter);

                if (triggerArea.IsFirstTriggerType)
                    firstTriggerAreaList.Add(triggerArea);
                else
                    secondTriggerAreaList.Add(triggerArea);
            }
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as ICheckmarkDetectorPresenter;
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            presenter.UnbindView();
        }

        private void SetAllSecondTriggerAreaActive(bool isActive)
        {
            if (secondTriggerAreaList == null)
                return;

            foreach (ICheckmarkDetectorTriggerArea triggerArea in secondTriggerAreaList)
            {
                if (isActive)
                    triggerArea.Show();
                else
                    triggerArea.Hide();
            }
        }

        private void SetAllFirstTriggerAreaActive(bool isActive)
        {
            if (firstTriggerAreaList == null)
                return;

            foreach (ICheckmarkDetectorTriggerArea triggerArea in firstTriggerAreaList)
            {
                if (isActive)
                    triggerArea.Show();
                else
                    triggerArea.Hide();
            }
        }
    }
}