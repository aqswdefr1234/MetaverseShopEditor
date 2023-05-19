using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadFolderData : MonoBehaviour //스크롤뷰에 붙인다.
{
    public Transform loadJsonTransform;
    void OnEnable()
    {
        loadJsonTransform.GetComponent<LoadJson>().LoadDataFolder();
    }
}
