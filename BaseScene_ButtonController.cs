using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BaseScene_ButtonController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject loadFolderPanel;
    [SerializeField] private Transform loadDataController;
    [SerializeField] private GameObject pointLightPrefab;
    [SerializeField] private GameObject spotLightPrefab;
    [SerializeField] private Material lightMat;//����ڰ� ����Ʈ ������Ʈ�� ������ �� �ְ� �ϱ�����
    [SerializeField] private GameObject roomPanel;

    private Transform objectsSpace;
    //------------------------------------BaseScene----------------------------------------//
    void Start()
    {
        objectsSpace = BaseScene_OverallManager.objCreationSpace;
    }
    public void BaseScene_CreateMeshButton()
    {
        BaseScene_OverallManager.BaseScene_ClearEvents();
        SceneManager.LoadScene("CreateObjectScene");
    }
    public void BaseScene_LoadOjbectButton()
    {
        loadFolderPanel.SetActive(true);
        menuPanel.SetActive(false);
        loadDataController.GetComponent<LoadJson>().LoadDataFolder();
    }
    public void RefreshPanel()
    {
        loadDataController.GetComponent<LoadJson>().LoadDataFolder();
    }
    public void OBJ_ConverterButton()
    {
        BaseScene_OverallManager.BaseScene_ClearEvents();
        SceneManager.LoadScene("OBJ_Converter");
    }
    public void PointLightButton()
    {
        GameObject light = Instantiate(pointLightPrefab, objectsSpace);
        light.GetComponent<MeshRenderer>().material = lightMat;
    }
    public void SpotLightButton()
    {
        GameObject light = Instantiate(spotLightPrefab, objectsSpace);
        light.GetComponent<MeshRenderer>().material = lightMat;
    }
    //------------------------------------���� ���----------------------------------------//
    public void MenuButton()
    {
        menuPanel.SetActive(true);
    }
    public void PanelCloseButton()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false); //���� ��ư�� �θ� ������Ʈ false
    }
    public void RoomPanel()
    {
        roomPanel.SetActive(true);
        menuPanel.SetActive(false);
    }
}
