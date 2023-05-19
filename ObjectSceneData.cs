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

    public string path;//������Ʈ�� �ε��� �� �� ��θ� �Է��Ѵ�. �ܺο��� ���� �� �� �ֵ��� public ����
    
    
    void Start()
    {
        myName = transform.name;
        myPosition = transform.position;
        myRotation = transform.rotation.eulerAngles;
        myScale = transform.localScale;
        myProductExplanation = "";
        
    }
    public void TransformChanged()//�ٸ� ��ũ��Ʈ���� Ȱ���ϱ� ���ؼ� �����д�.
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
