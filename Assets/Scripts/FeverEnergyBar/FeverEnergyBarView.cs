using System.Collections;
using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.TesterTools;
using TMPro;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class FeverEnergyBarView : ArchitectureView, IFeverEnergyBarView
    {
        [SerializeField] private TextMeshProUGUI tmp_feverStage;
        [SerializeField] private TextMeshProUGUI tmp_currentFeverEnergy;
        [SerializeField] private Color increaseColor;
        [SerializeField] private Color decreaseColor;

        [Inject] private IInputGetter inputGetter;

        private IFeverEnergyBarPresenter presenter;
        private Debugger debugger = new Debugger("FeverEnergyBarView");

        public void SetFeverStage(int newFeverStage)
        {
            tmp_feverStage.text = newFeverStage.ToString();
        }

        public void SetCurrentFeverEnergy(int currentFeverEnergy)
        {
            tmp_currentFeverEnergy.text = currentFeverEnergy.ToString();
        }

        public void PlayFeverEnergyIncreaseEffect()
        {
            StartCoroutine(Cor_PlayFeverEnergyTextEffect(increaseColor));
        }

        public void PlayFeverEnergyDecreaseEffect()
        {
            StartCoroutine(Cor_PlayFeverEnergyTextEffect(decreaseColor));
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IFeverEnergyBarPresenter;
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            presenter.UnbindView();
        }

        private void Update()
        {
            if (inputGetter.IsClickDown())
                presenter.Hit();
        }

        private IEnumerator Cor_PlayFeverEnergyTextEffect(Color textColor)
        {
            tmp_currentFeverEnergy.color = textColor;

            yield return new WaitForSeconds(0.3f);

            tmp_currentFeverEnergy.color = Color.white;
        }
    }
}