using System;
using System.IO;
using System.Collections;//�ڷ�ƾ
using System.Collections.Generic;//����Ʈ
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileReadAndRoad : MonoBehaviour
{
    [SerializeField] private GameObject fileButtonPrefab;//��ư ������
    [SerializeField] private Transform scrollView;
    [SerializeField] private Transform loadFolderPanelContent;//��ũ�Ѻ� content
    private GameObject modelFBXFile;
    private string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder/OriginalOBJ_Folder";
    void Start()
    {
        scrollView.GetChild(2).GetComponent<Button>().onClick.AddListener(ClosePanel);//x��ư
        LoadDataFolder();
    }
    public void LoadDataFolder()//������ ������ �ε��ϱ� ���� ��ũ�Ѻ信 ��Ÿ����.
    {
        ScrollViewContentSize.ClearChild(loadFolderPanelContent);
        IsExistFolder(dataFolder);
        DirectoryInfo directoryInfo = new DirectoryInfo(dataFolder);
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsi in fileSystemInfos)
        {
            if (fsi is FileInfo file)//fsi ������ FileInfo Ŭ������ �ν��Ͻ����� �˻��ϴ� �ڵ�
            {
                // ���� ó�� �ڵ�
                if (file.Extension == ".obj")
                {
                    GameObject buttonClone = Instantiate(fileButtonPrefab, loadFolderPanelContent);
                    buttonClone.GetComponentInChildren<TMP_Text>().text = file.Name;
                    buttonClone.GetComponent<Button>().onClick.AddListener(delegate { LoadOBJFile(file.FullName); });
                }
            }
        }
        ScrollViewContentSize.ContentScaleChange(loadFolderPanelContent);
    }
    private void LoadOBJFile(string fileName)
    {
        transform.GetComponent<OBJReadAndLoad>().ReadObj(fileName);
    }
    
    private void IsExistFolder(string path)//���������� ����
    {
        if (!Directory.Exists(path)) // ������ �������� ������
        {
            Directory.CreateDirectory(path); // ���� ����
            Debug.Log("���� ���� �Ϸ�");
        }
    }
    private void ClosePanel()
    {
        scrollView.gameObject.SetActive(false);
    }
}
