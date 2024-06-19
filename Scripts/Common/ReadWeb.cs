using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ReadWeb : MonoBehaviour
{
    [SerializeField] GameObject updatePanel;
    const string version = "0.0.1";
    string storeUrl = "";
    void Start()
    {
        UpdatePanelSet();
        StartCoroutine(ReadVersion());
    }
    IEnumerator ReadVersion() 
    {
        string url = "https://ksjtimechallenger.netlify.app/version.txt";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // ��û�� �����ϴ�.
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Notification.notiList.Add("Network Error");
                Debug.LogError(www.error);
            }
            else
            {   //����꿡�� �ؽ�Ʈ ������ �����ϸ� �ڵ����� ���� �������. �׷��� ��������� �ǹٸ��� �񱳰���.
                string[] webData = www.downloadHandler.text.Replace("\n", "").Split(",");
                string webVersion = webData[0];
                storeUrl = webData[1];
                if (version == webVersion) yield break;
                updatePanel.SetActive(true);
            }
        }
    }
    void UpdatePanelSet()
    {
        UnityEngine.UI.Button yesBtn = updatePanel.transform.Find("YesButton").GetComponent<UnityEngine.UI.Button>();
        UnityEngine.UI.Button noBtn = updatePanel.transform.Find("NoButton").GetComponent<UnityEngine.UI.Button>();

        yesBtn.onClick.AddListener(() => 
        {
            updatePanel.SetActive(false);
            Application.OpenURL(storeUrl);
        });
        noBtn.onClick.AddListener(() =>
        {
            updatePanel.SetActive(false);
        });
    }
}
