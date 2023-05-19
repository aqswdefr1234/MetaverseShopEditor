using System;
using System.IO;
using System.Collections;//코루틴
using System.Collections.Generic;//리스트
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileReadAndRoad : MonoBehaviour
{
    [SerializeField] private GameObject fileButtonPrefab;//버튼 프리팹
    [SerializeField] private Transform scrollView;
    [SerializeField] private Transform loadFolderPanelContent;//스크롤뷰 content
    private GameObject modelFBXFile;
    private string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder/OriginalOBJ_Folder";
    void Start()
    {
        scrollView.GetChild(2).GetComponent<Button>().onClick.AddListener(ClosePanel);//x버튼
        LoadDataFolder();
    }
    public void LoadDataFolder()//데이터 파일을 로드하기 위해 스크롤뷰에 나타낸다.
    {
        ScrollViewContentSize.ClearChild(loadFolderPanelContent);
        IsExistFolder(dataFolder);
        DirectoryInfo directoryInfo = new DirectoryInfo(dataFolder);
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsi in fileSystemInfos)
        {
            if (fsi is FileInfo file)//fsi 변수가 FileInfo 클래스의 인스턴스인지 검사하는 코드
            {
                // 파일 처리 코드
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
    
    private void IsExistFolder(string path)//폴더없으면 생성
    {
        if (!Directory.Exists(path)) // 폴더가 존재하지 않으면
        {
            Directory.CreateDirectory(path); // 폴더 생성
            Debug.Log("폴더 생성 완료");
        }
    }
    private void ClosePanel()
    {
        scrollView.gameObject.SetActive(false);
    }
}
