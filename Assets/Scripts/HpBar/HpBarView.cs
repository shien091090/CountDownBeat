using UnityEngine;

namespace GameCore
{
    public class HpBarView : MonoBehaviour, IHpBarView 
    {
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
    }
}