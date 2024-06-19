using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.IO;

public class ChallengeController : MonoBehaviour
{
    [SerializeField] private Transform content;

    TMP_InputField titleInput;
    //Set Date
    TMP_Dropdown startYearDrop, startMonthDrop, startDayDrop;
    TMP_Dropdown endYearDrop, endMonthDrop, endDayDrop;
    TMP_Dropdown hourDrop, minuteDrop;

    Button saveBtn;
    AppNotificationManager notificator = new AppNotificationManager();

    AdMobForward ad;
    void Start()
    {
        SetAd();
        SetUI();
    }
    void SetAd()
    {
        ad = AdMobForward.Instance;
        ad.StartInitialize();
    }
    void SetUI()
    {
        Transform startDateDropDownPanel = content.Find("StartDateDropDownPanel");
        Transform endDateDropDownPanel = content.Find("EndDateDropDownPanel");
        Transform timeDropDownPanel = content.Find("TimeDropDownPanel");
        Transform customDaysPanel = content.Find("CustomDaysPanel");
        // Start Date Dropdowns
        startYearDrop = startDateDropDownPanel.Find("YearDropDown").GetComponent<TMP_Dropdown>();
        startMonthDrop = startDateDropDownPanel.Find("MonthDropDown").GetComponent<TMP_Dropdown>();
        startDayDrop = startDateDropDownPanel.Find("DayDropDown").GetComponent<TMP_Dropdown>();

        // End Date Dropdowns
        endYearDrop = endDateDropDownPanel.Find("YearDropDown").GetComponent<TMP_Dropdown>();
        endMonthDrop = endDateDropDownPanel.Find("MonthDropDown").GetComponent<TMP_Dropdown>();
        endDayDrop = endDateDropDownPanel.Find("DayDropDown").GetComponent<TMP_Dropdown>();

        // Time Dropdowns
        hourDrop = timeDropDownPanel.Find("HourDropDown").GetComponent<TMP_Dropdown>();
        minuteDrop = timeDropDownPanel.Find("MinuteDropDown").GetComponent<TMP_Dropdown>();

        //Set days buttons
        SetDays();

        // Title Input Field
        titleInput = content.Find("TitleInputField").GetComponent<TMP_InputField>();

        //Event
        //Start
        startYearDrop.onValueChanged.AddListener((int index) => PopulateMonthDropdown(startYearDrop, startMonthDrop));
        startMonthDrop.onValueChanged.AddListener((int index) => PopulateDayDropdown(startYearDrop, startMonthDrop, startDayDrop));
        //End
        endYearDrop.onValueChanged.AddListener((int index) => PopulateMonthDropdown(endYearDrop, endMonthDrop));
        endMonthDrop.onValueChanged.AddListener((int index) => PopulateDayDropdown(endYearDrop, endMonthDrop, endDayDrop));

        //Save
        saveBtn = content.Find("SaveButton").GetComponent<Button>();
        saveBtn.onClick.AddListener(SaveChallenge);

        // Initialize dropdown options
        InitializeDropdowns();

    }
    void SetDays()
    {
        Transform customDaysPanel = content.Find("CustomDaysPanel");
        List<Button> daysBtnList = new List<Button>();

        Color32 defaultColor = new Color32(54, 54, 54, 255);
        Color32 unselectedColor = new Color32(0, 0, 0, 0);
        //버튼 찾기
        foreach (Transform child in customDaysPanel)
        {
            daysBtnList.Add(child.GetComponent<Button>());
        }
        //버튼 이벤트
        foreach (Button dayBtn in daysBtnList)
        {
            Image btnImage = dayBtn.transform.GetComponent<Image>();
            dayBtn.onClick.AddListener(() =>
            {
                Debug.Log("click");
                if (btnImage.color == defaultColor)
                {
                    btnImage.color = unselectedColor;
                    Debug.Log("unselectedColor");
                    return;
                }
                if(btnImage.color == unselectedColor)
                {
                    btnImage.color = defaultColor;
                    Debug.Log("defaultColor");
                    return;
                }
            });
        }
    }
    void InitializeDropdowns()
    {
        PopulateYearDropdown(startYearDrop);
        PopulateMonthDropdown(startYearDrop, startMonthDrop);
        PopulateDayDropdown(startYearDrop, startMonthDrop, startDayDrop);

        PopulateYearDropdown(endYearDrop);
        PopulateMonthDropdown(startYearDrop, endMonthDrop);
        PopulateDayDropdown(startYearDrop, endMonthDrop, endDayDrop);

        PopulateHourDropdown();
        PopulateMinuteDropdown();
    }
    void PopulateYearDropdown(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();
        int currentYear = DateTime.Now.Year;
        dropdown.AddOptions(new List<string> { currentYear.ToString(), (currentYear + 1).ToString() });
        dropdown.value = 0; // Set to the current year by default
    }

