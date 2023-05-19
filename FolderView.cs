using System;
using System.Collections.Generic;//����Ʈ ����� ����
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
public class FolderView : MonoBehaviour
{
    [SerializeField] private GameObject creatMeshObject;//��������������Ѵ�.
    [SerializeField] private GameObject settingObject;
    [SerializeField] private TMP_Text folderNameText;
    [SerializeField] private Transform fileButtonPrefab;
    [SerializeField] private Transform fileListContent;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private TMP_Text selectText;
    [SerializeField] private Transform foundation;
    [SerializeField] private List<string> beforeFolder = new List<string>();
    
    private string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private bool isReturn = false;//���Ϲ�ư�� ���� �̵��ߴ��� Ȯ��
    private GameObject DataObject;//������ �̹��� ��� �ֱ� ���Ͽ� �ش� ������Ʈ�� ã�Ƽ� �ִ´�.
    private int selectCount = 0;// �ո� ������ ���� �޸� ������ ���� �̹��� ��θ� �ٸ��� �ϱ� ���� �����ϴ� �뵵
    private string selectedImagePath = "";//���õ� �̹��� ��� loadimage �۵��� �ִ´�.
    private Texture2D selectedTexture;
    private byte[] fileData;
    private void Awake()
    {
        selectedTexture = new Texture2D(1, 1);
        selectText.text = "Choose a front image";
        LoadFiles(currentFolder);
    }

    public void LoadFiles(string folderPath)//������ ���ų�, �ڷΰ��� ��ư�� ������ ����ȴ�.
    {
        ScrollViewContentSize.ClearChild(fileListContent);
        int buttonCount = 0;
        if (isReturn == false)//�ڷΰ��� ��ư�� �������� �ƴ� ��
            beforeFolder.Add(currentFolder);//�̵��ϱ��� ���� ���� ���� �ִ´�.
        
        currentFolder = folderPath;//�̵��� ���� ���� �ִ´�.
        Debug.Log(currentFolder);

        //DirectoryInfo : ���͸� �� ���� ���͸��� �����, �̵��ϰ�, �����ϴ� �ν��Ͻ� �޼��带 ����
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        
        if (!directoryInfo.Exists)
        {
            Debug.LogError("Directory not found");
            return;
        }
        folderNameText.text = directoryInfo.Name;
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsi in fileSystemInfos)
        {
            if (fsi is FileInfo file)
            {
                // ���� ó�� �ڵ�
                if (file.Extension == ".jpg" || file.Extension == ".jpeg" || file.Extension == ".png" || file.Extension == ".gif" || file.Extension == ".bmp")
                {
                    buttonCount++;
                    Transform buttonTransform = Instantiate(fileButtonPrefab, fileListContent);
                    buttonTransform.GetComponentInChildren<TMP_Text>().text = file.Name;
                    buttonTransform.GetComponent<Button>().onClick.AddListener(delegate { LoadImage(file.FullName); });
                }
            }
            else if (fsi is DirectoryInfo directory)
            {
                //directory.FullName �� �ش������� ��ü ��θ� �����´�.
                Transform buttonTransform = Instantiate(fileButtonPrefab, fileListContent);
                if(directory.Name.Length != 0)
                {
                    buttonCount++;
                    if (directory.Name.Length <= 10)
                        buttonTransform.GetComponentInChildren<TMP_Text>().text = directory.Name;
                    else if (directory.Name.Length > 10)
                        buttonTransform.GetComponentInChildren<TMP_Text>().text = directory.Name.Substring(0, 8) + "..." + directory.Name.Substring(directory.Name.Length - 3, 3);
                }
                
                buttonTransform.GetComponent<Image>().color = new Color(1.0f, 1.0f, 0.0f);
                buttonTransform.GetComponent<Button>().onClick.AddListener(delegate { LoadFiles(directory.FullName); });
            }//Ŭ���̺�Ʈ �Ҵ�� �⺻������ void �Լ��� �Ҵ��� �� ������, delegate ���� ���ڰ� �ִ� �Լ� ��밡��.
        }
        Invoke("DelayContentView", 0.01f);
        
    }
    void LoadImage(string filePath)
    {
        selectedTexture = new Texture2D(1, 1);
        fileData = File.ReadAllBytes(filePath);
        selectedTexture.LoadImage(fileData);
        rawImage.texture = selectedTexture;
        selectedImagePath = filePath;
    }
    public void ReturnButton()//������ Ŭ���� �� ���� ��ư�� ������ ��ġ�� ����˴ϴ�.
    {
        if (beforeFolder.Count > 1)//������ 1���̻� �� ��
        {
            isReturn = true;
            LoadFiles(beforeFolder[beforeFolder.Count - 1]);//������ �ε��� ���� �ִ´�.
            Debug.Log("�ǵ��ư� ����" + beforeFolder[beforeFolder.Count - 1]);
            beforeFolder.RemoveAt(beforeFolder.Count - 1);
            isReturn = false;
        }
    }
    public void SelectImage()//��ư Ŭ���� �̹��� ��ΰ� data������Ʈ ��ũ��Ʈ�� ��.���� ������ �̹����� ���� �ֱ����ؼ�
    {
        if(rawImage.texture != null)
        {
            if (selectCount == 0)//�ո� ����
            {
                Texture2D frontTexture = selectedTexture;

                foundation.GetChild(0).GetComponent<ChildTextureString>().childTextureData = Convert.ToBase64String(fileData);
                foundation.GetChild(0).GetComponent<ChildTextureString>().childTexturePath = selectedImagePath;

                GetComponent<CreateMeshScript>().image_Front = frontTexture;
                foundation.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = frontTexture;

                selectCount = 1;
                selectText.text = "Choose a back image";
            }
            else if (selectCount == 1)//�޸� ����
            {
                Texture2D backTexture = selectedTexture;

                foundation.GetChild(1).GetComponent<ChildTextureString>().childTextureData = Convert.ToBase64String(FlipTexture(backTexture));
                foundation.GetChild(1).GetComponent<ChildTextureString>().childTexturePath = selectedImagePath;

                backTexture = selectedTexture;//flipTexture �޼ҵ忡�� selectedTexture�� �ٲ� �ؽ��ĸ� �ִ´�.
                GetComponent<CreateMeshScript>().image_Back = backTexture;
                foundation.GetChild(1).GetComponent<MeshRenderer>().material.mainTexture = backTexture;

                creatMeshObject.SetActive(true);
                settingObject.SetActive(false);//�޸���� ���ÿϷ�� �ش� ĵ���� ����
            }
        }
    }
    public byte[] FlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);
        Debug.Log("width" + original.width);
        Debug.Log("height" + original.height);
        for (int y = 0; y < original.height; y++)
        {
            for (int x = 0; x < original.width; x++)
            {
                flipped.SetPixel(original.width - x - 1, y, original.GetPixel(x, y));
            }
        }
        flipped.Apply();
        selectedTexture = flipped;
        return flipped.EncodeToPNG();
    }
    private void DelayContentView()
    {
        ScrollViewContentSize.ContentScaleChange(fileListContent);
    }
}