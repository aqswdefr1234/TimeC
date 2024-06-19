using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListController : MonoBehaviour
{
    [SerializeField] private Transform activeObject; // Prefab with TMP_Text and Button
    [SerializeField] private Transform listPanel;
    [SerializeField] private Transform calendar;

    Transform alarmContent, challengeContent;
    string path;

    Transform detailPanel;
    TMP_Text detailText;
    Button deleteButton, closeButton;
    string targetPath; // To store the file path of the selected item

    public static Dictionary<string, AlarmData> alarmDict = new Dictionary<string, AlarmData>();
    public static Dictionary<string, ChallengeData> challengeDict = new Dictionary<string, ChallengeData>();

    AppNotificationManager notificator = new AppNotificationManager();

    void Start()
    {
        notificator.PermissionAndroid();
        path = Application.persistentDataPath;
        SetUI();
        UpdateList();

        RemoveExpiredNotifications();
    }

    void SetUI()
    {
        // ScrollView content
        alarmContent = listPanel.Find("CurrentAlarmView").GetChild(0).GetChild(0);
        challengeContent = listPanel.Find("CurrentChallengeView").GetChild(0).GetChild(0);
        //detail 패널 자식객체
        detailPanel = listPanel.Find("DetailPanel");
        detailText = detailPanel.Find("DetailText").GetComponent<TMP_Text>();
        deleteButton = detailPanel.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(DeleteFile);
        closeButton = detailPanel.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() => 
        {
            detailPanel.gameObject.SetActive(false);
        });
    }

    public void UpdateList()
    {
        string[] files = Directory.GetFiles(path, "*.json");
        //기존 Clear
        foreach (Transform child in alarmContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in challengeContent)
        {
            Destroy(child.gameObject);
        }

        //알람과 첼린지 파일 구분하여 해당 딕셔너리에 더함.
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            if (file.Contains("Alarm_"))
            {
                AlarmData alarmData = JsonUtility.FromJson<AlarmData>(json);
                alarmDict[file] = alarmData;
                CreateItem(alarmData.title, file, alarmContent);
            }
            else if (file.Contains("Challenge_"))
            {
                ChallengeData challengeData = JsonUtility.FromJson<ChallengeData>(json);
                challengeDict[file] = challengeData;
                CreateItem(challengeData.title, file, challengeContent);
            }
        }
        transform.GetComponent<DatePicker>().UpdateCalendar();
    }
    void CreateItem(string title, string filePath, Transform parent)
    {
        //ActiveList에 인스턴스 생성
        Transform item = Instantiate(activeObject, parent);
        TMP_Text textComponent = item.GetComponent<TMP_Text>();
        textComponent.text = title;

        Button button = item.GetComponentInChildren<Button>();
        button.onClick.AddListener(() => 
        {
            detailPanel.gameObject.SetActive(true);
            ShowDetails(filePath);
        });
    }

    void ShowDetails(string filePath)
    {
        //삭제 버튼 누를 시 삭제하기 위해서
        targetPath = filePath;

        if (filePath.Contains("Alarm_"))
        {
            AlarmData alarmData = alarmDict[filePath];
            detailText.text = 
                $"Title: {alarmData.title}\n" +
                $"Date: {alarmData.year}/{alarmData.month}/{alarmData.day}\n" +
                $"Time: {alarmData.hour}:{alarmData.minute}";
        }
        else if (filePath.Contains("Challenge_"))
        {
            ChallengeData challengeData = challengeDict[filePath];
            detailText.text = 
                $"Title: {challengeData.title}\n" +
                $"Start Date: {challengeData.startYear}/{challengeData.startMonth}/{challengeData.startDay}\n" +
                $"End Date: {challengeData.endYear}/{challengeData.endMonth}/{challengeData.endDay}\n" +
                $"Time: {challengeData.hour}:{challengeData.minute}\n" +
                $"Frequency: {challengeData.frequency}";
        }
    }
    //알림 시간 혹은 만료 시간 후로 부터 5분이상 지났으면 삭제
    void RemoveExpiredNotifications()
    {
        List<string> removePath = new List<string>();
        DateTime now = DateTime.Now;
        foreach (KeyValuePair<string, AlarmData> pair in alarmDict)
        {
            DateTime endDate = new DateTime(pair.Value.year, pair.Value.month, pair.Value.day, pair.Value.hour, pair.Value.minute, 0).AddMinutes(5);
            if (now > endDate)
            {
                removePath.Add(pair.Key);
            }
        }
        foreach (KeyValuePair<string, ChallengeData> pair in challengeDict)
        {
            DateTime endDate = new DateTime(pair.Value.endYear, pair.Value.endMonth, pair.Value.endDay, pair.Value.hour, pair.Value.minute, 0).AddMinutes(5);
            if (now > endDate)
            {
                removePath.Add(pair.Key);
            }
        }
        foreach(string path in removePath)
        {
            targetPath = path;
            DeleteFile();
        }
        transform.GetComponent<DatePicker>().UpdateCalendar();
    }
    void DeleteFile()
    {
        string fileName = Path.GetFileNameWithoutExtension(targetPath);

        //알림 삭제
        if (fileName.Contains("Alarm_"))
        {
            notificator.DeleteAlarm(fileName, alarmDict[targetPath].id);
        }
        else if (fileName.Contains("Challenge_"))
        {
            notificator.DeleteChallenge(fileName, challengeDict[targetPath].ids);
        }
        //파일삭제
        File.Delete(targetPath);

        //딕셔너리에서 제거
        if (fileName.Contains("Alarm_"))
        {
            alarmDict.Remove(targetPath);
        }
        else if (fileName.Contains("Challenge_"))
        {
            challengeDict.Remove(targetPath);
        }

        //초기화
        detailText.text = "";
        detailPanel.gameObject.SetActive(false);
        targetPath = null; // Reset target path
        UpdateList();//리스트 초기화
    }
}