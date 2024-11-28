using System.Collections.Generic;
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

        private ICatchNetHandler model;
        private ICatchNetHandlerView view;
        private Dictionary<int, bool> posStateDict = new Dictionary<int, bool>();
        private Dictionary<int, CatchNetSpawnFadeInMode> posFadeInModeDict = new Dictionary<int, CatchNetSpawnFadeInMode>();

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);

        public void Init()
        {
            InitData();
        }

        public ICatchNetView Spawn(int spawnPosIndex)
        {
            return view.Spawn(spawnPosIndex);
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
        }

        public void UnbindView()
        {
            view = null;
        }

        public void UnbindModel()
        {
            model = null;
        }

        // public void SpawnCatchNet(ICatchNetPresenter catchNetPresenter)
        // {
        //     int spawnPosIndex = GetRandomSpawnPosIndex();
        //     if (spawnPosIndex < 0)
        //         return;
        //
        //     view?.Spawn(catchNetPresenter, spawnPosIndex);
        //     catchNetPresenter.Init(spawnPosIndex, posFadeInModeDict[spawnPosIndex]);
        //     catchNetPresenter.OnSuccessCatch -= OnSuccessCatch;
        //     catchNetPresenter.OnSuccessCatch += OnSuccessCatch;
        //
        //     SetPosState(spawnPosIndex, true);
        // }

        public bool TryOccupyPos(out int posIndex, out CatchNetSpawnFadeInMode fadeInMode)
        {
            posIndex = GetRandomSpawnPosIndex();
            if (posIndex >= 0)
            {
                fadeInMode = posFadeInModeDict[posIndex];
                SetPosState(posIndex, true);
                return true;
            }
            else
            {
                fadeInMode = CatchNetSpawnFadeInMode.None;
                return false;
            }
        }

        private void InitData()
        {
            posStateDict = new Dictionary<int, bool>();
            posFadeInModeDict = new Dictionary<int, CatchNetSpawnFadeInMode>();

            List<CatchNetSpawnPos> randomSpawnPosInfoList = view.RandomSpawnPosInfoList;
            for (int i = 0; i < randomSpawnPosInfoList.Count; i++)
            {
                CatchNetSpawnPos posInfo = randomSpawnPosInfoList[i];
                posStateDict[i] = false;
                posFadeInModeDict[i] = posInfo.FadeInMode;
            }
        }

        private int GetRandomSpawnPosIndex()
        {
            List<int> enableSpawnIndexList = GetEnableSpawnIndexList();
            if (enableSpawnIndexList.Count == 0)
                return -1;

            int indexListIndex = Random.Range(0, enableSpawnIndexList.Count);
            int spawnPosIndex = enableSpawnIndexList[indexListIndex];
            return spawnPosIndex;
        }

        private List<int> GetEnableSpawnIndexList()
        {
            List<int> enableSpawnIndexList = new List<int>();
            foreach ((int index, bool isSpawned) in posStateDict)
            {
                if (isSpawned == false)
                    enableSpawnIndexList.Add(index);
            }

            return enableSpawnIndexList;
        }

        private void SetPosState(int index, bool isSpawned)
        {
            if (posStateDict == null)
                return;

            if (posStateDict.ContainsKey(index))
                posStateDict[index] = isSpawned;
        }

        private void OnSuccessCatch(ICatchNetPresenter catchNetPresenter)
        {
            view.PlayCatchSuccessEffect(catchNetPresenter.Position);
        }
    }
}