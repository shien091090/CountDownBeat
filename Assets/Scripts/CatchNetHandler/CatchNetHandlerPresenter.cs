using System.Collections.Generic;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class CatchNetHandlerPresenter : ICatchNetHandlerPresenter
    {
        [Inject] private IViewManager viewManager;

        public int CurrentCatchNetCount { get; private set; }

        private ICatchNetHandler model;
        private ICatchNetHandlerView view;
        private Dictionary<int, bool> posStateDict;

        public void Init()
        {
            CurrentCatchNetCount = 0;
            InitPosStateDict();
        }

        public bool TrySpawnCatchNet(ICatchNetPresenter catchNetPresenter)
        {
            List<int> enableSpawnIndexList = new List<int>();
            foreach ((int index, bool isSpawned) in posStateDict)
            {
                if (isSpawned == false)
                    enableSpawnIndexList.Add(index);
            }
            
            if(enableSpawnIndexList.Count == 0)
                return false;
            
            int indexListIndex = Random.Range(0, enableSpawnIndexList.Count);
            int spawnPosIndex = enableSpawnIndexList[indexListIndex];
            view.Spawn(catchNetPresenter, spawnPosIndex);
            CurrentCatchNetCount++;
            return true;
        }

        public void BindModel(ICatchNetHandler model)
        {
            this.model = model;
        }

        public void BindView(ICatchNetHandlerView view)
        {
            this.view = view;
        }

        public void OpenView()
        {
            viewManager.OpenView<CatchNetHandlerView>(this);
        }

        private void InitPosStateDict()
        {
            posStateDict = new Dictionary<int, bool>();
            for (int i = 0; i < view.RandomSpawnPositionList.Count; i++)
            {
                posStateDict.Add(i, false);
            }
        }
    }
}