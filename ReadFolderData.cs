using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadFolderData : MonoBehaviour //��ũ�Ѻ信 ���δ�.
{
    public Transform loadJsonTransform;
    void OnEnable()
    {
        loadJsonTransform.GetComponent<LoadJson>().LoadDataFolder();
    }
}
