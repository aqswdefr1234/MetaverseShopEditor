using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;//리스트

public class OBJReadAndLoad : MonoBehaviour
{
    [SerializeField] private GameObject saveField; //ReadObj 후 활성화

    [SerializeField] private Transform foundation_Parent;
    [SerializeField] private Transform foundation_Child;
    [SerializeField] private List<Vector3> obj_vertices = new List<Vector3>();
    [SerializeField] private List<Vector2> obj_uvs = new List<Vector2>();
    [SerializeField] private List<Vector3> obj_normals = new List<Vector3>();
    [SerializeField] private string currentObjectName = "";
    public List<Vector3> unity_vertices = new List<Vector3>();
    public List<Vector2> unity_uvs = new List<Vector2>();
    public List<Vector3> unity_normals = new List<Vector3>();
    public List<int> unity_triangles = new List<int>();

    public Transform prefabTransform;//프리팹복제 후의 부모객체. 다른 스크립트에서 접근할 수 있어야 함.
    private int vertexIndexCount = 0;
    private int objectVerticesCount = 0;//obj파일에는 여러개의 오브젝트가 들어있을 수 있다. 두번째 오브젝트의 버텍스 인덱스는 첫번 째 오브젝트의 버텍스 개수의 다음부터 시작하므로 다음오브젝트를 로드하기전 그 값만큼 빼줘야한다,
    private int objectUvsCount = 0;
    private int objectNormalsCount = 0;

    void Start()
    {
        Transform foundationPrefab = Instantiate(foundation_Parent);
        prefabTransform = foundationPrefab;
    }

    public void ReadObj(string filePath)
    {
        int childCount = 0;
        objectVerticesCount = 0;
        objectUvsCount = 0;
        objectNormalsCount = 0;

        if (prefabTransform.childCount > 0)
        {
            for (int i = 0; i < prefabTransform.childCount; i++)// 생성되는 오브젝트는 하이라키상 가장 아래쪽으로 생성
            {
                Destroy(prefabTransform.GetChild(i).gameObject);
            }
        }
        ClearList();
        StreamReader reader = new StreamReader(filePath);
        string[] elements = new string[3];//f에서 사용. 워드 안의 / 를 구분하기위해서.ex, f 1/2/3 --> elements == {1, 2, 3}
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] words = line.Split(' ');

            switch (words[0])//v, vt ,vn, f
            {
                case "o":
                    if (childCount > 0)//복사된 오브젝트 2개부터 작동
                    {
                        LoadOBJMesh();
                        objectVerticesCount = obj_vertices.Count;
                        objectUvsCount = obj_uvs.Count;
                        objectNormalsCount = obj_normals.Count;
                        ClearList();
                        vertexIndexCount = 0;
                    }
                    currentObjectName = words[1];
                    childCount++;
                    break;
                case "v":
                    float x = float.Parse(words[1]);
                    float y = float.Parse(words[2]);
                    float z = float.Parse(words[3]);
                    obj_vertices.Add(new Vector3(x, y, z));
                    break;
                case "vt":
                    float u = float.Parse(words[1]);
                    float v = float.Parse(words[2]);
                    obj_uvs.Add(new Vector2(u, v));
                    break;
                case "vn":
                    float nx = float.Parse(words[1]);
                    float ny = float.Parse(words[2]);
                    float nz = float.Parse(words[3]);
                    obj_normals.Add(new Vector3(nx, ny, nz));
                    break;
                case "f":
                    for (int i = 1; i < words.Length ; i++)
                    {
                        elements = words[i].Split("/");
                        int verticeNumber = int.Parse(elements[0]) - 1 - objectVerticesCount;//obj파일에서는 1부터 시작하므로
                        int uvNumver = int.Parse(elements[1]) - 1 - objectUvsCount;
                        int normalNumber = int.Parse(elements[2]) - 1 - objectNormalsCount;
                        unity_vertices.Add(obj_vertices[verticeNumber]);
                        unity_uvs.Add(obj_uvs[uvNumver]);
                        unity_normals.Add(obj_normals[normalNumber]);
                    }
                    for (int i = 0; i < words.Length - 3; i++)//n각형 길이 : n + 1
                    {
                        unity_triangles.AddRange(new int[] { vertexIndexCount, vertexIndexCount + (i + 1), vertexIndexCount + (i + 2) });//시계방향
                    }
                    vertexIndexCount += words.Length - 1;//사용된 버텍스 개수. unity_vertices는 트라이앵글 구성 순서대로 미리 정렬이 되어있다. 그래서 순서대로 트라이 앵글을 구성하고 인덱스값만 증가시켜주면된다.
                    break;                          //예를 들어 첫번째 줄이 f 1/1/1 2/4/1 4/9/1 3/7/1 처럼 사각형 정보가 들어있다면 for문을 통해 트라이앵글 리스트에는 {0, 1, 2, 0, 2, 3}이 추가될 것이고
            }                                       //다음 줄을 읽을 때는 unity_vertices의 인덱스 4 부터 시작이므로 사용된 버텍스 개수를 더해주는 것이다.
            
        }
        reader.Close();
        vertexIndexCount = 0;
        LoadOBJMesh();//마지막 오브젝트만 적용

        saveField.SetActive(true);
        OBJScene_DataRepository.CurrentOBJTransform = prefabTransform;
    }
    private void LoadOBJMesh()
    {
        Mesh objMesh = new Mesh();
        Transform child = Instantiate(foundation_Child, prefabTransform);
        child.name = currentObjectName;
        objMesh = child.GetComponent<MeshFilter>().mesh;
        objMesh.vertices = unity_vertices.ToArray();
        objMesh.uv = unity_uvs.ToArray();
        objMesh.triangles = unity_triangles.ToArray();
        objMesh.normals = unity_normals.ToArray();
    }
    private void ClearList()
    {
        obj_vertices.Clear();
        obj_uvs.Clear();
        obj_normals.Clear();

        unity_vertices.Clear();
        unity_uvs.Clear();
        unity_normals.Clear();
        unity_triangles.Clear();
    }
}
