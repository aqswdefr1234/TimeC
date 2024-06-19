using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPositionController : MonoBehaviour
{
    [Header("Unit X : Screen.width / 100, Y : Screen.height / 100")]
    [SerializeField] private UIPosition[] uiArr;
    [Header("GridLayoutGroup")]
    [SerializeField] private GridLayoutGroup[] gridArr;
    [Header("VerticalLayoutGroup")]
    [SerializeField] private VerticalLayoutGroup[] verArr;
    void Start()
    {
        Invoke("ReLoadUI", 0.1f);
    }
    void ReLoadUI()
    {
        float widthUnit = Screen.width / 100f;
        float heightUnit = Screen.height / 100f;

        foreach (UIPosition uiPosition in uiArr)
        {
            ReScaleUI(uiPosition);
            RePositionUI(widthUnit, heightUnit, uiPosition);
        }
        UpdateGridCell();
        UpdateVerticalChild();
    }
    void RePositionUI(float widthUnit, float heightUnit, UIPosition ui)
    {
        ui.target.position = new Vector3(ui.posXInt * widthUnit, ui.posYInt * heightUnit, 0);
    }
    void ReScaleUI(UIPosition ui)
    {
        RectTransform rectTransform = ui.target.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            // �θ� ĵ���� ũ�⿡ ���� ������ ����
            float anchorMinX = (float)ui.posXInt / 100f;
            float anchorMinY = (float)ui.posYInt / 100f;
            float anchorMaxX = anchorMinX + (float)ui.scaleXInt / 100f;
            float anchorMaxY = anchorMinY + (float)ui.scaleYInt / 100f;

            rectTransform.anchorMin = new Vector2(anchorMinX, anchorMinY);
            rectTransform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
    void UpdateGridCell()
    {
        foreach(GridLayoutGroup gridGroup in gridArr)
        {
            int childCount = gridGroup.transform.childCount;
            RectTransform rectTrans = gridGroup.transform.GetComponent<RectTransform>();
            //���� ����
            if(gridGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                int count = gridGroup.constraintCount;
                int oppositeCount = (int)Math.Ceiling((float)childCount / count);
                int cellWidth = (int)Math.Floor(rectTrans.rect.width / count);
                int cellHeight = (int)Math.Floor(rectTrans.rect.height / oppositeCount);
                gridGroup.cellSize = new Vector2 (cellWidth, cellHeight);
            }
            //���α���
            else if (gridGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                int count = gridGroup.constraintCount;
                int oppositeCount = (int)Math.Ceiling((float)childCount / count);
                int cellHeight = (int)Math.Floor(rectTrans.rect.height / count);
                int cellWidth = (int)Math.Floor(rectTrans.rect.width / oppositeCount);
                gridGroup.cellSize = new Vector2(cellWidth, cellHeight);
            }
            else continue;
        }
    }
    void UpdateVerticalChild()
    {
        foreach (VerticalLayoutGroup verGroup in verArr)
        {
            RectTransform rectTrans = verGroup.transform.GetComponent<RectTransform>();
            //�ʺ�����
            foreach(Transform child in verGroup.transform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect == null) continue;
                //�θ� �ʺ񺸴� �дٸ� �θ� �ʺ� �����.
                if (childRect.rect.width > rectTrans.rect.width)
                {
                    childRect.sizeDelta = new Vector2(rectTrans.rect.width, childRect.rect.height);
                }
            }
        }
    }
    public static void ReLoadOutsideUI(Transform target, int posX, int posY, int scaleX, int scaleY)
    {
        float widthUnit = Screen.width / 100f;
        float heightUnit = Screen.height / 100f;
        //������
        target.position = new Vector3(posX * widthUnit, posY * heightUnit, 0);
        //������
        RectTransform rectTransform = target.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            // �θ� ĵ���� ũ�⿡ ���� ������ ����
            float anchorMinX = (float)posX / 100f;
            float anchorMinY = (float)posY / 100f;
            float anchorMaxX = anchorMinX + (float)scaleX / 100f;
            float anchorMaxY = anchorMinY + (float)scaleY / 100f;

            rectTransform.anchorMin = new Vector2(anchorMinX, anchorMinY);
            rectTransform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
[System.Serializable]
public class UIPosition
{
    public Transform target;
    public int posXInt;
    public int posYInt;
    public int scaleXInt;
    public int scaleYInt;
}