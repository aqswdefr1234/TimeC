using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    Transform bottomButtonPanel;

    Button calenderBtn, currentListBtn, alarmBtn, challengeBtn;
    Transform calenderPanel, currentListPanel, alarmPanel, challengePanel;
    List<Transform> panelList = new List<Transform>();
    void Start()
    {
        SetPanel();
        SetUI();
    }
    void SetUI() 
    {
        calenderBtn = bottomButtonPanel.Find("CalenderButton").GetComponent<Button>();
        currentListBtn = bottomButtonPanel.Find("CurrentListButton").GetComponent<Button>();
        alarmBtn = bottomButtonPanel.Find("AlarmSetButton").GetComponent<Button>();
        challengeBtn = bottomButtonPanel.Find("ChallengeSetButton").GetComponent<Button>();

        calenderBtn.onClick.AddListener(() => ActivatePanel(calenderPanel));
        currentListBtn.onClick.AddListener(() => ActivatePanel(currentListPanel));
        alarmBtn.onClick.AddListener(() => ActivatePanel(alarmPanel));
        challengeBtn.onClick.AddListener(() => ActivatePanel(challengePanel));
    }
    void SetPanel()
    {
        Transform canvas = FindScene.FindSceneRoot("Canvas");
        bottomButtonPanel = canvas.Find("BottomButtonPanel");

        calenderPanel = canvas.Find("CalenderPanel"); panelList.Add(calenderPanel);
        currentListPanel = canvas.Find("CurrentListPanel"); panelList.Add(currentListPanel);
        alarmPanel = canvas.Find("AlarmSettingPanel"); panelList.Add(alarmPanel);
        challengePanel = canvas.Find("ChallengeSettingPanel"); panelList.Add(challengePanel);
    }
    void ActivatePanel(Transform panel)
    {
        foreach(Transform child in panelList)
        {
            if(panel == child) child.gameObject.SetActive(true);
            else child.gameObject.SetActive(false);
        }
    }
}
