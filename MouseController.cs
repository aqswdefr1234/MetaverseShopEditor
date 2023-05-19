using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Transform currentObject;//선택된 오브젝트.인스펙터창에서 확인하기위해 [SerializeField] 선언
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject localAxis;
    // 카메라 이동 속도
    [SerializeField] private float moveSpeed = 50f;
    // 마우스 휠로 카메라 확대/축소 속도
    [SerializeField] private float mouseWheelSpeed = 1000f;
    // 마우스 드래그로 카메라 회전 속도
    [SerializeField] private float rotateSpeed = 500f;

    [SerializeField] private Material newMat;//마우스로 클릭시 outline생성하기 위해서

    //좌표계 드래그
    private Transform selectedAxis;
    private bool isLocalAxisControl = false;
    private Transform obj;//클릭한 대상 오브젝트
    private ObjectSceneData objData;

    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 newPos;

    void Update()
    {
        MouseLeftClick_3DObject();
        MouseWheelClick_Camera();
        MouseRightClick_Camera();
        MouseWheelScrollZoom();
        LocalAxisPressed();
    }
    void MouseLeftClick_3DObject()//마우스 왼쪽버튼 클릭
    {//콜라이더가 존재하는 것만 검출한다. 콜라이더 없는 것도 검출하려면 Physics.RaycastNonAlloc 사용해야한다.
        if (Input.GetMouseButtonDown(0))
        {
            if (currentObject != null)//currentObject가 null이 아니라는 것은 전에 "MovableObject"가 눌렸다는 뜻.Outline material이 들어가 있는 상태이다.
            {//그러므로 전의 outline머터리얼을 제거하기 위해 remove를 실행시키고,또 다시 "MovableObject"가 아닌 오브젝트가 눌리거나 아무것도 눌리지 않았을 때  currentObject의 머터리얼을 계속제거하지 않게 하기위해 null로 비운다.
                RemoveOutline(currentObject);
                currentObject = null;
            }
                
            int layerMask = LayerMask.GetMask("RayCastLayer");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //IsPointerOverGameObject()는 포인터 위에 ui가 있으면 true 없으면 false이다. 즉 ui가 위에 있으면 뒤의 오브젝트를 raycast로 감지하지 못하게된다.
            if (EventSystem.current.IsPointerOverGameObject() == false && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))//무한대로 설정해야 레이어가 아닌 오브젝트는 뚫고 지나간다.
            {//설정된 레이어에 부딪칠경우 레이캐스트가 통과하지 못한다. 방 오브젝트는 레이어를 설정하지 않고,
                if (hit.transform.tag == "MovableObject")
                {//정보를 확인할 오브젝트에는 RayCastLayer를 설정하면 방 밖에서 안쪽에 있는 오브젝트를 가져올 수 있게 된다.
                    currentObject = hit.transform;
                    AddOutline(currentObject);
                    BaseScene_OverallManager.selectedObjectTransform = hit.transform;
                    localAxis.SetActive(true);
                    localAxis.GetComponent<Arrow_Local>().SetNewPostion();
                }
                else if (hit.transform.tag == "SpotLight" || hit.transform.tag == "PointLight")
                {
                    currentObject = hit.transform;
                    BaseScene_OverallManager.selectedObjectTransform = hit.transform;
                    localAxis.SetActive(true);
                    localAxis.GetComponent<Arrow_Local>().SetNewPostion();
                }
                else if(hit.transform.tag == "Arrow")
                {
                    Debug.Log("Arrow");
                    if (BaseScene_OverallManager.selectedObjectTransform != null)
                    {
                        obj = BaseScene_OverallManager.selectedObjectTransform;
                        objData = obj.GetComponent<ObjectSceneData>();
                        selectedAxis = hit.transform;
                        isLocalAxisControl = true;
                        mZCoord = Camera.main.WorldToScreenPoint(obj.position).z;//여기서 Camera.main.WorldToScreenPoint는 월드 좌표계에서 스크린좌표로 바꾸는 메소드인데, 이것의 z값은 마우스가 클릭된 지점에서 3D 객체와 카메라 사이의 거리를 나타낸다.
                        mOffset = obj.position - GetMouseWorldPos();
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (obj != null && isLocalAxisControl == true)
            {
                Debug.Log("버튼 업");
                objData.TransformChanged();
                BaseScene_OverallManager.objectPosition = obj.position;
            }
            isLocalAxisControl = false;
        }
            
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    void LocalAxisPressed()
    {
        if(isLocalAxisControl == true)
        {
            if (selectedAxis.name == "ArrowX")
            {
                newPos = GetMouseWorldPos() + mOffset;
                obj.position = new Vector3(newPos.x, obj.position.y, obj.position.z);
            }
            else if (selectedAxis.name == "ArrowY")
            {
                newPos = GetMouseWorldPos() + mOffset;
                obj.position = new Vector3(obj.position.x, newPos.y, obj.position.z);
            }
            else if (selectedAxis.name == "ArrowZ")
            {
                newPos = GetMouseWorldPos() + mOffset;
                obj.position = new Vector3(obj.position.x, obj.position.y, newPos.z);
            }
            localAxis.transform.position = new Vector3(obj.position.x, obj.position.y, obj.position.z);
        }
    }
    void MouseWheelClick_Camera()//휠 누르고있으면
    {
        if (Input.GetMouseButton(2))//Transform.Translate은 로컬좌표값이 변하는 것이므로 
        {//아래서 z값을 0으로 두더라도 월드 좌표 값은 z값도 변화할수있다.forward도 마찬가지이다.
            float horizontal = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
            cameraTransform.Translate(new Vector3(-horizontal, -vertical, 0));
        }
    }
    void MouseRightClick_Camera()// 마우스 오른쪽 버튼 클릭. 카메라 회전
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
            cameraTransform.Rotate(Vector3.up, mouseX, Space.World);
            cameraTransform.Rotate(Vector3.right, -mouseY, Space.Self);
        }
    }
    void MouseWheelScrollZoom()// 카메라 확대/축소
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * mouseWheelSpeed * Time.deltaTime;
        cameraTransform.Translate(new Vector3(0, 0, scroll), Space.Self);
    }
    void AddOutline(Transform trans)
    {
        // 현재 GameObject의 Renderer 컴포넌트 가져오기
        MeshRenderer renderer = trans.GetComponent<MeshRenderer>();
        // 새로운 Material 추가
        Material[] newMaterials = new Material[renderer.materials.Length + 1];
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            newMaterials[i] = renderer.materials[i];
        }
        newMaterials[newMaterials.Length - 1] = newMat;

        // 새로운 Material 배열을 Renderer 컴포넌트에 할당
        renderer.materials = newMaterials;
    }
    void RemoveOutline(Transform trans)//material을 제거를 하려고하였지만 정상적으로 이루어지지 않았다.
    {   //따라서 새로운 material[]에, 기존의 마지막 인덱스 material을 제거한 배열을 넣고 그 material[]을 오브젝트에 할당한다.
        MeshRenderer renderer = trans.GetComponent<MeshRenderer>();
        Material[] newMaterials = new Material[renderer.materials.Length - 1]; //마지막 인덱스 제거하기 위해서 -1
        for (int i = 0; i < renderer.materials.Length - 1; i++)
        {
            newMaterials[i] = renderer.materials[i];
        }
        renderer.materials = newMaterials;
    }
}
//https://bloodstrawberry.tistory.com/884 : ui요소 아래 오브젝트 레이케스트 감지하지 못하게하는 방법.
