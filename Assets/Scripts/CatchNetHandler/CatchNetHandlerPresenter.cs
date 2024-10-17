using System.Collections.Generic;
using System.Linq;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.TesterTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class CatchNetHandlerPresenter : ICatchNetHandlerPresenter
    {
        private const string DEBUGGER_KEY = "CatchNetHandlerPresenter";

        [Inject] private IViewManager viewManager;

        public int CurrentCatchNetCount { get; private set; }

        private ICatchNetHandler model;
        private ICatchNetHandlerView view;
        private Dictionary<int, bool> posStateDict;

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);

        public void Init()
        {
            InitPosStateDict();
            UpdateCurrentCatchNetCount();
        }

        public void SpawnCatchNet(ICatchNetPresenter catchNetPresenter)
        {
            List<int> enableSpawnIndexList = new List<int>();
            foreach ((int index, bool isSpawned) in posStateDict)
            {
                if (isSpawned == false)
                    enableSpawnIndexList.Add(index);
            }

            if (enableSpawnIndexList.Count == 0)
                return;

            int indexListIndex = Random.Range(0, enableSpawnIndexList.Count);
            int spawnPosIndex = enableSpawnIndexList[indexListIndex];
            view.Spawn(catchNetPresenter, spawnPosIndex);
            catchNetPresenter.SetSpawnPosIndex(spawnPosIndex);

            SetPosState(spawnPosIndex, true);
            UpdateCurrentCatchNetCount();
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

        public void FreeUpPosAndRefreshCurrentCount(int spawnIndex)
        {
            SetPosState(spawnIndex, false);
            UpdateCurrentCatchNetCount();
        }

        private void InitPosStateDict()
        {
            posStateDict = new Dictionary<int, bool>();
            for (int i = 0; i < view.RandomSpawnPositionList.Count; i++)
            {
                posStateDict.Add(i, false);
            }
        }

        private void SetPosState(int index, bool isSpawned)
        {
            if (posStateDict == null)
                return;

            if (posStateDict.ContainsKey(index))
                posStateDict[index] = isSpawned;
        }

        private void UpdateCurrentCatchNetCount()
        {
            CurrentCatchNetCount = 0;
            foreach (bool isSpawned in posStateDict.Values.Where(isSpawned => isSpawned))
            {
                CurrentCatchNetCount++;
            }
        }
    }
}