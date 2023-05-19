using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectSceneData : MonoBehaviour
{
    public string myName;
    public Vector3 myPosition;
    public Vector3 myRotation;
    public Vector3 myScale;
    public string myProductExplanation;

    public string path;//오브젝트를 로드할 때 그 경로를 입력한다. 외부에서 접근 할 수 있도록 public 선언
    
    
    void Start()
    {
        myName = transform.name;
        myPosition = transform.position;
        myRotation = transform.rotation.eulerAngles;
        myScale = transform.localScale;
        myProductExplanation = "";
        
    }
    public void TransformChanged()//다른 스크립트에서 활용하기 위해서 만들어둔다.
    {
        if (transform.position != myPosition)
        {
            myPosition = transform.position;
        }
        if (transform.rotation.eulerAngles != myRotation)
        {
            myRotation = transform.rotation.eulerAngles;
        }
        if (transform.localScale != myScale)
        {
            myScale = transform.localScale;
        }
    }
    public void StartSceneLoadingObjects(string name, Vector3 pos, Vector3 rot, Vector3 scal, string expalain, string filePath)
    {
        myName = name;
        myPosition = pos;
        myRotation = rot;
        myScale = scal;
        myProductExplanation = expalain;
        path = filePath;
        transform.name = name;
        transform.position = myPosition;
        transform.rotation = Quaternion.Euler(myRotation);
        transform.localScale = myScale;
    }
}
