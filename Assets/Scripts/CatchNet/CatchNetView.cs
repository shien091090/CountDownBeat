using TMPro;
using UnityEngine;

namespace GameCore
{
    public class CatchNetView : MonoBehaviour, ICatchNetView
    {
        [SerializeField] private TextMeshProUGUI tmp_catchNumber;
        
        private ICatchNetPresenter presenter;

        public void BindPresenter(ICatchNetPresenter presenter)
        {
            this.presenter = presenter;
            presenter.BindView(this);
        }

        public void SetCatchNumber(string catchNumberText)
        {
            tmp_catchNumber.text = catchNumberText;
        }
    }
}