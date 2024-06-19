using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Globalization;
public class DatePicker : MonoBehaviour
{
    [SerializeField] private Transform calenderPanel;
    [SerializeField] private GameObject dayButtonPrefab;
    [SerializeField] private GameObject listViewItemPrefab;

    Transform calender, listViewContent;

    TMP_Text monthText, yearText;
    Button previousMonthButton, nextMonthButton;

    DateTime todayDate, currentDate;
    List<Button> dayButtons = new List<Button>();
    Button todayButton = null;

    //알람, 첼린지 이미지를 초기화 시켜주기 위해서
    List<Transform> activatedList = new List<Transform>();

    void Start()
    {
        SetUI();

        todayDate = DateTime.Now;
        currentDate = DateTime.Now;

        previousMonthButton.onClick.AddListener(ShowPreviousMonth);
        nextMonthButton.onClick.AddListener(ShowNextMonth);

        GenerateDayButtons();
        UpdateCalendar();
    }
    void SetUI()
    {
        //Calendar
        calender = calenderPanel.Find("Calender");
        //MonthPanel
        Transform monthPanel = calenderPanel.Find("MonthPanel");
        monthText = monthPanel.Find("MonthText").GetComponent<TMP_Text>();
        yearText = monthPanel.Find("YearText").GetComponent<TMP_Text>();
        previousMonthButton = monthPanel.Find("PreButton").GetComponent<Button>();
        nextMonthButton = monthPanel.Find("NextButton").GetComponent<Button>();
        //ListView
        listViewContent = calenderPanel.Find("ListView").GetChild(0).GetChild(0);

    }
    void GenerateDayButtons()
    {
        for (int i = 0; i < 42; i++) // 6 weeks * 7 days = 42 days
        {
            GameObject dayButtonObject = Instantiate(dayButtonPrefab, calender);
            Button dayButton = dayButtonObject.GetComponent<Button>();
            dayButtons.Add(dayButton);
        }
    }

    public void UpdateCalendar()
    {
        //달력 초기화
        ReturnButtonColor();
        Initialize();

        monthText.text = currentDate.Month.ToString();
        yearText.text = currentDate.Year.ToString();

        DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

        //일요일 : 0, 토요일 : 6
        int dayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        //월 시작 날짜의 요일을 기준으로 달력을 생성한다.
        DateTime startDate = firstDayOfMonth.AddDays(-dayOfWeek);
        for (int i = 0; i < dayButtons.Count; i++)
        {
            DateTime day = startDate.AddDays(i);
            TMP_Text dayText = dayButtons[i].GetComponentInChildren<TMP_Text>();
            dayText.text = day.Day.ToString();

            //Transform의 name 바꿔주기. 알람과 첼린지를 표시하는데 있어서 필요
            string dayName = $"{day:yyyyMMdd}{day.DayOfWeek.ToString().Substring(0, 3).ToUpper()}";
            dayButtons[i].name = dayName;

            if (day.Month == currentDate.Month)
            {
                dayButtons[i].interactable = true;
                dayText.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                dayButtons[i].interactable = false;
                dayText.color = new Color32(100, 100, 100, 100);
            }
            //오늘 날짜라면
            if(day.Date == todayDate.Date)
            {
                ChangeTodayColor(dayButtons[i]);
                todayButton = dayButtons[i];
            }
        }
        UpdateListInCalendar();
    }

    void ShowPreviousMonth()
    {
        currentDate = currentDate.AddMonths(-1);
        UpdateCalendar();
    }

