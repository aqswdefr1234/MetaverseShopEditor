using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragVertex : MonoBehaviour
{//https://docs.unity3d.com/ScriptReference/Input-mousePosition.html
    
    [SerializeField] private GameObject lineImage;//라인 프리팹
    public int VertexCount = -1;//인스펙터 창에서 현재 사용된 버텍스의 개수를 설정한다.LineScale 스크립트에 값을 보내야 하므로 public선언

    GameObject[] imageLineClone;//클론
    Transform currentObject;//누른 오브젝트 트랜스폼
    int currentObjectNumber = -1;//현재 선택된 버텍스의 넘버

    void Start()
    {
        imageLineClone = new GameObject[VertexCount];
        for (int i = 0; i < imageLineClone.Length; i++)//클론 생성
        {
            imageLineClone[i] = Instantiate(lineImage, transform);//UV_UI오브젝트 아래에 생성
            imageLineClone[i].transform.SetSiblingIndex(0);//ui 출력우선순위를 정한다.0이 가장 먼저 출력된다.(가장 아래에 위치)
            imageLineClone[i].name = i.ToString();
        }
    }
    
    public void DownMouse()
    {
        currentObject = EventSystem.current.currentSelectedGameObject.transform;//스크린 포지션
        Debug.Log(currentObject.name);
        currentObjectNumber = Convert.ToInt32(currentObject.name[8].ToString());//currentObject.name[8] 만 사용하면 char형식이므로 인트화시켰을 때 아스키 코드값으로 나온다.그래서 문자열 화 시켜야한다.
        Debug.Log(currentObject.name[8]);
    }
    public void Draging(BaseEventData data)//움직일 때만 작동됨
    {
        PointerEventData pointer = (PointerEventData)data;
        currentObject.position = pointer.position;//스크린 포지션이다.
        Debug.Log(currentObject.position);
    }
}
