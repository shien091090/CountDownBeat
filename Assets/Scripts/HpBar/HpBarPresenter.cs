using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class HpBarPresenter : IHpBarPresenter
    {
        [Inject] private IViewManager viewManager;

        private IHpBarModel model;
        private IHpBarView view;

        public void UpdateFrame()
        {
            model?.UpdateFrame();
        }

        public void BindModel(IHpBarModel model)
        {
            this.model = model;

            SetEventRegister(true);
        }

        public void BindView(IHpBarView view)
        {
            this.view = view;
        }

        public void UnbindView()
        {
            view = null;
        }

        private void Init()
        {
            OpenView();
        }

        public void RefreshHp(float currentHp)
        {
            if (model.MaxHp == 0)
                view?.RefreshHpSliderValue(0);
            else
            {
                float hpSliderValue = currentHp / model.MaxHp;
                view?.RefreshHpSliderValue(hpSliderValue);
            }
        }

        public void UnbindModel()
        {
            model = null;
        }

        private void SetEventRegister(bool isListen)
        {
            model.OnInit -= Init;
            model.OnRelease -= Release;
            model.OnRefreshHp -= RefreshHp;

            if (isListen)
            {
                model.OnInit += Init;
                model.OnRelease += Release;
                model.OnRefreshHp += RefreshHp;
            }
        }

        private void Release()
        {
            SetEventRegister(false);
            UnbindModel();
        }

        private void OpenView()
        {
            viewManager.OpenView<HpBarView>(this);
        }
    }
}