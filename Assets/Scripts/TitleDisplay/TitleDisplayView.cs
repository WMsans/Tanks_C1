using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public enum TitleDisplayState
    {
        Hide = 0,
        Main,
        LocalMain,
        ConfirmClearProgress,
        MsgProgressCleared,
        PickLevel,
        Multiplayer,
        Num
    }
public class TitleDisplayView : MonoBehaviour
{
    public GameObject MainPanelObj;
    public GameObject LocalMainPanelObj;
    public GameObject ConfirmClearProgressPanelObj;
    public GameObject MsgProgressClearedPanelObj;
    public GameObject PickLevelPanelObj;
    public GameObject levelGridRoot;
    public GameObject levelGridChildPrefab;
    List<GameObject> panels = new List<GameObject>();
    private StateMachineRunner _stateMachineRunner;

    private void Awake()
    {
        _stateMachineRunner = GetComponent<StateMachineRunner>();
    }

    public void InitStateMachine()
    {
        panels.Add(MainPanelObj);
        panels.Add(LocalMainPanelObj);
        panels.Add(ConfirmClearProgressPanelObj);
        panels.Add(MsgProgressClearedPanelObj);
        panels.Add(PickLevelPanelObj);
    }
    public void HideAllPanel()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
    public IEnumerator GenerateLevelGrid()
    {
            Debug.Log($"Create button");

        for (int i = 0; i < levelGridRoot.transform.childCount; i++)
        {
            Destroy(levelGridRoot.transform.GetChild(i).gameObject);
        }
        yield return new WaitForEndOfFrame();
        GameData gameData = GameDataSystemManager.Instance.CurrentGameData;
        for (int i = 0; i < GameDataSystemManager.Instance.levelInfos.Count; i++)
        {
            Debug.Log($"Create button i ={i}");
            LevelInfo levelInfo = GameDataSystemManager.Instance.levelInfos[i];
            LevelButton levelButton = Instantiate(levelGridChildPrefab, levelGridRoot.transform).GetComponent<LevelButton>();
            if (i == 0) levelButton.Init(levelInfo, i, false);
            else levelButton.Init(levelInfo, i, !gameData.levelProgressList[i - 1].isCleared);
        }

        PickLevelPanelObj.SetActive(true);
    }

    public void SetState(TitleDisplayState state)
    {
        _stateMachineRunner.ChangeState((TitleDisplayBaseState)state);
    }
}

