using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScale : MonoBehaviour
{
    [SerializeField] private RectTransform startVertex;//�ν����� â���� ���� ���� �ʴ´�. �ν����� â���� Ȯ���ϱ� ���ؼ� �ø�������� �ʵ� �������� ��.
    [SerializeField] private RectTransform endVertex;
    int myNumber;
    int VertexCount;
    Vector2 startAnchored = new Vector2();
    Vector2 endAnchored = new Vector2();

    void Start()//ui�� Ʈ������ �������� �������ų� ��Ʈ������ �������� �������� �⺻������ ��ũ�� ��ǥ�� ���ȴ�.
    {
        myNumber = Convert.ToInt32(gameObject.name);
        VertexCount = GameObject.Find("UV_UI").GetComponent<DragVertex>().VertexCount;
        Debug.Log(VertexCount);
        if (myNumber != VertexCount - 1)//������ ���ؽ��� �������� �ʴ´ٸ�
        {
            startVertex = GameObject.Find("Vertex (" + myNumber + ")").GetComponent<RectTransform>(); //���ؽ� ������Ʈ ã�� RectTransform ������Ʈ �����Ű��
            endVertex = GameObject.Find("Vertex (" + (myNumber + 1) + ")").GetComponent<RectTransform>();
            startAnchored = startVertex.anchoredPosition;
            endAnchored = endVertex.anchoredPosition;
        }
        else//������ ���ؽ��� �����Ѵٸ�
        {
            startVertex = GameObject.Find("Vertex (" + myNumber + ")").GetComponent<RectTransform>(); //���ؽ� ������Ʈ ã�� RectTransform ������Ʈ �����Ű��
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
        transform.rotation = Quaternion.Euler(0, 0, -degree);//transform.Rotate ����ϸ� �Լ��� �ѹ��������� �����ϹǷ� rotation�� ����ؾ��Ѵ�. �������� ���ϱ� ���ؼ� Quaternion.Euler�� ����Ѵ�.
    }
}
