using System;
using Zenject;

namespace GameCore
{
    public class DirectionFlagWallHandler : IDirectionFlagWallHandler
    {
        [Inject] private IDirectionFlagWallHandlerPresenter presenter;

        public event Action OnInit;
        public event Action OnRelease;

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Release()
        {
            OnRelease?.Invoke();
        }

        private void Init()
        {
            presenter.BindModel(this);
            OnInit?.Invoke();
        }
    }
}