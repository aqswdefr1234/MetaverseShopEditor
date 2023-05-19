using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContentSize
{
    public static void ContentScaleChange(Transform content)
    {
        int uiCount = content.childCount;
        content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uiCount * 70f); //��ư�� ���� ���� 70�̴�.
    }
    public static void ClearChild(Transform trans)
    {
        foreach(Transform child in trans)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
