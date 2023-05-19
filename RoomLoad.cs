using UnityEngine;
using UnityEngine.UI;

public class RoomLoad : MonoBehaviour
{
    public string roomName;

    [SerializeField] private Button room1;
    [SerializeField] private Button room2;
    [SerializeField] private Transform room1Prefab;
    [SerializeField] private Transform room2Prefab;

    private Transform objectsSpace;
    void Start()//������ ���� �� ������Ʈ �������� loadjson��ũ��Ʈ ���� ����ǵ��� �Ѵ�.
    {
        objectsSpace = BaseScene_OverallManager.objCreationSpace;
        GameObject currentRoom = GameObject.FindWithTag("Room");//������Ʈ�� ���� ��� null ��
        if(currentRoom != null)
            roomName = currentRoom.name;
        room1.onClick.AddListener(RoomSelect);
        room2.onClick.AddListener(RoomSelect);
    }
    public void RoomSelect()//���߿� room������ �������� �� �޼ҵ�� �����ϱ� ���ؼ�
    {
        GameObject currentRoom = GameObject.FindWithTag("Room");
        if (currentRoom != null)
            Destroy(currentRoom);
        Transform currentObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform;
        roomName = currentObject.name;
        if(roomName == "Room1")//��ư �̸� Room1
        {
            Transform room = Instantiate(room1Prefab, objectsSpace);
            room.name = "Room1";
        }
            
        else if(roomName == "Room2")//��ư �̸� Room2
        {
            Transform room = Instantiate(room2Prefab, objectsSpace);
            room.name = "Room2";
        }
        currentObject.parent.gameObject.SetActive(false);
    }
}
