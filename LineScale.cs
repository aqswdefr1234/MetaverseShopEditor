using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScale : MonoBehaviour
{
    [SerializeField] private RectTransform startVertex;//인스펙터 창에서 직접 넣진 않는다. 인스펙터 창에서 확인하기 위해서 시리얼라이즈 필드 선언해준 것.
    [SerializeField] private RectTransform endVertex;
    int myNumber;
    int VertexCount;
    Vector2 startAnchored = new Vector2();
    Vector2 endAnchored = new Vector2();

    void Start()//ui의 트랜스폼 포지션을 가져오거나 렉트랜스폼 포지션을 가져오면 기본적으로 스크린 좌표로 계산된다.
    {
        myNumber = Convert.ToInt32(gameObject.name);
        VertexCount = GameObject.Find("UV_UI").GetComponent<DragVertex>().VertexCount;
        Debug.Log(VertexCount);
        if (myNumber != VertexCount - 1)//마지막 버텍스를 참조하지 않는다면
        {
            startVertex = GameObject.Find("Vertex (" + myNumber + ")").GetComponent<RectTransform>(); //버텍스 오브젝트 찾아 RectTransform 컴포넌트 연결시키기
            endVertex = GameObject.Find("Vertex (" + (myNumber + 1) + ")").GetComponent<RectTransform>();
            startAnchored = startVertex.anchoredPosition;
            endAnchored = endVertex.anchoredPosition;
        }
        else//마지막 버텍스를 참조한다면
        {
            startVertex = GameObject.Find("Vertex (" + myNumber + ")").GetComponent<RectTransform>(); //버텍스 오브젝트 찾아 RectTransform 컴포넌트 연결시키기
            endVertex = GameObject.Find("Vertex (0)").GetComponent<RectTransform>();
            startAnchored = startVertex.anchoredPosition;
            endAnchored = endVertex.anchoredPosition;
        }
        StartCoroutine(LineCoroutine());
    }
    IEnumerator LineCoroutine()
    {
        while (true)
        {
            transform.position = startVertex.position;
            LineMove();
            yield return new WaitForSeconds(0.1f);
        }
    }
    void LineMove()
    {
        startAnchored = startVertex.anchoredPosition;
        endAnchored = endVertex.anchoredPosition;
        Vector2 differenceValue = new Vector2();
        differenceValue = endAnchored - startAnchored;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(10, differenceValue.magnitude);

        float degree = Mathf.Atan2(differenceValue.x, differenceValue.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, -degree);//transform.Rotate 사용하면 함수가 한번돌때마다 증감하므로 rotation을 사용해야한다. 짐벌락을 피하기 위해서 Quaternion.Euler를 사용한다.
    }
}
