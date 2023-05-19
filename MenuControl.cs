using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject loadPanel;
    public void OpenPanel()
    {
        menuPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        menuPanel.SetActive(false);
    }
    public void GoToBaseScene()
    {
        OBJScene_DataRepository.OBJScene_ClearEvents();
        SceneManager.LoadScene("BaseScene");
    }
    public void OpenLoadObjectsPanel()
    {
        loadPanel.SetActive(true);
    }
}
