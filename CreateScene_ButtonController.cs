using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CreateScene_ButtonController : MonoBehaviour
{
    public GameObject menuPanel;
    //------------------------------------���� ���----------------------------------------//
    public void MenuButton()
    {
        menuPanel.SetActive(true);
    }
    public void PanelCloseButton()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false); //���� ��ư�� �θ� ������Ʈ false
    }
    //---------------------------------------CreateObjectScene----------------------------------------//
    public void CreateObjectScene_BaseSceneButton()
    {
        SceneManager.LoadScene("BaseScene");
    }
}
