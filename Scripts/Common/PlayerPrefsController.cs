using System;
using UnityEngine;

public class PlayerPrefsController
{
    public static void SaveMidnightData()
    {
        DateTime now = DateTime.Now;
        DateTime nextMidnight = now.Date.AddDays(1);

        //자정 시간을 저장
        PlayerPrefs.SetString("NextMidnight", nextMidnight.ToString());
        PlayerPrefs.Save();
    }
    public static bool LoadAfterMidnight()
    {
        //한번도 데이터가 저장되지 않았던 경우에는 항상 작동되게 해야한다.
        if (!PlayerPrefs.HasKey("NextMidnight")) 
        {
            DeleteDailyData();
            return true;
        }
        

        string nextMidnightString = PlayerPrefs.GetString("NextMidnight");
        DateTime nextMidnight = DateTime.Parse(nextMidnightString);
        DateTime now = DateTime.Now;

        //자정이 지났다면
        if (now >= nextMidnight)
        {
            DeleteDailyData();
            return true;
        }
        //안 지났다면
        else
        {
            return false;
        }
    }
    public static void SaveScore(int score)
    {
        PlayerPrefs.SetInt("DailyScore", score);
        PlayerPrefs.Save();
    }
    public static void SaveRemoveVal(int val)
    {
        PlayerPrefs.SetInt("RemoveVal", val);
        PlayerPrefs.Save();
    }
    public static void SaveAdState(string str)
    {
        PlayerPrefs.SetString("AdForward", str);
        PlayerPrefs.Save();
    }
    public static int LoadDailyScore()
    {
        if (!PlayerPrefs.HasKey("NextMidnight")) return 9999;
        int score = PlayerPrefs.GetInt("DailyScore");
        return score;
    }
    public static int LoadRemoveVal()
    {
        if (!PlayerPrefs.HasKey("RemoveVal")) return 9999;
        int val = PlayerPrefs.GetInt("RemoveVal");
        return val;
    }
    //하루에 한번만 광고 로드하기 위함. 광고를 이미 봤다면 1 리턴. 아니면 0
    public static string LoadAdForwardState()
    {
        if (!PlayerPrefs.HasKey("AdForward")) return "Yet";
        string state = PlayerPrefs.GetString("AdForward");
        return state;
    }
    public static void DeleteDailyData()
    {
        PlayerPrefs.DeleteKey("NextMidnight");
        PlayerPrefs.DeleteKey("DailyScore");
        PlayerPrefs.DeleteKey("RemoveVal");
        PlayerPrefs.DeleteKey("AdForward");
    }
}
