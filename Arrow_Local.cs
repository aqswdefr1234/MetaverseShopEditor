using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Local : MonoBehaviour
{
    [SerializeField] private Transform uiController;
    [SerializeField] private Transform xAxis;
    [SerializeField] private Transform yAxis;
    [SerializeField] private Transform zAxis;

    void OnEnable()
    {
        SetNewPostion();
    }
    public void SetNewPostion()
    {
        Transform targetObject;
        targetObject = BaseScene_OverallManager.selectedObjectTransform;
        transform.position = new Vector3(targetObject.position.x, targetObject.position.y, targetObject.position.z);
    }
}
