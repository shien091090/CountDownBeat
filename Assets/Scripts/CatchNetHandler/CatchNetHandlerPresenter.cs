using System.Collections.Generic;
using SNShien.Common.DataTools;
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
        private JsonParser jsonParser = new JsonParser();

        public ICatchNetView Spawn(int spawnPosIndex)
        {
            return view.Spawn(spawnPosIndex);
        }

        public void BindModel(ICatchNetHandler model)
        {
            this.model = model;

            SetEventRegister(true);
        }

        public void BindView(ICatchNetHandlerView view)
        {
            this.view = view;
        }

        private void FreeUpPosAndRefreshCurrentCount(int spawnIndex)
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

        private void PlaySuccessCatchEffect(ICatchNetPresenter catchNetPresenter)
        {
            view.PlayCatchSuccessEffect(catchNetPresenter.Position);
        }

        private void Init()
        {
            OpenView();
            InitData();
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

        private void SetEventRegister(bool isListen)
        {
            model.OnInit -= Init;
            model.OnRelease -= Release;
            model.OnSettleCatchNet -= SettleCatchNet;

            if (isListen)
            {
                model.OnInit += Init;
                model.OnRelease += Release;
                model.OnSettleCatchNet += SettleCatchNet;
            }
        }

        private void SettleCatchNet(ICatchNetPresenter catchNetPresenter)
        {
            FreeUpPosAndRefreshCurrentCount(catchNetPresenter.SpawnPosIndex);
            PlaySuccessCatchEffect(catchNetPresenter);
        }

        private void SetPosState(int index, bool isSpawned)
        {
            if (posStateDict == null)
                return;

            if (posStateDict.ContainsKey(index))
                posStateDict[index] = isSpawned;
        }

        private void Release()
        {
            SetEventRegister(false);
            UnbindModel();
        }

        private void OpenView()
        {
            viewManager.OpenView<CatchNetHandlerView>(this);
        }
    }
}