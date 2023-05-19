using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectControlScript : MonoBehaviour
{
    private Transform selectedOBJ;
    public Transform childTransform;// insertTexture ��ũ��Ʈ���� �����ϱ� ���ؼ�

    [SerializeField] private GameObject loadTextureObject;
    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private Transform scrollView;
    [SerializeField] private Transform scrollViewContent;//��ũ�Ѻ� content
    [SerializeField] private GameObject sliders;
    [SerializeField] private GameObject rgbBtn;

    void Start()
    {
        //Transform ���� �ٲ���� �� ����� �޼ҵ� ���
        OBJScene_DataRepository.OnTransformChanged += HandleTransformChanged;
    }
    private void HandleTransformChanged(Transform newTransform)
    {
        selectedOBJ = newTransform;
        OBJChildFolderView();
    }


    public void OBJChildFolderView()//������ ������ �ε��ϱ� ���� ��ũ�Ѻ信 ��Ÿ����.
    {
        foreach (Transform childTransform in scrollViewContent)
        {
            GameObject.Destroy(childTransform.gameObject);
        }

        StartCoroutine(AfterDestroyAddObject());//�ı� �� ������ �ð��� �ɸ��� ������ �ڷ�ƾ�� ����Ͽ� �˻����־���Ѵ�.

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
