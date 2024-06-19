using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Notification : MonoBehaviour
{
    public static List<string> notiList = new List<string>();
    GameObject notifyText, canvasObject, panelObject;
    
    void Start()
    {
        SetNotify();
        DontDestroyOnLoad(notifyText);
        DontDestroyOnLoad(canvasObject);
    }
    void SetNotify()
    {
        CreateCanvas();
        CreatePanel();
        CreateText();
        StartCoroutine(Detect());
    }
    void CreateCanvas()
    {
        canvasObject = new GameObject("NotificationCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //canvas.sortingOrder = 10;
        canvasObject.AddComponent<GraphicRaycaster>();

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080, 1920);

        
        DontDestroyOnLoad(canvasObject);
    }
    void CreatePanel()
    {
        if (canvasObject == null) return;

        panelObject = new GameObject("Panel");
        RectTransform panelRectTransform = panelObject.AddComponent<RectTransform>();
        panelRectTransform.SetParent(canvasObject.transform, false);

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0);

        VerticalLayoutGroup verticalLayoutGroup = panelObject.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.spacing = 5f;

        ContentSizeFitter contentSizeFitter = panelObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        UIPositionController.ReLoadOutsideUI(panelObject.transform, 0, 70, 90, 0);
    }

    IEnumerator Detect()
    {
        while (true)
        {
            if (notiList.Count > 0)
            {
                foreach (string noti in notiList) Notify(noti);
                notiList.Clear();
            }
            yield return new WaitForSeconds(0.5f);
        }

    }
    void CreateText()
    {
        notifyText = new GameObject("NotifyText");
        notifyText.AddComponent<TextMeshProUGUI>().fontSize = (int)System.Math.Ceiling(Screen.height / 20f);
        notifyText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        notifyText.GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 30f);
    }
    void Notify(string notice)
    {
        GameObject clone = Instantiate(notifyText, panelObject.transform);
        clone.GetComponent<TextMeshProUGUI>().text = notice;
        clone.AddComponent<Disappearance>();
    }
}