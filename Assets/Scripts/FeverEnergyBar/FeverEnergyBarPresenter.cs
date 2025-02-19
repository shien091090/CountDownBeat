using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class FeverEnergyBarPresenter : IFeverEnergyBarPresenter
    {
        [Inject] private IViewManager viewManager;

        private IFeverEnergyBarModel model;
        private IFeverEnergyBarView view;

        public void BindModel(IFeverEnergyBarModel model)
        {
            this.model = model;

            SetEventRegister(true);
        }

        public void UnbindModel()
        {
            SetEventRegister(false);

            model = null;
        }

        public void OpenView()
        {
            viewManager.OpenView<FeverEnergyBarView>(this);
        }

        public void BindView(IFeverEnergyBarView view)
        {
            this.view = view;
        }

        public void UnbindView()
        {
            view = null;
        }

        public void Hit()
        {
            model.Hit();
        }

        private void SetEventRegister(bool isListen)
        {
            model.OnUpdateFeverEnergyValue -= OnUpdateFeverEnergyValue;
            model.OnUpdateFeverStage -= OnUpdateFeverStage;

            if (isListen)
            {
                model.OnUpdateFeverEnergyValue += OnUpdateFeverEnergyValue;
                model.OnUpdateFeverStage += OnUpdateFeverStage;
            }
        }

        private void OnUpdateFeverStage(int newFeverStage)
        {
            view.SetFeverStage(newFeverStage);
        }

        private void OnUpdateFeverEnergyValue(UpdateFeverEnergyBarEvent eventInfo)
        {
            view.SetCurrentFeverEnergy(eventInfo.AfterEnergyValue);

            if (eventInfo.AfterEnergyValue > eventInfo.BeforeEnergyValue)
                view.PlayFeverEnergyIncreaseEffect();
            else
                view.PlayFeverEnergyDecreaseEffect();
        }
    }
}