    void PopulateMonthDropdown(TMP_Dropdown yearDrop, TMP_Dropdown monthDrop)
    {
        monthDrop.ClearOptions();
        int currentMonth = DateTime.Now.Month;
        List<string> months = new List<string>();

        // If the selected year is the current year, start from the current month
        if (yearDrop.options[yearDrop.value].text == DateTime.Now.Year.ToString())
        {
            for (int i = currentMonth; i <= 12; i++)
            {
                months.Add(i.ToString());
            }
        }
        else // If the selected year is the next year, start from January
        {
            for (int i = 1; i <= 12; i++)
            {
                months.Add(i.ToString());
            }
        }

        monthDrop.AddOptions(months);
        monthDrop.value = 0; // Set to the first month available by default
    }

    void PopulateDayDropdown(TMP_Dropdown yearDrop, TMP_Dropdown monthDrop, TMP_Dropdown dayDrop)
    {
        dayDrop.ClearOptions();
        int selectedYear = int.Parse(yearDrop.options[yearDrop.value].text);
        int selectedMonth = int.Parse(monthDrop.options[monthDrop.value].text);
        int currentDay = DateTime.Now.Day;
        List<string> days = new List<string>();

        int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);

        // If the selected year and month are the current year and month, start from the current day
        if (selectedYear == DateTime.Now.Year && selectedMonth == DateTime.Now.Month)
        {
            for (int i = currentDay; i <= daysInMonth; i++)
            {
                days.Add(i.ToString());
            }
        }
        else // If the selected year and month are not the current year and month, start from the 1st
        {
            for (int i = 1; i <= daysInMonth; i++)
            {
                days.Add(i.ToString());
            }
        }

        dayDrop.AddOptions(days);
        dayDrop.value = 0; // Set to the first day available by default

