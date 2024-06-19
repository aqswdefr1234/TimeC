using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using System.IO;

public class AlarmController : MonoBehaviour
{
    [SerializeField] private Transform alarmSettingPanel;
    TMP_Dropdown yearDrop, monthDrop, dayDrop, hourDrop, minuteDrop;
    TMP_InputField titleInput;

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
        //Top Transform Find
        titleInput = alarmSettingPanel.Find("TitleInputField").GetComponent<TMP_InputField>();
        UnityEngine.UI.Button saveBtn = alarmSettingPanel.Find("SaveButton").GetComponent<UnityEngine.UI.Button>();
        Transform dateDropDownPanel = alarmSettingPanel.Find("DateDropDownPanel");
        Transform timeDropDownPanel = alarmSettingPanel.Find("TimeDropDownPanel");
        // Date
        yearDrop = dateDropDownPanel.Find("YearDropDown").GetComponent<TMP_Dropdown>();
        monthDrop = dateDropDownPanel.Find("MonthDropDown").GetComponent<TMP_Dropdown>();
        dayDrop = dateDropDownPanel.Find("DayDropDown").GetComponent<TMP_Dropdown>();
        // Time
        hourDrop = timeDropDownPanel.Find("HourDropDown").GetComponent<TMP_Dropdown>();
        minuteDrop = timeDropDownPanel.Find("MinuteDropDown").GetComponent<TMP_Dropdown>();
        //Event
        yearDrop.onValueChanged.AddListener((int index) => PopulateMonthDropdown());
        monthDrop.onValueChanged.AddListener((int index) => PopulateDayDropdown());
        saveBtn.onClick.AddListener(SaveAlarm);

        PopulateYearDropdown();
        PopulateMonthDropdown();
        PopulateDayDropdown();
        PopulateHourDropdown();
        PopulateMinuteDropdown();
    }

    void PopulateYearDropdown()
    {
        yearDrop.ClearOptions();
        int currentYear = DateTime.Now.Year;
        yearDrop.AddOptions(new List<string> { currentYear.ToString(), (currentYear + 1).ToString() });
        yearDrop.value = 0; // Set to the current year by default
    }

    void PopulateMonthDropdown()
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
        PopulateDayDropdown(); // Populate days based on the first month available
    }

    void PopulateDayDropdown()
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
    void SaveAlarm()
    {
        string titleText = titleInput.text;
        int selectedYear = int.Parse(yearDrop.options[yearDrop.value].text);
        int selectedMonth = int.Parse(monthDrop.options[monthDrop.value].text);
        int selectedDay = int.Parse(dayDrop.options[dayDrop.value].text);
        int selectedHour = int.Parse(hourDrop.options[hourDrop.value].text);
        int selectedMinute = int.Parse(minuteDrop.options[minuteDrop.value].text);

        DateTime selectedDateTime = new DateTime(selectedYear, selectedMonth, selectedDay, selectedHour, selectedMinute, 0);
        DateTime currentDateTime = DateTime.Now;

        if(titleText == "")
        {
            Notification.notiList.Add("The title is empty.");
            return;
        }
        // Check if the selected date and time is at least 2 minutes in the future
        if (selectedDateTime > currentDateTime.AddMinutes(2))
        {
            // Create an object to hold the alarm data
            AlarmData alarmData = new AlarmData
            {
                title = titleText,
                year = selectedYear,
                month = selectedMonth,
                day = selectedDay,
                hour = selectedHour,
                minute = selectedMinute
            };
            // Generate a unique filename based on the selected date and time
            string filename = $"Alarm_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string path = Path.Combine(Application.persistentDataPath, filename);
            notificator.SetUpAlarm(filename, alarmData);

            string json = JsonUtility.ToJson(alarmData);
            File.WriteAllText(path, json);
            Notification.notiList.Add("Save!");
            //활성화 리스트 항목을 업데이트 해준다.
            transform.GetComponent<ListController>().UpdateList();

            ad.ShowInterstitialAd();
        }
        else
        {
            Notification.notiList.Add("Selected date and time must be at least 2 minutes in the future.");
        }
    }
}

[Serializable]
public class AlarmData
{
    public string title;
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
    public int id;
}