    void ShowNextMonth()
    {
        currentDate = currentDate.AddMonths(1);
        UpdateCalendar();
    }
    void ChangeTodayColor(Button btn)
    {
        btn.transform.GetComponent<Image>().color = Color.yellow;
    }
    void ReturnButtonColor()
    {
        //오늘 날짜 버튼에 색깔이 바뀌어 있는데 월이 바뀌면 다시 원래 색상으로 돌리기 위해
        if (todayButton != null)
        {
            todayButton.transform.GetComponent<Image>().color = new Color32(54, 54, 54, 255);
            todayButton = null;
        }
    }
    void UpdateListInCalendar()
    {
        if (ListController.alarmDict.Count > 0)
        {
            foreach(KeyValuePair<string, AlarmData> pair in ListController.alarmDict)
            {
                if (pair.Value.year == currentDate.Year && pair.Value.month == currentDate.Month)
                {
                    string date = $"{pair.Value.year}{pair.Value.month:00}{pair.Value.day:00}";
                    string dayOfWeek = new DateTime(pair.Value.year, pair.Value.month, pair.Value.day).DayOfWeek.ToString().Substring(0, 3).ToUpper();

                    Transform dayTrans = calender.Find(date + dayOfWeek);
                    Transform alarmImage = dayTrans.Find("AlarmImage");
                    alarmImage.gameObject.SetActive(true);
                    activatedList.Add(alarmImage);

                    //해당버튼 클릭시 이벤트
                    dayTrans.GetComponent<Button>().onClick.AddListener(() => 
                    {
                        GameObject viewItem = Instantiate(listViewItemPrefab, listViewContent);
                        viewItem.GetComponent<TMP_Text>().text = $"<color=#FFB6C1>{pair.Value.title}</color> / {pair.Value.hour} : {pair.Value.minute}";
                    });
                }
            }
            
        }
        if (ListController.challengeDict.Count > 0)
        {
            foreach (KeyValuePair<string, ChallengeData> pair in ListController.challengeDict)
            {
                //기간 범위에 해당하는지 확인
                int currentMonthCount = currentDate.Year * 12 + currentDate.Month;
                int startMonthCount = pair.Value.startYear * 12 + pair.Value.startMonth;
                int endMonthCount = pair.Value.endYear * 12 + pair.Value.endMonth;
                //범위에 있지 않으면
                if (startMonthCount > currentMonthCount || currentMonthCount > endMonthCount)
                {
                    continue;
                }

                //시작날짜 끝 날짜 데이터 변환
                DateTime startDate = new DateTime(pair.Value.startYear, pair.Value.startMonth, pair.Value.startDay);
                DateTime endDate = new DateTime(pair.Value.endYear, pair.Value.endMonth, pair.Value.endDay);
                string[] frequencyDays = pair.Value.frequency.Split(',');

                //캘린더 자식객체의 이름을 전부 대조해 스타트 기간과 엔드기간 사이에 있고 해당 요일에 맞는 객체의 "ChallengeImage"를 활성화
                foreach (Transform child in calender)
                {
                    string childName = child.name;

                    int year = int.Parse(childName.Substring(0, 4));
                    int month = int.Parse(childName.Substring(4, 2));
                    int day = int.Parse(childName.Substring(6, 2));

                    DateTime date = new DateTime(year, month, day);

                    if (date >= startDate && date <= endDate)
                    {
                        string dayOfWeek = childName.Substring(8, 3);

                        if (Array.IndexOf(frequencyDays, dayOfWeek) != -1)
                        {
                            Transform challengeImage = child.Find("ChallengeImage");
                            challengeImage.gameObject.SetActive(true);
                            activatedList.Add(challengeImage);
                            //해당버튼 클릭시 이벤트
                            child.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                GameObject listBtn = Instantiate(listViewItemPrefab, listViewContent);
                                listBtn.GetComponent<TMP_Text>().text = $"<color=#ADD8E6>{pair.Value.title}</color> / {pair.Value.hour}:{pair.Value.minute}";
                            });
                        }
                    }
                }
            }
        }
    }
    void Initialize() 
    {
        ClearListView();
        foreach (Transform target in activatedList)
        {
            target.gameObject.SetActive(false);
        }
        
        foreach (Transform child in calender)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.GetComponent<Button>().onClick.AddListener(() => 
            {
                ClearListView();
                //클릭한 버튼 며칠인지
                string th = child.GetChild(0).GetComponent<TMP_Text>().text;
                GameObject viewItem = Instantiate(listViewItemPrefab, listViewContent);
                viewItem.GetComponent<TMP_Text>().text = $"<color=#FFFF00>{th}th</color>";
                viewItem.transform.SetSiblingIndex(0);
            });
        }
        activatedList.Clear();
    }
    void ClearListView()
    {
        foreach (Transform target in listViewContent)
        {
            Destroy(target.gameObject);
        }
    }
}