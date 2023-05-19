using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectControlScript : MonoBehaviour
{
    private Transform selectedOBJ;
    public Transform childTransform;// insertTexture 스크립트에서 접근하기 위해서

    [SerializeField] private GameObject loadTextureObject;
    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private Transform scrollView;
    [SerializeField] private Transform scrollViewContent;//스크롤뷰 content
    [SerializeField] private GameObject sliders;
    [SerializeField] private GameObject rgbBtn;

    void Start()
    {
        //Transform 값이 바뀌었을 때 실행될 메소드 등록
        OBJScene_DataRepository.OnTransformChanged += HandleTransformChanged;
    }
    private void HandleTransformChanged(Transform newTransform)
    {
        selectedOBJ = newTransform;
        OBJChildFolderView();
    }


    public void OBJChildFolderView()//데이터 파일을 로드하기 위해 스크롤뷰에 나타낸다.
    {
        foreach (Transform childTransform in scrollViewContent)
        {
            GameObject.Destroy(childTransform.gameObject);
        }

        StartCoroutine(AfterDestroyAddObject());//파괴 될 때까지 시간이 걸리기 때문에 코루틴을 사용하여 검사해주어야한다.

    }
    private void ChangeMatScriptPush(Transform selectedChild)
    {
        OBJScene_DataRepository.CurrentOBJChildTransform = selectedChild;
        childTransform = selectedChild;
        loadTextureObject.SetActive(true);
        sliders.SetActive(true);
        rgbBtn.SetActive(true);
        sliders.transform.GetChild(0).GetComponent<TMP_Text>().text = selectedChild.name;
    }
    IEnumerator AfterDestroyAddObject()
    {
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        while (true)
        {
            if (scrollViewContent.childCount == 0)
            {
                foreach (Transform childTransform in selectedOBJ)
                {
                    GameObject buttonClone = Instantiate(btnPrefab, scrollViewContent);
                    buttonClone.GetComponentInChildren<TMP_Text>().text = childTransform.name;
                    buttonClone.GetComponent<Button>().onClick.AddListener(delegate { ChangeMatScriptPush(childTransform); });
                }
                ScrollViewContentSize.ContentScaleChange(scrollViewContent);
                break;
            }
            else
                yield return delay;
        }
    }
}
