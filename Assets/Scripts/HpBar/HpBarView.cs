using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class HpBarView : MonoBehaviour, IHpBarView
    {
        [SerializeField] private Slider sld_hpBar;

        private IHpBarPresenter presenter;

        public void UpdateView()
        {
        }

        public void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IHpBarPresenter;
            presenter.BindView(this);
        }

        public void ReOpenView(params object[] parameters)
        {
        }

        public void RefreshHpSliderValue(float value)
        {
            sld_hpBar.value = value;
        }
    }
}