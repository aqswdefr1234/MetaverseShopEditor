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

    void Start()//obj파일의 child가 선택되었을 때 패널이 활성화 되어야 한다.
    {
        IsExistFolder(currentFolder);
        LoadDataFolder(currentFolder);
        refreshBtn.onClick.AddListener(RefreshContent);
    }
    private void IsExistFolder(string folderPath)//폴더없으면 생성
    {
        if (!Directory.Exists(folderPath)) // 폴더가 존재하지 않으면
        {
            Directory.CreateDirectory(folderPath); // 폴더 생성
            Debug.Log("폴더 생성 완료");
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
    private void LoadDataFolder(string folderPath)//데이터 파일을 로드하기 위해 스크롤뷰에 나타낸다.
    {
        int buttonPrefabCount = 0;
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsi in fileSystemInfos)
        {
            if (fsi is FileInfo file)//fsi 변수가 FileInfo 클래스의 인스턴스인지 검사하는 코드
            {
                // 파일 처리 코드
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
        currentSelectedObject.GetComponent<ChildTextureString>().childTexturePath = filePath;//나중에 파일을 저장할 때 사용된 이미지 경로
    }
}
