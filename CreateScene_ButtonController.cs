using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CreateScene_ButtonController : MonoBehaviour
{
    public GameObject menuPanel;
    //------------------------------------공통 요소----------------------------------------//
    public void MenuButton()
    {
        menuPanel.SetActive(true);
    }
    public void PanelCloseButton()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false); //눌른 버튼의 부모 오브젝트 false
    }
    //---------------------------------------CreateObjectScene----------------------------------------//
    public void CreateObjectScene_BaseSceneButton()
    {
        SceneManager.LoadScene("BaseScene");
    }
}
