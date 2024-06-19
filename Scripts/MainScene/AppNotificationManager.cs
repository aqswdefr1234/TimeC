using UnityEngine.Android;
using Unity.Notifications.Android;
using System;
using System.Collections.Generic;

public class AppNotificationManager
{
    public void SetUpAlarm(string fileName, AlarmData alarmData)
    {
        InitializeNotificationChannel(fileName, "Alarm Notifications", "Notifications for specific date");

        // �˶� �ð� ����
        DateTime alarmTime = new DateTime(alarmData.year, alarmData.month, alarmData.day, alarmData.hour, alarmData.minute, 0);
        var notification = new AndroidNotification()
        {
            Title = alarmData.title,
            Text = $"Alarm: {alarmData.title}",
            SmallIcon = "icon_2", // ���ҽ��� �ִ� ���� ������
            LargeIcon = "icon_3", // ���ҽ��� �ִ� ū ������
            FireTime = alarmTime,
        };

        // �˶� ����
        int id = AndroidNotificationCenter.SendNotification(notification, fileName);
        alarmData.id = id;
    }
    public void SetupChallenge(string fileName, ChallengeData challengeData)
    {
        List<int> ids = new List<int>();

        // �˸� ä�� ����
        InitializeNotificationChannel(fileName, "Challenge Notifications", "Notifications for weekly challenges");

        // ���� �Ⱓ�� ���� �Ⱓ ����
        DateTime startDate = new DateTime(challengeData.startYear, challengeData.startMonth, challengeData.startDay);
        DateTime endDate = new DateTime(challengeData.endYear, challengeData.endMonth, challengeData.endDay);

        // �ָ��� �۵��ǰ� �Ϸ��� ���� �Ľ�
        string[] daysOfWeekStr = challengeData.frequency.Split(',');
        List<DayOfWeek> daysOfWeek = new List<DayOfWeek>();

        foreach (string dayStr in daysOfWeekStr)
        {
            daysOfWeek.Add(StringToDayOfWeek(dayStr));
        }

        // ���� ������ ���Ͽ� �˸� ����
        foreach (DayOfWeek dayOfWeek in daysOfWeek)
        {
            SetWeeklyNotifications(challengeData.title, "Challenge: " + challengeData.title, dayOfWeek,
                                   challengeData.hour, challengeData.minute, startDate, endDate, fileName, ids);
        }
        challengeData.ids = ids;
    }


    // �˸� ä�� ���� �޼ҵ�
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

    // ���� Ư�� ���Ͽ� �˸� ���� �޼ҵ�
    private void SetWeeklyNotifications(string title, string text, DayOfWeek dayOfWeek, int hour, int minute, DateTime startDate, DateTime endDate, string channelId, List<int> ids)
    {
        // ���� ��¥���� ���� ��¥���� �� ������ �ݺ�
        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek == dayOfWeek)
            {
                DateTime alarmTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                var notification = new AndroidNotification()
                {
                    Title = title,
                    Text = text,
                    SmallIcon = "icon_2", // ���ҽ��� �ִ� ���� ������
                    LargeIcon = "icon_3", // ���ҽ��� �ִ� ū ������
                    FireTime = alarmTime,
                };

                // �˶� ����
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
