using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class LoadJson : MonoBehaviour
{
    [SerializeField] private Transform childPrefab;//obj �� ������ ����.
    [SerializeField] private Transform parentPrefab;//obj �� ������ ����.
    [SerializeField] private GameObject spotLight;
    [SerializeField] private GameObject pointLight;
    [SerializeField] private Material lightMat;
    [SerializeField] private Transform loadFolderPanelContent;
    [SerializeField] private Transform fileButtonPrefab;
    [SerializeField] private GameObject LoadingObject;//foundation ������Ʈ�� �ִ´�

    private Transform loadSceneObjectTransform;
    private int btnCount = 0;//��ư����
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
    public void LoadDataFolder()//������ ������ �ε��ϱ� ���� ��ũ�Ѻ信 ��Ÿ����.
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
            if (fsi is FileInfo file)//fsi ������ FileInfo Ŭ������ �ν��Ͻ����� �˻��ϴ� �ڵ�
            {
                // ���� ó�� �ڵ�
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
                GameObject cloneLoadingObject = Instantiate(pointLight, BaseScene_OverallManager.objCreationSpace);//room �ȿ� �ִ´�.
                loadSceneObjectTransform = cloneLoadingObject.transform;
            }
            else if(path == "ThisIsInnerObject_SpotLight")
            {
                GameObject cloneLoadingObject = Instantiate(spotLight, BaseScene_OverallManager.objCreationSpace);//room �ȿ� �ִ´�.
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
            int childCount = objData.obj_Name.Count - 1;// parent�̸��� ���ԵǾ������Ƿ�
            Transform cloneParent = Instantiate(parentPrefab, BaseScene_OverallManager.objCreationSpace);
            loadSceneObjectTransform = cloneParent;
            cloneParent.name = objData.obj_Name[0];
            cloneParent.GetComponent<ObjectSceneData>().path = path;
            for (int i = 0; i < childCount; i++)
            {
                Transform cloneChild = Instantiate(childPrefab, cloneParent);
                Mesh childMesh = cloneChild.GetComponent<MeshFilter>().mesh;
                cloneChild.name = objData.obj_Name[i + 1];
                //2���� �迭 �Ǵ� ����Ʈ�� �Ϲ������� json���� ��ȯ �Ұ����ϹǷ� child�� ���ؽ�,uv, Ʈ���� �ޱ��� ����Ʈ �ϳ��� ����� ������ŭ �������� �����ϴ� ������
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
        // PNG ��Ʈ�� ���� ����Ʈ �迭�� ���ڵ�
        byte[] bytes = System.Convert.FromBase64String(encodedValue);

        // ����Ʈ �迭�� �ؽ�ó�� ��ȯ
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        // �ؽ�ó ���
        return texture;
    }
}
