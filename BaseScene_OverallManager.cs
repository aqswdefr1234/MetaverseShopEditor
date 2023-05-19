using UnityEngine;
using System;

public class BaseScene_OverallManager : MonoBehaviour
{
    public static event Action<Transform> OnSelectTransformChanged;
    public static event Action<Vector3> OnPositionChanged;

    public static Transform objCreationSpace;//������Ʈ ��������

    public static Transform selectedTransform;
    public static Transform selectedObjectTransform
    {
        get
        {
            Debug.Log("get");
            if (selectedTransform != null)
                return selectedTransform;
            else
                return null;
        }
        set//�� �Ҵ�� ����.get�� ���� ������ ���� ���� ����.
        {
            Debug.Log("set");
            if(value != null)
            {
                selectedTransform = value;
                OnSelectTransformChanged?.Invoke(value);//get�� ������ ���� ���� �� ����.
                Debug.Log("OnSelectTransformChanged");
            }
            else
                selectedTransform = value;
        }
    }
    public static Vector3 objectPosition
    {
        set
        {
            OnPositionChanged?.Invoke(value);
            Debug.Log("OnPositionChanged");
        }
    }
    public static void BaseScene_ClearEvents()
    {
        OnSelectTransformChanged = null;
        OnPositionChanged = null;
    }
}
