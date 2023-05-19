using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Transform currentObject;//���õ� ������Ʈ.�ν�����â���� Ȯ���ϱ����� [SerializeField] ����
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject localAxis;
    // ī�޶� �̵� �ӵ�
    [SerializeField] private float moveSpeed = 50f;
    // ���콺 �ٷ� ī�޶� Ȯ��/��� �ӵ�
    [SerializeField] private float mouseWheelSpeed = 1000f;
    // ���콺 �巡�׷� ī�޶� ȸ�� �ӵ�
    [SerializeField] private float rotateSpeed = 500f;

    [SerializeField] private Material newMat;//���콺�� Ŭ���� outline�����ϱ� ���ؼ�

    //��ǥ�� �巡��
    private Transform selectedAxis;
    private bool isLocalAxisControl = false;
    private Transform obj;//Ŭ���� ��� ������Ʈ
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
    void MouseLeftClick_3DObject()//���콺 ���ʹ�ư Ŭ��
    {//�ݶ��̴��� �����ϴ� �͸� �����Ѵ�. �ݶ��̴� ���� �͵� �����Ϸ��� Physics.RaycastNonAlloc ����ؾ��Ѵ�.
        if (Input.GetMouseButtonDown(0))
        {
            if (currentObject != null)//currentObject�� null�� �ƴ϶�� ���� ���� "MovableObject"�� ���ȴٴ� ��.Outline material�� �� �ִ� �����̴�.
            {//�׷��Ƿ� ���� outline���͸����� �����ϱ� ���� remove�� �����Ű��,�� �ٽ� "MovableObject"�� �ƴ� ������Ʈ�� �����ų� �ƹ��͵� ������ �ʾ��� ��  currentObject�� ���͸����� ����������� �ʰ� �ϱ����� null�� ����.
                RemoveOutline(currentObject);
                currentObject = null;
            }
                
            int layerMask = LayerMask.GetMask("RayCastLayer");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //IsPointerOverGameObject()�� ������ ���� ui�� ������ true ������ false�̴�. �� ui�� ���� ������ ���� ������Ʈ�� raycast�� �������� ���ϰԵȴ�.
            if (EventSystem.current.IsPointerOverGameObject() == false && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))//���Ѵ�� �����ؾ� ���̾ �ƴ� ������Ʈ�� �հ� ��������.
            {//������ ���̾ �ε�ĥ��� ����ĳ��Ʈ�� ������� ���Ѵ�. �� ������Ʈ�� ���̾ �������� �ʰ�,
                if (hit.transform.tag == "MovableObject")
                {//������ Ȯ���� ������Ʈ���� RayCastLayer�� �����ϸ� �� �ۿ��� ���ʿ� �ִ� ������Ʈ�� ������ �� �ְ� �ȴ�.
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
                        mZCoord = Camera.main.WorldToScreenPoint(obj.position).z;//���⼭ Camera.main.WorldToScreenPoint�� ���� ��ǥ�迡�� ��ũ����ǥ�� �ٲٴ� �޼ҵ��ε�, �̰��� z���� ���콺�� Ŭ���� �������� 3D ��ü�� ī�޶� ������ �Ÿ��� ��Ÿ����.
                        mOffset = obj.position - GetMouseWorldPos();
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (obj != null && isLocalAxisControl == true)
            {
                Debug.Log("��ư ��");
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
    void MouseWheelClick_Camera()//�� ������������
    {
        if (Input.GetMouseButton(2))//Transform.Translate�� ������ǥ���� ���ϴ� ���̹Ƿ� 
        {//�Ʒ��� z���� 0���� �δ��� ���� ��ǥ ���� z���� ��ȭ�Ҽ��ִ�.forward�� ���������̴�.
            float horizontal = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
            cameraTransform.Translate(new Vector3(-horizontal, -vertical, 0));
        }
    }
    void MouseRightClick_Camera()// ���콺 ������ ��ư Ŭ��. ī�޶� ȸ��
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
            cameraTransform.Rotate(Vector3.up, mouseX, Space.World);
            cameraTransform.Rotate(Vector3.right, -mouseY, Space.Self);
        }
    }
    void MouseWheelScrollZoom()// ī�޶� Ȯ��/���
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * mouseWheelSpeed * Time.deltaTime;
        cameraTransform.Translate(new Vector3(0, 0, scroll), Space.Self);
    }
    void AddOutline(Transform trans)
    {
        // ���� GameObject�� Renderer ������Ʈ ��������
        MeshRenderer renderer = trans.GetComponent<MeshRenderer>();
        // ���ο� Material �߰�
        Material[] newMaterials = new Material[renderer.materials.Length + 1];
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            newMaterials[i] = renderer.materials[i];
        }
        newMaterials[newMaterials.Length - 1] = newMat;

        // ���ο� Material �迭�� Renderer ������Ʈ�� �Ҵ�
        renderer.materials = newMaterials;
    }
    void RemoveOutline(Transform trans)//material�� ���Ÿ� �Ϸ����Ͽ����� ���������� �̷������ �ʾҴ�.
    {   //���� ���ο� material[]��, ������ ������ �ε��� material�� ������ �迭�� �ְ� �� material[]�� ������Ʈ�� �Ҵ��Ѵ�.
        MeshRenderer renderer = trans.GetComponent<MeshRenderer>();
        Material[] newMaterials = new Material[renderer.materials.Length - 1]; //������ �ε��� �����ϱ� ���ؼ� -1
        for (int i = 0; i < renderer.materials.Length - 1; i++)
        {
            newMaterials[i] = renderer.materials[i];
        }
        renderer.materials = newMaterials;
    }
}
//https://bloodstrawberry.tistory.com/884 : ui��� �Ʒ� ������Ʈ �����ɽ�Ʈ �������� ���ϰ��ϴ� ���.
