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
            // 요청을 보냅니다.
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Notification.notiList.Add("Network Error");
                Debug.LogError(www.error);
            }
            else
            {   //깃허브에서 텍스트 파일을 수정하면 자동으로 줄이 띄워진다. 그래서 제거해줘야 옳바르게 비교가능.
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
