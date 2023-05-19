using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OBJScene_DataRepository : MonoBehaviour
{
    public static event Action<Transform> OnTransformChanged;
    public static event Action<Transform> OnChildTransformChanged;
    public static event Action<Color> OnChildColorChanged;

    public static Transform CurrentOBJTransform
    {
        set//set�� CurrentOBJTransform�� ���� �Ҵ�Ǿ��� �� ����ȴ�. get�� transform a = OBJScene_DataRepository.CurrentOBJTransform ó�� ���� ������ �� ����ȴ�.
        {
            OnTransformChanged?.Invoke(value);//get�� ������ ���� ���� �� ����.
            Debug.Log("OnTransformChanged");
        }
    }
    public static Transform CurrentOBJChildTransform
    {
        set
        {
            OnChildTransformChanged?.Invoke(value);
            Debug.Log("OnChildTransformChanged");
        }
    }
    public static Color CurrentOBJChildColor
    {
        set
        {
            OnChildColorChanged?.Invoke(value);
            Debug.Log("OnChildColorChanged");
        }
    }
    public static void OBJScene_ClearEvents()//���� �ٲ� �� �̺�Ʈ�� �����ؾ� ������ �ȳ��´�.
    {
        OnTransformChanged = null;
        OnChildTransformChanged = null;
        OnChildColorChanged = null;
    }
}