        //End Date라면 + 7
        if(dayDrop.transform.parent.name == "EndDateDropDownPanel")
        {
            dayDrop.value = 7;
        }
    }
    void PopulateHourDropdown()
    {
        hourDrop.ClearOptions();
        List<string> hours = new List<string>();

        for (int i = 0; i < 24; i++)
        {
            hours.Add(i.ToString("D2"));
        }

        hourDrop.AddOptions(hours);
        hourDrop.value = DateTime.Now.Hour + 1; // Set to the current hour by default
    }

    void PopulateMinuteDropdown()
    {
        minuteDrop.ClearOptions();
        List<string> minutes = new List<string>();

        for (int i = 0; i < 60; i++)
        {
            minutes.Add(i.ToString("D2"));
        }

        minuteDrop.AddOptions(minutes);
        minuteDrop.value = DateTime.Now.Minute; // Set to the current minute + 5 by default
    }
    void SaveChallenge()
    {
        //조건따져서 하나라도 해당되지 않으면 리턴
        if (!IsSaveable()) 
        {
            Notification.notiList.Add("Failed to save!");
            return;
        }
        // Create an object to hold the challenge data
        ChallengeData challengeData = new ChallengeData
        {
            title = titleInput.text,
            startYear = int.Parse(startYearDrop.options[startYearDrop.value].text),
            startMonth = int.Parse(startMonthDrop.options[startMonthDrop.value].text),
            startDay = int.Parse(startDayDrop.options[startDayDrop.value].text),
            endYear = int.Parse(endYearDrop.options[endYearDrop.value].text),
            endMonth = int.Parse(endMonthDrop.options[endMonthDrop.value].text),
            endDay = int.Parse(endDayDrop.options[endDayDrop.value].text),
            hour = int.Parse(hourDrop.options[hourDrop.value].text),
            minute = int.Parse(minuteDrop.options[minuteDrop.value].text),
            frequency = ReturnDays()
        };
        // Generate a unique filename based on the challenge title and current date/time
        string filename = $"Challenge_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string path = Path.Combine(Application.persistentDataPath, filename);

        //알림메시지 예약 하고 알림 id 데이터 입력
        notificator.SetupChallenge(filename, challengeData);

        string json = JsonUtility.ToJson(challengeData);
        File.WriteAllText(path, json);

        Notification.notiList.Add("Save success!");
        Debug.Log("Challenge saved to: " + path);

        //활성화 리스트 항목을 업데이트 해준다.
        transform.GetComponent<ListController>().UpdateList();

        ad.ShowInterstitialAd();
    }
    bool IsSaveable()
    {
        // Check if title is not empty
        if (string.IsNullOrWhiteSpace(titleInput.text))
        {
            Notification.notiList.Add("Title cannot be empty");
            return false;
        }

        // Get current date and time
        DateTime currentTime = DateTime.Now;

        // Create start and end DateTime objects
        DateTime startDateTime = new DateTime(
            int.Parse(startYearDrop.options[startYearDrop.value].text),
            int.Parse(startMonthDrop.options[startMonthDrop.value].text),
            int.Parse(startDayDrop.options[startDayDrop.value].text),
            int.Parse(hourDrop.options[hourDrop.value].text),
            int.Parse(minuteDrop.options[minuteDrop.value].text),
            0
        );

        DateTime endDateTime = new DateTime(
            int.Parse(endYearDrop.options[endYearDrop.value].text),
            int.Parse(endMonthDrop.options[endMonthDrop.value].text),
            int.Parse(endDayDrop.options[endDayDrop.value].text),
            int.Parse(hourDrop.options[hourDrop.value].text),
            int.Parse(minuteDrop.options[minuteDrop.value].text),
            0
        );

        // Check if start date is after the current time
        if (startDateTime <= currentTime)
        {
            Notification.notiList.Add("Start date must be later than now");
            return false;
        }

        // Check if end date is after the start date
        if (endDateTime <= startDateTime)
        {
            Notification.notiList.Add("End date must be later than start date");
            return false;
        }
        return true;
    }
    string ReturnDays()
    {
        Transform customDaysPanel = content.Find("CustomDaysPanel");
        List<Button> daysBtnList = new List<Button>();

        //버튼 찾기
        foreach (Transform child in customDaysPanel)
        {
            daysBtnList.Add(child.GetComponent<Button>());
        }
        List<string> daysList = new List<string>();
        foreach (Button dayBtn in daysBtnList)
        {
            Image btnImage = dayBtn.transform.GetComponent<Image>();
            if (btnImage.color == new Color32(54, 54, 54, 255))
            {
                TMP_Text dayText = dayBtn.transform.GetChild(0).GetComponent<TMP_Text>();
                daysList.Add(dayText.text);
            }
        }
        //ex, "MON,TUE,FRI"
        return string.Join(",", daysList);
    }
}

[Serializable]
public class ChallengeData
{
    public string title;
    public int startYear;
    public int startMonth;
    public int startDay;
    public int endYear;
    public int endMonth;
    public int endDay;
    public int hour;
    public int minute;
    public string frequency;
    public List<int> ids = new List<int>();
}