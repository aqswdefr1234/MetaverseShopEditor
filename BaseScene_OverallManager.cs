using UnityEngine;
using System;

public class BaseScene_OverallManager : MonoBehaviour
{
    public static event Action<Transform> OnSelectTransformChanged;
    public static event Action<Vector3> OnPositionChanged;

    public static Transform objCreationSpace;//오브젝트 생성공간

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
        set//값 할당시 실행.get이 없기 때문에 읽을 수는 없음.
        {
            Debug.Log("set");
            if(value != null)
            {
                selectedTransform = value;
                OnSelectTransformChanged?.Invoke(value);//get이 없으면 값을 읽을 수 없다.
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
