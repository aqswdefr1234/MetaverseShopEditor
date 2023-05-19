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
    [SerializeField] private RawImage creatMesh_RawImage;//creatMesh 오브젝트의 raw이미지
    [SerializeField] private GameObject foundation_Front;//메쉬가 바뀔 기초 오브젝트 앞면
    [SerializeField] private GameObject foundation_Back;//메쉬가 바뀔 기초 오브젝트 뒷면
    [SerializeField] private GameObject uv_UI;//캔버스의 UV_UI오브젝트
    [SerializeField] private Transform leftBottom;//raw이미지의 왼쪽 하단 포지션값
    [SerializeField] private Transform rightUp;//raw이미지의 오른쪽 상단 포지션값
    [SerializeField] private GameObject saveObject;

    private Mesh mesh_Front;
    private Mesh mesh_Back;
    private MeshRenderer meshRenderer;
    private Vector3[] vertexPos = new Vector3[15];//ui 버텍스 포지션값 읽기
    private float rawImageWidth = 0f;
    private int clickCreateButtonCount = 0;

    void Start()//맨처음에는 awake로했는데 비활성화 상태에서도 작동되는 특성때문에 결과값이 이상하게나왔다.
    {
        rawImageWidth = Convert.ToInt32(rightUp.position.x - leftBottom.position.x);//너비 구하기
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
            if(image_Front != null && image_Back != null)//이미지를 전부 선택했다면 or 폴더뷰 스크립트에서 작업이 완료되었다면
            {
                creatMesh_RawImage.texture = image_Front;
                break;
            }
            yield return delay;
        }
    }
    public void CreateButton()//sign == -1 이면 앞면,sign == 1 이면 뒷면
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
    void CreateMeshes(GameObject foundation, Mesh mesh, Texture2D image, int sign)//앞면일때 sign 은 1, 뒷면일 때 sign은 -1
    {
        List<Vector3> inner_Vertices = new List<Vector3>();
        List<Vector2> inner_Uvs = new List<Vector2>();
        List<int> tri = new List<int>();

        meshRenderer = foundation.GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = image;
        for (int i = 0; i < 15; i++)
        {
            vertexPos[i] = uv_UI.transform.Find("Vertex (" + i + ")").transform.position;//ui 버텍스 포지션
        }

        Vector3 leftQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[14], vertexPos[0], vertexPos[1], vertexPos[2]);
        Vector3 rightQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[5], vertexPos[6], vertexPos[7], vertexPos[8]);
        Vector3 reductionLeftQuad = MoveOrigin_Vector3(leftQuadCenterVec3, leftBottom, rawImageWidth, true);//원점 기준 축소된 사격형 중심점 좌표
        Vector3 reductionRightQuad = MoveOrigin_Vector3(rightQuadCenterVec3, leftBottom, rawImageWidth, true);

        Vector3 verticesVec3 = new Vector3();
        Vector2 uvsVec2 = new Vector2();
        float vecPosX = 0f;
        float vecPosY = 0f;
        for (int i = 0; i < 15; i++)//총 버텍스가 450개씩 만들어짐.문제점 :  이렇게 만들면 vertex1번이 기준 점으로 축소하면서 다른 triangle을 통과해버린다. 즉 메시들이 겹쳐버린다. 따라서 개별적인 기준점이 필요함.
        {
            float curve = 0f;


            for (int j = 0; j < 30; j++)//기준 버텍스와 uv로 부터 스케일을 줄여가며 z값을 증가시킨다. 그래야 옷의 곡면 표현가능.
            {
                if (j == 0)
                    curve = 0f;
                else
                    curve = 0.02f;
                //뒤쪽에 + j / (30 * 2f) 를 붙인 이유는 생성한 모든층의 버텍스들이 0,0 기준으로 생성되기 때문에 층이 올라갈때마다 1/(30 * 2) 씩 증가 시켜야함 여기서 30은 층이 30개 이기 때문이고 * 2는 최대 증가 하는 값이 0.5,0.5를 넘어가지 않아야 하므로
                if (i == 0 || i == 1)//팔 쪽 부분 기준점을 다르게 하기 위해 
                {
                    vecPosX = (vertexPos[i].x - leftQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.x;//사각형 중심점 기준으로 축소시킨 후 다시 점 옮겨줌
                    vecPosY = (vertexPos[i].y - leftQuadCenterVec3.y) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.y;
                    verticesVec3 = new Vector3(vecPosX, vecPosY, curve * sign);
                    uvsVec2 = new Vector2(verticesVec3.x + 0.5f, verticesVec3.y + 0.5f);
                    inner_Vertices.Add(verticesVec3);
                    inner_Uvs.Add(uvsVec2);
                }
                else if (i == 6 || i == 7)
                {
                    vecPosX = (vertexPos[i].x - rightQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionRightQuad.x;//사각형 중심점 기준으로 축소시킨 후 다시 점 옮겨줌
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
                    verticesVec3 = new Vector3(vecPosX * (30f - j) / (30 * rawImageWidth), vecPosY * (30f - j) / (30 * rawImageWidth), curve * sign);//sign == 1 이면 앞면,sign == -1 이면 뒷면
                    //기준점을 다시 옮겨야함.(rawImage의 왼쪽 모서리 부근으로)
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
        for (int i = 0; i < 29; i++)//버텍스당 2개의 트라이 앵글.총 30개의 층이 있지만 마지막 층은 구성방식이 다르므로 제외
        {
            for (int j = 0; j < 15; j++)
            {
                if (j == 14)//같은층의 마지막 번째 버텍스 일 때 0하고만나야함
                {
                    if (sign == -1)
                    {
                        tri.AddRange(new int[] { i + 30 * j, i, i + 1 });//반시계방향
                        tri.AddRange(new int[] { i + 30 * j, i + 1, i + 30 * j + 1 });//반시계방향
                    }
                    else
                    {
                        tri.AddRange(new int[] { i + 30 * j, i + 1, i });//시계방향
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * j + 1, i + 1 });//시계방향
                    }
                }
                else
                {
                    if (sign == -1)
                    {
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * (j + 1), i + 30 * (j + 1) + 1 });//반시계방향
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * (j + 1) + 1, i + 30 * j + 1 });//반시계방향
                    }
                    else
                    {
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * j + 30 + 1, i + 30 * j + 30 });//시계방향
                        tri.AddRange(new int[] { i + 30 * j, i + 30 * j + 1, i + 30 * j + 30 + 1 });//시계방향
                    }

                }

            }
        }
        for (int i = 0; i < 15; i++)//마지막층
        {
            if (i != 14)//15번째 센터 버텍스인 경우를 제외 한 후, 마지막 인덱스가 아닌경우. 현재 경우에는 14
            {

                if (sign == -1)
                    tri.AddRange(new int[] { 450, (i + 1) * 30 + 29, i * 30 + 29 });//반시계방향
                else
                    tri.AddRange(new int[] { 450, i * 30 + 29, (i + 1) * 30 + 29 });//시계방향
                //
            }

            else
            {

                if (sign == -1)//반시계방향
                {
                    tri.Add(29);
                    tri.Add(i * 30 + 29);
                    tri.Add(450);
                }
                else//시계방향
                {
                    tri.Add(450);
                    tri.Add(i * 30 + 29);
                    tri.Add(29);
                }

            }

        }
        Debug.Log(tri.Count);

        mesh.triangles = tri.ToArray();//int[] 배열로 들어가야 하므로
        mesh.uv = inner_Uvs.ToArray();

        NormalsInitialize(mesh, sign);//노멀 초기화

        //foundation.GetComponent<MeshRenderer>().material.mainTexture = image;
        //foundation.transform.position = new Vector3(0f, 0f, 2f);
    }
    void CreateUvsBack(GameObject foundation, Mesh mesh, Texture2D image, int normalDirection)//앞면일때 sign 은 1, 뒷면일 때 sign은 -1
    {
        List<Vector2> inner_Uvs = new List<Vector2>();
        foundation.GetComponent<MeshFilter>().mesh.uv = inner_Uvs.ToArray();
        meshRenderer = foundation.GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = image;
        for (int i = 0; i < 15; i++)
            vertexPos[i] = uv_UI.transform.Find("Vertex (" + i + ")").transform.position;//ui 버텍스 포지션

        Vector3 leftQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[14], vertexPos[0], vertexPos[1], vertexPos[2]);
        Vector3 rightQuadCenterVec3 = CenterBenchmark_Vector3(vertexPos[5], vertexPos[6], vertexPos[7], vertexPos[8]);
        Vector3 reductionLeftQuad = MoveOrigin_Vector3(leftQuadCenterVec3, leftBottom, rawImageWidth, true);//원점 기준 축소된 사격형 중심점 좌표
        Vector3 reductionRightQuad = MoveOrigin_Vector3(rightQuadCenterVec3, leftBottom, rawImageWidth, true);
        Vector2 uvsVec2 = new Vector2();
        float vecPosX = 0f;
        float vecPosY = 0f;
        for (int i = 0; i < 15; i++)//총 버텍스가 450개씩 만들어짐.문제점 :  이렇게 만들면 vertex1번이 기준 점으로 축소하면서 다른 triangle을 통과해버린다. 즉 메시들이 겹쳐버린다. 따라서 개별적인 기준점이 필요함.
        {
            for (int j = 0; j < 30; j++)//기준 버텍스와 uv로 부터 스케일을 줄여가며 z값을 증가시킨다. 그래야 옷의 곡면 표현가능.
            {

                //뒤쪽에 + j / (30 * 2f) 를 붙인 이유는 생성한 모든층의 버텍스들이 0,0 기준으로 생성되기 때문에 층이 올라갈때마다 1/(30 * 2) 씩 증가 시켜야함 여기서 30은 층이 30개 이기 때문이고 * 2는 최대 증가 하는 값이 0.5,0.5를 넘어가지 않아야 하므로
                if (i == 0 || i == 1)//팔 쪽 부분 기준점을 다르게 하기 위해 
                {
                    vecPosX = (vertexPos[i].x - leftQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.x;//사각형 중심점 기준으로 축소시킨 후 다시 점 옮겨줌
                    vecPosY = (vertexPos[i].y - leftQuadCenterVec3.y) * (30f - j) / (30 * rawImageWidth) + reductionLeftQuad.y;
                    uvsVec2 = new Vector2(vecPosX + 0.5f, vecPosY + 0.5f);
                    inner_Uvs.Add(uvsVec2);
                }
                else if (i == 6 || i == 7)
                {
                    vecPosX = (vertexPos[i].x - rightQuadCenterVec3.x) * (30f - j) / (30 * rawImageWidth) + reductionRightQuad.x;//사각형 중심점 기준으로 축소시킨 후 다시 점 옮겨줌
                    vecPosY = (vertexPos[i].y - rightQuadCenterVec3.y) * (30f - j) / (30 * rawImageWidth) + reductionRightQuad.y;
                    uvsVec2 = new Vector2(vecPosX + 0.5f, vecPosY + 0.5f);
                    inner_Uvs.Add(uvsVec2);
                }
                else
                {
                    vecPosX = MoveOrigin_Vector3(vertexPos[i], leftBottom, rawImageWidth, false).x;
                    vecPosY = MoveOrigin_Vector3(vertexPos[i], leftBottom, rawImageWidth, false).y;
                    //기준점을 다시 옮겨야함.(rawImage의 왼쪽 모서리 부근으로)
                    uvsVec2 = new Vector2(vecPosX * (30f - j) / (30 * rawImageWidth) + 0.5f, vecPosY * (30f - j) / (30 * rawImageWidth) + 0.5f);
                    //uvsVec2 = new Vector2((vecPosX + rawImageWidth / 2f) * (30f - j) / (30f * rawImageWidth) + j / (30f * 2f), (vecPosY + rawImageWidth / 2f) * (30f - j) / (30f * rawImageWidth) + j / (30f * 2f));

                    inner_Uvs.Add(uvsVec2);
                }
            }
        }
        inner_Uvs.Add(CenterXY_Value(inner_Uvs[1 * 30 + 29], inner_Uvs[2 * 30 + 29], inner_Uvs[5 * 30 + 29], inner_Uvs[6 * 30 + 29]));
        mesh.uv = inner_Uvs.ToArray();
        NormalsInitialize(mesh, normalDirection);//노멀 초기화
        foundation.GetComponent<MeshRenderer>().material.mainTexture = image;

    }
    void NormalsInitialize(Mesh mesh_, int normalValue)//유니티 에서는 버텍스 기준으로 노멀 값을 설정한다. 즉 버텍스 개수와 같다. 노멀값은 버텍스에 인접한 폴리곤들의 평균 노멀값을 넣는다.
    {
        mesh_.RecalculateNormals();
        Vector3[] normals = mesh_.normals;
        Debug.Log("노멀 개수" + normals.Length);//버텍스 개수와 같다.

        for (int i = 0; i < normals.Length; i++)//노멀값 변경
        {
            if (i % 30 == 0)//첫번째 층의 버텍스의 경우
            {
                int value = i / 30;
                if (value == 7 || value == 8)//오른쪽 어깨부분
                    normals[i] = new Vector3(1, 1, 0);
                else if (value == 0 || value == 14)//왼쪽 어깨부분
                    normals[i] = new Vector3(-1, 1, 0);
                else if (value == 13 || value == 12 || value == 11 || value == 10 || value == 9)//목부분
                    normals[i] = new Vector3(0, 1, 0);
                else if (value == 1 || value == 2 || value == 3)//왼쪽 소매 아래부분 or 왼쪽아래 부분
                    normals[i] = new Vector3(-1, -1, 0);
                else if (value == 5 || value == 6 || value == 4)//오른쪽 소매 아래부분 or 오른쪽아래 부분
                    normals[i] = new Vector3(1, -1, 0);

            }
            else
                normals[i] = new Vector3(0, 0, normalValue);// 빛이 (0,0,-normalValue)방향으로 진행할때 빛을 반사시킨다.
        }
        mesh_.normals = normals;
    }
    Vector2 CenterXY_Value(Vector2 uvs1, Vector2 uvs2, Vector2 uvs5, Vector2 uvs6)//소매 쪽의 아랫부분을 기준으로 교차점을 구한다.
    {
        Vector2 lineA = uvs2 - uvs1;
        Vector2 lineB = uvs6 - uvs5;

        float denominator = (lineB.y * lineA.x) - (lineB.x * lineA.y);//분모

        if (denominator == 0)
        {
            // 직선 A와 B가 평행하므로 교차점이 없음
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
    Vector3 CenterBenchmark_Vector3(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4) //사각형의 중심점
    {
        float x = (vec1.x + vec2.x + vec3.x + vec4.x) / 4f;
        float y = (vec1.y + vec2.y + vec3.y + vec4.y) / 4f;
        float z = (vec1.z + vec2.x + vec3.x + vec4.x) / 4f;
        Vector3 centerVec = new Vector3(x, y, z);
        return centerVec;
    }
    Vector3 MoveOrigin_Vector3(Vector3 movedVertix, Transform rawImageLeft, float rawImageWidth, bool isReducing) //원점 기준으로 옮기기 및 축소
    {
        if (isReducing == true)
            return new Vector3((movedVertix.x - rawImageLeft.position.x - rawImageWidth / 2f) / rawImageWidth, (movedVertix.y - rawImageLeft.position.y - rawImageWidth / 2f) / rawImageWidth, 0f);
        else
            return new Vector3((movedVertix.x - rawImageLeft.position.x - rawImageWidth / 2f), (movedVertix.y - rawImageLeft.position.y - rawImageWidth / 2f), 0f);
    }
}
