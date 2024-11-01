using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class HpBarPresenter : IHpBarPresenter
    {
        // [Inject] private ViewManager viewManager;

        private IHpBarModel model;
        private IHpBarView view;

        public void RefreshHp(float currentHp)
        {
            if (model.MaxHp == 0)
                view.RefreshHpSliderValue(0);
            else
            {
                float hpSliderValue = currentHp / model.MaxHp;
                view.RefreshHpSliderValue(hpSliderValue);
            }
        }

        public void BindModel(IHpBarModel model)
        {
            this.model = model;
        }

        public void OpenView()
        {
            // viewManager.OpenView<HpBarView>(this);
        }

        public void BindView(IHpBarView view)
        {
            this.view = view;
        }
    }
}