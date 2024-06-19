using UnityEngine.Android;
using Unity.Notifications.Android;
using System;
using System.Collections.Generic;

public class AppNotificationManager
{
    public void SetUpAlarm(string fileName, AlarmData alarmData)
    {
        InitializeNotificationChannel(fileName, "Alarm Notifications", "Notifications for specific date");

        // 알람 시간 설정
        DateTime alarmTime = new DateTime(alarmData.year, alarmData.month, alarmData.day, alarmData.hour, alarmData.minute, 0);
        var notification = new AndroidNotification()
        {
            Title = alarmData.title,
            Text = $"Alarm: {alarmData.title}",
            SmallIcon = "icon_2", // 리소스에 있는 작은 아이콘
            LargeIcon = "icon_3", // 리소스에 있는 큰 아이콘
            FireTime = alarmTime,
        };

        // 알람 설정
        int id = AndroidNotificationCenter.SendNotification(notification, fileName);
        alarmData.id = id;
    }
    public void SetupChallenge(string fileName, ChallengeData challengeData)
    {
        List<int> ids = new List<int>();

        // 알림 채널 설정
        InitializeNotificationChannel(fileName, "Challenge Notifications", "Notifications for weekly challenges");

        // 시작 기간과 종료 기간 설정
        DateTime startDate = new DateTime(challengeData.startYear, challengeData.startMonth, challengeData.startDay);
        DateTime endDate = new DateTime(challengeData.endYear, challengeData.endMonth, challengeData.endDay);

        // 주마다 작동되게 하려는 요일 파싱
        string[] daysOfWeekStr = challengeData.frequency.Split(',');
        List<DayOfWeek> daysOfWeek = new List<DayOfWeek>();

        foreach (string dayStr in daysOfWeekStr)
        {
            daysOfWeek.Add(StringToDayOfWeek(dayStr));
        }

        // 매주 지정된 요일에 알림 설정
        foreach (DayOfWeek dayOfWeek in daysOfWeek)
        {
            SetWeeklyNotifications(challengeData.title, "Challenge: " + challengeData.title, dayOfWeek,
                                   challengeData.hour, challengeData.minute, startDate, endDate, fileName, ids);
        }
        challengeData.ids = ids;
    }


    // 알림 채널 설정 메소드
    private void InitializeNotificationChannel(string id, string name, string description)
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = id,
            Name = name,
            Importance = Importance.High,
            Description = description,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // 매주 특정 요일에 알림 설정 메소드
    private void SetWeeklyNotifications(string title, string text, DayOfWeek dayOfWeek, int hour, int minute, DateTime startDate, DateTime endDate, string channelId, List<int> ids)
    {
        // 시작 날짜부터 종료 날짜까지 주 단위로 반복
        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek == dayOfWeek)
            {
                DateTime alarmTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                var notification = new AndroidNotification()
                {
                    Title = title,
                    Text = text,
                    SmallIcon = "icon_2", // 리소스에 있는 작은 아이콘
                    LargeIcon = "icon_3", // 리소스에 있는 큰 아이콘
                    FireTime = alarmTime,
                };

                // 알람 설정
                int id = AndroidNotificationCenter.SendNotification(notification, channelId);
                ids.Add(id);
            }
        }
    }
    public void DeleteAlarm(string channelId, int notificationId)
    {
        AndroidNotificationCenter.CancelScheduledNotification(notificationId);
        AndroidNotificationCenter.DeleteNotificationChannel(channelId);
    }
    public void DeleteChallenge(string channelId, List<int> notificationIds)
    {
        foreach(int id in notificationIds)
        {
            AndroidNotificationCenter.CancelScheduledNotification(id);
        }
        AndroidNotificationCenter.DeleteNotificationChannel(channelId);
    }
    DayOfWeek StringToDayOfWeek(string str)
    {
        if (str == "SUN") return DayOfWeek.Sunday;
        if (str == "MON") return DayOfWeek.Monday;
        if (str == "TUE") return DayOfWeek.Tuesday;
        if (str == "WED") return DayOfWeek.Wednesday;
        if (str == "THU") return DayOfWeek.Thursday;
        if (str == "FRI") return DayOfWeek.Friday;
        else return DayOfWeek.Saturday;
    }
    public void PermissionAndroid()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }
}
