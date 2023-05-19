using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;//����Ʈ

public class OBJReadAndLoad : MonoBehaviour
{
    [SerializeField] private GameObject saveField; //ReadObj �� Ȱ��ȭ

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

    public Transform prefabTransform;//�����պ��� ���� �θ�ü. �ٸ� ��ũ��Ʈ���� ������ �� �־�� ��.
    private int vertexIndexCount = 0;
    private int objectVerticesCount = 0;//obj���Ͽ��� �������� ������Ʈ�� ������� �� �ִ�. �ι�° ������Ʈ�� ���ؽ� �ε����� ù�� ° ������Ʈ�� ���ؽ� ������ �������� �����ϹǷ� ����������Ʈ�� �ε��ϱ��� �� ����ŭ ������Ѵ�,
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
            for (int i = 0; i < prefabTransform.childCount; i++)// �����Ǵ� ������Ʈ�� ���̶�Ű�� ���� �Ʒ������� ����
            {
                Destroy(prefabTransform.GetChild(i).gameObject);
            }
        }
        ClearList();
        StreamReader reader = new StreamReader(filePath);
        string[] elements = new string[3];//f���� ���. ���� ���� / �� �����ϱ����ؼ�.ex, f 1/2/3 --> elements == {1, 2, 3}
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] words = line.Split(' ');

            switch (words[0])//v, vt ,vn, f
            {
                case "o":
                    if (childCount > 0)//����� ������Ʈ 2������ �۵�
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
                        int verticeNumber = int.Parse(elements[0]) - 1 - objectVerticesCount;//obj���Ͽ����� 1���� �����ϹǷ�
                        int uvNumver = int.Parse(elements[1]) - 1 - objectUvsCount;
                        int normalNumber = int.Parse(elements[2]) - 1 - objectNormalsCount;
                        unity_vertices.Add(obj_vertices[verticeNumber]);
                        unity_uvs.Add(obj_uvs[uvNumver]);
                        unity_normals.Add(obj_normals[normalNumber]);
                    }
                    for (int i = 0; i < words.Length - 3; i++)//n���� ���� : n + 1
                    {
                        unity_triangles.AddRange(new int[] { vertexIndexCount, vertexIndexCount + (i + 1), vertexIndexCount + (i + 2) });//�ð����
                    }
                    vertexIndexCount += words.Length - 1;//���� ���ؽ� ����. unity_vertices�� Ʈ���̾ޱ� ���� ������� �̸� ������ �Ǿ��ִ�. �׷��� ������� Ʈ���� �ޱ��� �����ϰ� �ε������� ���������ָ�ȴ�.
                    break;                          //���� ��� ù��° ���� f 1/1/1 2/4/1 4/9/1 3/7/1 ó�� �簢�� ������ ����ִٸ� for���� ���� Ʈ���̾ޱ� ����Ʈ���� {0, 1, 2, 0, 2, 3}�� �߰��� ���̰�
            }                                       //���� ���� ���� ���� unity_vertices�� �ε��� 4 ���� �����̹Ƿ� ���� ���ؽ� ������ �����ִ� ���̴�.
            
        }
        reader.Close();
        vertexIndexCount = 0;
        LoadOBJMesh();//������ ������Ʈ�� ����

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
