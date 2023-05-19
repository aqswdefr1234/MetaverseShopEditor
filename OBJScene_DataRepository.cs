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
        set//set은 CurrentOBJTransform에 값이 할당되었을 때 실행된다. get은 transform a = OBJScene_DataRepository.CurrentOBJTransform 처럼 값을 가져올 때 실행된다.
        {
            OnTransformChanged?.Invoke(value);//get이 없으면 값을 읽을 수 없다.
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
    public static void OBJScene_ClearEvents()//씬이 바뀔 때 이벤트를 해제해야 에러가 안나온다.
    {
        OnTransformChanged = null;
        OnChildTransformChanged = null;
        OnChildColorChanged = null;
    }
}
