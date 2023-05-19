using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class LoadJson : MonoBehaviour
{
    [SerializeField] private Transform childPrefab;//obj 씬 폴더에 있음.
    [SerializeField] private Transform parentPrefab;//obj 씬 폴더에 있음.
    [SerializeField] private GameObject spotLight;
    [SerializeField] private GameObject pointLight;
    [SerializeField] private Material lightMat;
    [SerializeField] private Transform loadFolderPanelContent;
    [SerializeField] private Transform fileButtonPrefab;
    [SerializeField] private GameObject LoadingObject;//foundation 오브젝트를 넣는다

    private Transform loadSceneObjectTransform;
    private int btnCount = 0;//버튼개수
    private string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder/OBJ_File";
    private string sceneDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder/SceneData";

    private Transform objectsSpace;
    [SerializeField] private Transform room1Prefab;
    [SerializeField] private Transform room2Prefab;
    
    void Start()
    {
        objectsSpace = BaseScene_OverallManager.objCreationSpace;
        string json = File.ReadAllText(sceneDataPath);
        TransformData tfd = JsonUtility.FromJson<TransformData>(json);
        for (int i = 0; i < tfd.myName.Count; i++)
        {
            LoadData(tfd.path[i]);//cloneLoadingObject
            loadSceneObjectTransform.GetComponent<ObjectSceneData>().StartSceneLoadingObjects(tfd.myName[i], tfd.myPosition[i], tfd.myRotation[i], tfd.myScale[i], tfd.myProductExplanation[i], tfd.path[i]);
        }
        if (tfd.myRoomName == "Room1")
        {
            Transform room = Instantiate(room1Prefab, objectsSpace);
            room.name = "Room1";
        }

        else if (tfd.myRoomName == "Room2")
        {
            Transform room = Instantiate(room2Prefab, objectsSpace);
            room.name = "Room2";
        }

    }
    public void LoadDataFolder()//데이터 파일을 로드하기 위해 스크롤뷰에 나타낸다.
    {
        ScrollViewContentSize.ClearChild(loadFolderPanelContent);

        DirectoryInfo directoryInfo = new DirectoryInfo(dataFolder);
        if (!directoryInfo.Exists)
        {
            Debug.LogError("Directory not found");
            return;
        }
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsi in fileSystemInfos)
        {
            if (fsi is FileInfo file)//fsi 변수가 FileInfo 클래스의 인스턴스인지 검사하는 코드
            {
                // 파일 처리 코드
                if (file.Extension == "")
                {
                    btnCount++;
                    Transform buttonTransform = Instantiate(fileButtonPrefab, loadFolderPanelContent);
                    buttonTransform.GetComponentInChildren<TMP_Text>().text = file.Name;
                    buttonTransform.GetComponent<Button>().onClick.AddListener(delegate { LoadData(file.FullName); });
                }
            }
        }
        ScrollViewContentSize.ContentScaleChange(loadFolderPanelContent);
    }
    public void LoadData(string path)
    {
        if (path.Substring(0, 17) == "ThisIsInnerObject")
        {
            if(path == "ThisIsInnerObject_PointLight")
            {
                GameObject cloneLoadingObject = Instantiate(pointLight, BaseScene_OverallManager.objCreationSpace);//room 안에 넣는다.
                loadSceneObjectTransform = cloneLoadingObject.transform;
            }
            else if(path == "ThisIsInnerObject_SpotLight")
            {
                GameObject cloneLoadingObject = Instantiate(spotLight, BaseScene_OverallManager.objCreationSpace);//room 안에 넣는다.
                loadSceneObjectTransform = cloneLoadingObject.transform;
            }
            loadSceneObjectTransform.GetComponent<MeshRenderer>().material = lightMat;
        }
        else
        {
            int verticesAdd = 0;
            int uvsAdd = 0;
            int trianglesAdd = 0;

            string json = File.ReadAllText(path);//OBJ_DataCustomParsing
            OBJ_DataCustomParsing objData = JsonUtility.FromJson<OBJ_DataCustomParsing>(json);
            int childCount = objData.obj_Name.Count - 1;// parent이름도 포함되어있으므로
            Transform cloneParent = Instantiate(parentPrefab, BaseScene_OverallManager.objCreationSpace);
            loadSceneObjectTransform = cloneParent;
            cloneParent.name = objData.obj_Name[0];
            cloneParent.GetComponent<ObjectSceneData>().path = path;
            for (int i = 0; i < childCount; i++)
            {
                Transform cloneChild = Instantiate(childPrefab, cloneParent);
                Mesh childMesh = cloneChild.GetComponent<MeshFilter>().mesh;
                cloneChild.name = objData.obj_Name[i + 1];
                //2차원 배열 또는 리스트는 일반적으로 json으로 변환 불가능하므로 child의 버텍스,uv, 트라이 앵글을 리스트 하나로 만들어 개수만큼 범위에서 추출하는 방법사용
                childMesh.vertices = objData.obj_Vertices.GetRange(verticesAdd, objData.child_VerticesCount[i]).ToArray();
                childMesh.uv = objData.obj_Uvs.GetRange(uvsAdd, objData.child_UVCount[i]).ToArray();
                childMesh.triangles = objData.obj_Polygon.GetRange(trianglesAdd, objData.child_TrianglesCount[i]).ToArray();

                verticesAdd += objData.child_VerticesCount[i];
                uvsAdd += objData.child_UVCount[i];
                trianglesAdd += objData.child_TrianglesCount[i];

                if(objData.obj_Texture[i] != "null")
                {
                    cloneChild.GetComponent<Renderer>().material.mainTexture = Decoding(objData.obj_Texture[i]);
                }
                cloneChild.GetComponent<Renderer>().material.color = objData.obj_color32[i];
                
            }
        }
    }
    private Texture2D Decoding(string encodedValue)//
    {
        // PNG 스트링 값을 바이트 배열로 디코딩
        byte[] bytes = System.Convert.FromBase64String(encodedValue);

        // 바이트 배열을 텍스처로 변환
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        // 텍스처 출력
        return texture;
    }
}
