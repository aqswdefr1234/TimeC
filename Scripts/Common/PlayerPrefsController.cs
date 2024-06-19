using System;
using UnityEngine;

public class PlayerPrefsController
{
    public static void SaveMidnightData()
    {
        DateTime now = DateTime.Now;
        DateTime nextMidnight = now.Date.AddDays(1);

        //���� �ð��� ����
        PlayerPrefs.SetString("NextMidnight", nextMidnight.ToString());
        PlayerPrefs.Save();
    }
    public static bool LoadAfterMidnight()
    {
        //�ѹ��� �����Ͱ� ������� �ʾҴ� ��쿡�� �׻� �۵��ǰ� �ؾ��Ѵ�.
        if (!PlayerPrefs.HasKey("NextMidnight")) 
        {
            DeleteDailyData();
            return true;
        }
        

        string nextMidnightString = PlayerPrefs.GetString("NextMidnight");
        DateTime nextMidnight = DateTime.Parse(nextMidnightString);
        DateTime now = DateTime.Now;

        //������ �����ٸ�
        if (now >= nextMidnight)
        {
            DeleteDailyData();
            return true;
        }
        //�� �����ٸ�
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
    //�Ϸ翡 �ѹ��� ���� �ε��ϱ� ����. ���� �̹� �ôٸ� 1 ����. �ƴϸ� 0
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
