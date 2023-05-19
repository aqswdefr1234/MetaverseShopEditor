using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class CreateMeshScript : MonoBehaviour
{
    
    public Texture2D image_Front = null;
    public Texture2D image_Back = null;

    [SerializeField] private GameObject createMeshObject;
    [SerializeField] private RawImage creatMesh_RawImage;//creatMesh ������Ʈ�� raw�̹���
    [SerializeField] private GameObject foundation_Front;//�޽��� �ٲ� ���� ������Ʈ �ո�
    [SerializeField] private GameObject foundation_Back;//�޽��� �ٲ� ���� ������Ʈ �޸�
    [SerializeField] private GameObject uv_UI;//ĵ������ UV_UI������Ʈ
    [SerializeField] private Transform leftBottom;//raw�̹����� ���� �ϴ� �����ǰ�
    [SerializeField] private Transform rightUp;//raw�̹����� ������ ��� �����ǰ�
    [SerializeField] private GameObject saveObject;

    private Mesh mesh_Front;
    private Mesh mesh_Back;
    private MeshRenderer meshRenderer;
    private Vector3[] vertexPos = new Vector3[15];//ui ���ؽ� �����ǰ� �б�
    private float rawImageWidth = 0f;
    private int clickCreateButtonCount = 0;

    void Start()//��ó������ awake���ߴµ� ��Ȱ��ȭ ���¿����� �۵��Ǵ� Ư�������� ������� �̻��ϰԳ��Դ�.
    {
        rawImageWidth = Convert.ToInt32(rightUp.position.x - leftBottom.position.x);//�ʺ� ���ϱ�
        foundation_Front.GetComponent<MeshFilter>().mesh.Clear();
        foundation_Back.GetComponent<MeshFilter>().mesh.Clear();
        mesh_Front = foundation_Front.GetComponent<MeshFilter>().mesh;
        mesh_Back = foundation_Back.GetComponent<MeshFilter>().mesh;

        StartCoroutine(IsLoadRawImage());
    }
    IEnumerator IsLoadRawImage()
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);
        while (true)
        {
            if(image_Front != null && image_Back != null)//�̹����� ���� �����ߴٸ� or ������ ��ũ��Ʈ���� �۾��� �Ϸ�Ǿ��ٸ�
            {
                creatMesh_RawImage.texture = image_Front;
                break;
            }
            yield return delay;
        }
    }
    public void CreateButton()//sign == -1 �̸� �ո�,sign == 1 �̸� �޸�
    {
        if (clickCreateButtonCount == 0)
        {
            CreateMeshes(foundation_Front, mesh_Front, image_Front, -1);
            CreateMeshes(foundation_Back, mesh_Back, image_Back, 1);
            CreateUvsBack(foundation_Front, mesh_Front, image_Front, -1);
            clickCreateButtonCount = 1;
            creatMesh_RawImage.texture = image_Back;
        }

        else if (clickCreateButtonCount == 1)
        {
            CreateUvsBack(foundation_Back, mesh_Back, image_Back, 1);
            createMeshObject.SetActive(false);
            saveObject.SetActive(true);
            foundation_Front.transform.parent.GetComponent<TurningObject>().enabled = true;
        }
    }
    void CreateMeshes(GameObject foundation, Mesh mesh, Texture2D image, int sign)//�ո��϶� sign �� 1, �޸��� �� sign�� -1
    {
        List<Vector3> inner_Vertices = new List<Vector3>();
        List<Vector2> inner_Uvs = new List<Vector2>();
        List<int> tri = new List<int>();

        meshRenderer = foundation.GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = image;
        for (int i = 0; i < 15; i++)
        {
            vertexPos[i] = uv_UI.transform.Find("Vertex (" + i + ")").transform.position;//ui ���ؽ� ������
        }

        Vector3 leftQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[14], vertexPos[0], vertexPos[1], vertexPos[2]);
        Vector3 rightQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[5], vertexPos[6], vertexPos[7], vertexPos[8]);
        Vector3 reductionLeftQuad = MoveOrigin_Vector3(leftQuadCenterVec3, leftBottom, rawImageWidth, true);//���� ���� ��ҵ� ����� �߽��� ��ǥ
        Vector3 reductionRightQuad = MoveOrigin_Vector3(rightQuadCenterVec3, leftBottom, rawImageWidth, true);

        Vector3 verticesVec3 = new Vector3();
        Vector2 uvsVec2 = new Vector2();
        float vecPosX = 0f;
        float vecPosY = 0f;
        for (int i = 0; i < 15; i++)//�� ���ؽ��� 450���� �������.������ :  �̷��� ����� vertex1���� ���� ������ ����ϸ鼭 �ٸ� triangle�� ����ع�����. �� �޽õ��� ���Ĺ�����. ���� �������� �������� �ʿ���.
        {
            float curve = 0f;


            for (int j = 0; j < 30; j++)//���� ���ؽ��� uv�� ���� �������� �ٿ����� z���� ������Ų��. �׷��� ���� ��� ǥ������.
            {
                if (j == 0)
                    curve = 0f;
                else
                    curve = 0.02f;
                //���ʿ� + j / (30 * 2f) �� ���� ������ ������ ������� ���ؽ����� 0,0 �������� �����Ǳ� ������ ���� �ö󰥶����� 1/(30 * 2) �� ���� ���Ѿ��� ���⼭ 30�� ���� 30�� �̱� �����̰� * 2�� �ִ� ���� �ϴ� ���� 0.5,0.5�� �Ѿ�� �ʾƾ� �ϹǷ�
                if (i == 0 || i == 1)//�� �� �κ� �������� �ٸ��� �ϱ� ���� 
                {
                    vecPosX = (vertexPos[i].x - leftQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.x;//�簢�� �߽��� �������� ��ҽ�Ų �� �ٽ� �� �Ű���
                    vecPosY = (vertexPos[i].y - leftQuadCenterVec3.y) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.y;
                    verticesVec3 = new Vector3(vecPosX, vecPosY, curve * sign);
                    uvsVec2 = new Vector2(verticesVec3.x + 0.5f, verticesVec3.y + 0.5f);
                    inner_Vertices.Add(verticesVec3);
                    inner_Uvs.Add(uvsVec2);
                }
                else if (i == 6 || i == 7)
                {
                    vecPosX = (vertexPos[i].x - rightQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionRightQuad.x;//�簢�� �߽��� �������� ��ҽ�Ų �� �ٽ� �� �Ű���
                    vecPosY = (vertexPos[i].y - rightQuadCenterVec3.y) * (30f - j) / (30 * rawImageWidth) + reductionRightQuad.y;
                    verticesVec3 = new Vector3(vecPosX, vecPosY, curve * sign);
                    uvsVec2 = new Vector2(verticesVec3.x + 0.5f, verticesVec3.y + 0.5f);
                    inner_Vertices.Add(verticesVec3);
                    inner_Uvs.Add(uvsVec2);
                }
                else
                {
                    vecPosX = MoveOrigin_Vector3(vertexPos[i], leftBottom, rawImageWidth, false).x;
                    vecPosY = MoveOrigin_Vector3(vertexPos[i], leftBottom, rawImageWidth, false).y;
                    verticesVec3 = new Vector3(vecPosX * (30f - j) / (30 * rawImageWidth), vecPosY * (30f - j) / (30 * rawImageWidth), curve * sign);//sign == 1 �̸� �ո�,sign == -1 �̸� �޸�
                    //�������� �ٽ� �Űܾ���.(rawImage�� ���� �𼭸� �α�����)
                    uvsVec2 = new Vector2(verticesVec3.x + 0.5f, verticesVec3.y + 0.5f);
                    inner_Vertices.Add(verticesVec3);
                    inner_Uvs.Add(uvsVec2);
                }
            }
        }
        Vector2 centerVertex15 = CenterXY_Value(new Vector2(inner_Vertices[1 * 30 + 29].x, inner_Vertices[1 * 30 + 29].y), new Vector2(inner_Vertices[2 * 30 + 29].x, inner_Vertices[2 * 30 + 29].y), new Vector2(inner_Vertices[5 * 30 + 29].x, inner_Vertices[5 * 30 + 29].y), new Vector2(inner_Vertices[6 * 30 + 29].x, inner_Vertices[6 * 30 + 29].y));
        inner_Vertices.Add(new Vector3(centerVertex15.x, centerVertex15.y, 0.02f * sign));
        inner_Uvs.Add(CenterXY_Value(inner_Uvs[1 * 30 + 29], inner_Uvs[2 * 30 + 29], inner_Uvs[5 * 30 + 29], inner_Uvs[6 * 30 + 29]));
        Debug.Log(inner_Uvs.Count);
        Debug.Log(inner_Vertices.Count);


        mesh.vertices = inner_Vertices.ToArray();
        for (int i = 0; i < 29; i++)//���ؽ��� 2���� Ʈ���� �ޱ�.�� 30���� ���� ������ ������ ���� ��������� �ٸ��Ƿ� ����
        {
            for (int j = 0; j < 15; j++)
            {
                if (j == 14)//�������� ������ ��° ���ؽ� �� �� 0�ϰ�������
                {
                    if (sign == -1)
                    {
                        tri.AddRange(new int[] { i + 30 * j, i, i + 1 });//�ݽð����
                        tri.AddRange(new int[] { i + 30 * j, i + 1, i + 30 * j + 1 });//�ݽð����
                    }
                    else
                    {
                        tri.AddRange(new int[] { i + 30 * j, i + 1, i });//�ð����
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * j + 1, i + 1 });//�ð����
                    }
                }
                else
                {
                    if (sign == -1)
                    {
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * (j + 1), i + 30 * (j + 1) + 1 });//�ݽð����
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * (j + 1) + 1, i + 30 * j + 1 });//�ݽð����
                    }
                    else
                    {
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * j + 30 + 1, i + 30 * j + 30 });//�ð����
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * j + 1, i + 30 * j + 30 + 1 });//�ð����
                    }

                }

            }
        }
        for (int i = 0; i < 15; i++)//��������
        {
            if (i != 14)//15��° ���� ���ؽ��� ��츦 ���� �� ��, ������ �ε����� �ƴѰ��. ���� ��쿡�� 14
            {

                if (sign == -1)
                    tri.AddRange(new int[] { 450, (i + 1) * 30 + 29, i * 30 + 29 });//�ݽð����
                else
                    tri.AddRange(new int[] { 450, i * 30 + 29, (i + 1) * 30 + 29 });//�ð����
                //
            }

            else
            {

                if (sign == -1)//�ݽð����
                {
                    tri.Add(29);
                    tri.Add(i * 30 + 29);
                    tri.Add(450);
                }
                else//�ð����
                {
                    tri.Add(450);
                    tri.Add(i * 30 + 29);
                    tri.Add(29);
                }

            }

        }
        Debug.Log(tri.Count);

        mesh.triangles = tri.ToArray();//int[] �迭�� ���� �ϹǷ�
        mesh.uv = inner_Uvs.ToArray();

        NormalsInitialize(mesh, sign);//��� �ʱ�ȭ

        //foundation.GetComponent<MeshRenderer>().material.mainTexture = image;
        //foundation.transform.position = new Vector3(0f, 0f, 2f);
    }
    void CreateUvsBack(GameObject foundation, Mesh mesh, Texture2D image, int normalDirection)//�ո��϶� sign �� 1, �޸��� �� sign�� -1
    {
        List<Vector2> inner_Uvs = new List<Vector2>();
        foundation.GetComponent<MeshFilter>().mesh.uv = inner_Uvs.ToArray();
        meshRenderer = foundation.GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = image;
        for (int i = 0; i < 15; i++)
            vertexPos[i] = uv_UI.transform.Find("Vertex (" + i + ")").transform.position;//ui ���ؽ� ������

        Vector3 leftQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[14], vertexPos[0], vertexPos[1], vertexPos[2]);
        Vector3 rightQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[5], vertexPos[6], vertexPos[7], vertexPos[8]);
        Vector3 reductionLeftQuad = MoveOrigin_Vector3(leftQuadCenterVec3, leftBottom, rawImageWidth, true);//���� ���� ��ҵ� ����� �߽��� ��ǥ
        Vector3 reductionRightQuad = MoveOrigin_Vector3(rightQuadCenterVec3, leftBottom, rawImageWidth, true);
        Vector2 uvsVec2 = new Vector2();
        float vecPosX = 0f;
        float vecPosY = 0f;
        for (int i = 0; i < 15; i++)//�� ���ؽ��� 450���� �������.������ :  �̷��� ����� vertex1���� ���� ������ ����ϸ鼭 �ٸ� triangle�� ����ع�����. �� �޽õ��� ���Ĺ�����. ���� �������� �������� �ʿ���.
        {
            for (int j = 0; j < 30; j++)//���� ���ؽ��� uv�� ���� �������� �ٿ����� z���� ������Ų��. �׷��� ���� ��� ǥ������.
            {

                //���ʿ� + j / (30 * 2f) �� ���� ������ ������ ������� ���ؽ����� 0,0 �������� �����Ǳ� ������ ���� �ö󰥶����� 1/(30 * 2) �� ���� ���Ѿ��� ���⼭ 30�� ���� 30�� �̱� �����̰� * 2�� �ִ� ���� �ϴ� ���� 0.5,0.5�� �Ѿ�� �ʾƾ� �ϹǷ�
                if (i == 0 || i == 1)//�� �� �κ� �������� �ٸ��� �ϱ� ���� 
                {
                    vecPosX = (vertexPos[i].x - leftQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.x;//�簢�� �߽��� �������� ��ҽ�Ų �� �ٽ� �� �Ű���
                    vecPosY = (vertexPos[i].y - leftQuadCenterVec3.y) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.y;
                    uvsVec2 = new Vector2(vecPosX + 0.5f, vecPosY + 0.5f);
                    inner_Uvs.Add(uvsVec2);
                }
                else if (i == 6 || i == 7)
                {
                    vecPosX = (vertexPos[i].x - rightQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionRightQuad.x;//�簢�� �߽��� �������� ��ҽ�Ų �� �ٽ� �� �Ű���
                    vecPosY = (vertexPos[i].y - rightQuadCenterVec3.y) * (30f - j) / (30 * rawImageWidth) + reductionRightQuad.y;
                    uvsVec2 = new Vector2(vecPosX + 0.5f, vecPosY + 0.5f);
                    inner_Uvs.Add(uvsVec2);
                }
                else
                {
                    vecPosX = MoveOrigin_Vector3(vertexPos[i], leftBottom, rawImageWidth, false).x;
                    vecPosY = MoveOrigin_Vector3(vertexPos[i], leftBottom, rawImageWidth, false).y;
                    //�������� �ٽ� �Űܾ���.(rawImage�� ���� �𼭸� �α�����)
                    uvsVec2 = new Vector2(vecPosX * (30f - j) / (30 * rawImageWidth) + 0.5f, vecPosY * (30f - j) / (30 * rawImageWidth) + 0.5f);
                    //uvsVec2 = new Vector2((vecPosX + rawImageWidth / 2f) * (30f - j) / (30f * rawImageWidth) + j / (30f * 2f), (vecPosY + rawImageWidth / 2f) * (30f - j) / (30f * rawImageWidth) + j / (30f * 2f));

                    inner_Uvs.Add(uvsVec2);
                }
            }
        }
        inner_Uvs.Add(CenterXY_Value(inner_Uvs[1 * 30 + 29], inner_Uvs[2 * 30 + 29], inner_Uvs[5 * 30 + 29], inner_Uvs[6 * 30 + 29]));
        mesh.uv = inner_Uvs.ToArray();
        NormalsInitialize(mesh, normalDirection);//��� �ʱ�ȭ
        foundation.GetComponent<MeshRenderer>().material.mainTexture = image;

    }
    void NormalsInitialize(Mesh mesh_, int normalValue)//����Ƽ ������ ���ؽ� �������� ��� ���� �����Ѵ�. �� ���ؽ� ������ ����. ��ְ��� ���ؽ��� ������ ��������� ��� ��ְ��� �ִ´�.
    {
        mesh_.RecalculateNormals();
        Vector3[] normals = mesh_.normals;
        Debug.Log("��� ����" + normals.Length);//���ؽ� ������ ����.

        for (int i = 0; i < normals.Length; i++)//��ְ� ����
        {
            if (i % 30 == 0)//ù��° ���� ���ؽ��� ���
            {
                int value = i / 30;
                if (value == 7 || value == 8)//������ ����κ�
                    normals[i] = new Vector3(1, 1, 0);
                else if (value == 0 || value == 14)//���� ����κ�
                    normals[i] = new Vector3(-1, 1, 0);
                else if (value == 13 || value == 12 || value == 11 || value == 10 || value == 9)//��κ�
                    normals[i] = new Vector3(0, 1, 0);
                else if (value == 1 || value == 2 || value == 3)//���� �Ҹ� �Ʒ��κ� or ���ʾƷ� �κ�
                    normals[i] = new Vector3(-1, -1, 0);
                else if (value == 5 || value == 6 || value == 4)//������ �Ҹ� �Ʒ��κ� or �����ʾƷ� �κ�
                    normals[i] = new Vector3(1, -1, 0);

            }
            else
                normals[i] = new Vector3(0, 0, normalValue);// ���� (0,0,-normalValue)�������� �����Ҷ� ���� �ݻ��Ų��.
        }
        mesh_.normals = normals;
    }
    Vector2 CenterXY_Value(Vector2 uvs1, Vector2 uvs2, Vector2 uvs5, Vector2 uvs6)//�Ҹ� ���� �Ʒ��κ��� �������� �������� ���Ѵ�.
    {
        Vector2 lineA = uvs2 - uvs1;
        Vector2 lineB = uvs6 - uvs5;

        float denominator = (lineB.y * lineA.x) - (lineB.x * lineA.y);//�и�

        if (denominator == 0)
        {
            // ���� A�� B�� �����ϹǷ� �������� ����
            return new Vector2((uvs1.x + uvs2.x + uvs5.x + uvs6.x) / 4f, (uvs1.y + uvs2.y + uvs5.y + uvs6.y) / 4f);
        }
        else
        {
            Vector2 pointDiff = uvs1 - uvs5;
            float numerator = (lineB.x * pointDiff.y) - (lineB.y * pointDiff.x);
            float t = numerator / denominator;
            Vector2 intersectionPoint = uvs1 + (lineA * t);
            Debug.Log("Intersection point: " + intersectionPoint);
            return intersectionPoint;
        }
    }
    Vector3 CenterBenchmark_Vector3(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4) //�簢���� �߽���
    {
        float x = (vec1.x + vec2.x + vec3.x + vec4.x) / 4f;
        float y = (vec1.y + vec2.y + vec3.y + vec4.y) / 4f;
        float z = (vec1.z + vec2.x + vec3.x + vec4.x) / 4f;
        Vector3 centerVec = new Vector3(x, y, z);
        return centerVec;
    }
    Vector3 MoveOrigin_Vector3(Vector3 movedVertix, Transform rawImageLeft, float rawImageWidth, bool isReducing) //���� �������� �ű�� �� ���
    {
        if (isReducing == true)
            return new Vector3((movedVertix.x - rawImageLeft.position.x - rawImageWidth / 2f) / rawImageWidth, (movedVertix.y - rawImageLeft.position.y - rawImageWidth / 2f) / rawImageWidth, 0f);
        else
            return new Vector3((movedVertix.x - rawImageLeft.position.x - rawImageWidth / 2f), (movedVertix.y - rawImageLeft.position.y - rawImageWidth / 2f), 0f);
    }
}
