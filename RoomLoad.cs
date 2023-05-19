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
    void Start()//문제가 생길 시 프로젝트 설정에서 loadjson스크립트 먼저 실행되도록 한다.
    {
        objectsSpace = BaseScene_OverallManager.objCreationSpace;
        GameObject currentRoom = GameObject.FindWithTag("Room");//오브젝트가 없는 경우 null 값
        if(currentRoom != null)
            roomName = currentRoom.name;
        room1.onClick.AddListener(RoomSelect);
        room2.onClick.AddListener(RoomSelect);
    }
    public void RoomSelect()//나중에 room개수가 많아지면 한 메소드로 관리하기 위해서
    {
        GameObject currentRoom = GameObject.FindWithTag("Room");
        if (currentRoom != null)
            Destroy(currentRoom);
        Transform currentObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform;
        roomName = currentObject.name;
        if(roomName == "Room1")//버튼 이름 Room1
        {
            Transform room = Instantiate(room1Prefab, objectsSpace);
            room.name = "Room1";
        }
            
        else if(roomName == "Room2")//버튼 이름 Room2
        {
            Transform room = Instantiate(room2Prefab, objectsSpace);
            room.name = "Room2";
        }
        currentObject.parent.gameObject.SetActive(false);
    }
}
