using System;
using System.Collections.Generic;//리스트 사용을 위해
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
public class FolderView : MonoBehaviour
{
    [SerializeField] private GameObject creatMeshObject;//직접연결해줘야한다.
    [SerializeField] private GameObject settingObject;
    [SerializeField] private TMP_Text folderNameText;
    [SerializeField] private Transform fileButtonPrefab;
    [SerializeField] private Transform fileListContent;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private TMP_Text selectText;
    [SerializeField] private Transform foundation;
    [SerializeField] private List<string> beforeFolder = new List<string>();
    
    private string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private bool isReturn = false;//리턴버튼을 눌러 이동했는지 확인
    private GameObject DataObject;//선택한 이미지 경로 넣기 위하여 해당 오브젝트를 찾아서 넣는다.
    private int selectCount = 0;// 앞면 선택할 때와 뒷면 선택할 때의 이미지 경로를 다르게 하기 위해 구분하는 용도
    private string selectedImagePath = "";//선택된 이미지 경로 loadimage 작동시 넣는다.
    private Texture2D selectedTexture;
    private byte[] fileData;
    private void Awake()
    {
        selectedTexture = new Texture2D(1, 1);
        selectText.text = "Choose a front image";
        LoadFiles(currentFolder);
    }

    public void LoadFiles(string folderPath)//폴더에 들어가거나, 뒤로가기 버튼을 누를때 실행된다.
    {
        ScrollViewContentSize.ClearChild(fileListContent);
        int buttonCount = 0;
        if (isReturn == false)//뒤로가기 버튼을 누른것이 아닐 때
            beforeFolder.Add(currentFolder);//이동하기전 현재 폴더 값을 넣는다.
        
        currentFolder = folderPath;//이동할 폴더 값을 넣는다.
        Debug.Log(currentFolder);

        //DirectoryInfo : 디렉터리 및 하위 디렉터리를 만들고, 이동하고, 열거하는 인스턴스 메서드를 노출
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
                // 파일 처리 코드
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
                //directory.FullName 은 해당폴더의 전체 경로를 가져온다.
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
            }//클릭이벤트 할당시 기본적으로 void 함수만 할당할 수 있지만, delegate 사용시 인자가 있는 함수 사용가능.
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
    public void ReturnButton()//폴더를 클릭할 때 리턴 버튼에 들어가기전 위치가 저장됩니다.
    {
        if (beforeFolder.Count > 1)//개수가 1개이상 일 때
        {
            isReturn = true;
            LoadFiles(beforeFolder[beforeFolder.Count - 1]);//마지막 인덱스 값을 넣는다.
            Debug.Log("되돌아간 폴더" + beforeFolder[beforeFolder.Count - 1]);
            beforeFolder.RemoveAt(beforeFolder.Count - 1);
            isReturn = false;
        }
    }
    public void SelectImage()//버튼 클릭시 이미지 경로가 data오브젝트 스크립트에 들어감.저장 폴더에 이미지도 같이 넣기위해서
    {
        if(rawImage.texture != null)
        {
            if (selectCount == 0)//앞면 선택
            {
                Texture2D frontTexture = selectedTexture;

                foundation.GetChild(0).GetComponent<ChildTextureString>().childTextureData = Convert.ToBase64String(fileData);
                foundation.GetChild(0).GetComponent<ChildTextureString>().childTexturePath = selectedImagePath;

                GetComponent<CreateMeshScript>().image_Front = frontTexture;
                foundation.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = frontTexture;

                selectCount = 1;
                selectText.text = "Choose a back image";
            }
            else if (selectCount == 1)//뒷면 선택
            {
                Texture2D backTexture = selectedTexture;

                foundation.GetChild(1).GetComponent<ChildTextureString>().childTextureData = Convert.ToBase64String(FlipTexture(backTexture));
                foundation.GetChild(1).GetComponent<ChildTextureString>().childTexturePath = selectedImagePath;

                backTexture = selectedTexture;//flipTexture 메소드에서 selectedTexture에 바뀐 텍스쳐를 넣는다.
                GetComponent<CreateMeshScript>().image_Back = backTexture;
                foundation.GetChild(1).GetComponent<MeshRenderer>().material.mainTexture = backTexture;

                creatMeshObject.SetActive(true);
                settingObject.SetActive(false);//뒷면까지 선택완료시 해당 캔버스 끄기
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