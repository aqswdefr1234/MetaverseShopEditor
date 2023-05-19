using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;

public class OBJ_InsertTexture : MonoBehaviour
{
    [SerializeField] private Transform LoadTexturePanel;
    [SerializeField] private Transform LoadTextureContent;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Button refreshBtn;

    [SerializeField] private GameObject imageButtonPrefab;

    private Transform currentSelectedObject;
    private string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder/Texture";//using System;

    void Start()//obj������ child�� ���õǾ��� �� �г��� Ȱ��ȭ �Ǿ�� �Ѵ�.
    {
        IsExistFolder(currentFolder);
        LoadDataFolder(currentFolder);
        refreshBtn.onClick.AddListener(RefreshContent);
    }
    private void IsExistFolder(string folderPath)//���������� ����
    {
        if (!Directory.Exists(folderPath)) // ������ �������� ������
        {
            Directory.CreateDirectory(folderPath); // ���� ����
            Debug.Log("���� ���� �Ϸ�");
        }
    }
    private void RefreshContent()
    {
        StartCoroutine(DestroyChild());
    }
    IEnumerator DestroyChild()
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);
        foreach (Transform childTransform in LoadTextureContent)
        {
            GameObject.Destroy(childTransform.gameObject);
        }

        while (true)
        {
            if (LoadTextureContent.childCount == 0)
            {
                LoadDataFolder(currentFolder);
                break;
            }
            else
                yield return delay;
        }
    }
    private void LoadDataFolder(string folderPath)//������ ������ �ε��ϱ� ���� ��ũ�Ѻ信 ��Ÿ����.
    {
        int buttonPrefabCount = 0;
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsi in fileSystemInfos)
        {
            if (fsi is FileInfo file)//fsi ������ FileInfo Ŭ������ �ν��Ͻ����� �˻��ϴ� �ڵ�
            {
                // ���� ó�� �ڵ�
                if (file.Extension == ".jpg" || file.Extension == ".jpeg" || file.Extension == ".png" || file.Extension == ".gif" || file.Extension == ".bmp")
                {
                    buttonPrefabCount++;
                    GameObject buttonClone = Instantiate(imageButtonPrefab, LoadTextureContent);
                    buttonClone.GetComponentInChildren<TMP_Text>().text = file.Name;
                    buttonClone.GetComponent<Button>().onClick.AddListener(delegate { Loading_File(file.FullName); });
                }
            }
        }
        LoadTexturePanel.GetComponent<ContentSizeAdjust>().uiCount = buttonPrefabCount;
        LoadTexturePanel.GetComponent<ContentSizeAdjust>().ContentScaleChange();
    }
    private void Loading_File(string filePath)
    {
        Texture2D selectedTexture = new Texture2D(1, 1);
        byte[] fileData = File.ReadAllBytes(filePath);
        selectedTexture.LoadImage(fileData);
        rawImage.texture = selectedTexture;
        currentSelectedObject = GetComponent<ObjectControlScript>().childTransform;
        currentSelectedObject.GetComponent<Renderer>().material.mainTexture = selectedTexture;
        currentSelectedObject.GetComponent<ChildTextureString>().childTexturePath = filePath;//���߿� ������ ������ �� ���� �̹��� ���
    }
}
