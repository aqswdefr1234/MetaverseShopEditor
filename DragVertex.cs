using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragVertex : MonoBehaviour
{//https://docs.unity3d.com/ScriptReference/Input-mousePosition.html
    
    [SerializeField] private GameObject lineImage;//���� ������
    public int VertexCount = -1;//�ν����� â���� ���� ���� ���ؽ��� ������ �����Ѵ�.LineScale ��ũ��Ʈ�� ���� ������ �ϹǷ� public����

    GameObject[] imageLineClone;//Ŭ��
    Transform currentObject;//���� ������Ʈ Ʈ������
    int currentObjectNumber = -1;//���� ���õ� ���ؽ��� �ѹ�

    void Start()
    {
        imageLineClone = new GameObject[VertexCount];
        for (int i = 0; i < imageLineClone.Length; i++)//Ŭ�� ����
        {
            imageLineClone[i] = Instantiate(lineImage, transform);//UV_UI������Ʈ �Ʒ��� ����
            imageLineClone[i].transform.SetSiblingIndex(0);//ui ��¿켱������ ���Ѵ�.0�� ���� ���� ��µȴ�.(���� �Ʒ��� ��ġ)
            imageLineClone[i].name = i.ToString();
        }
    }
    
    public void DownMouse()
    {
        currentObject = EventSystem.current.currentSelectedGameObject.transform;//��ũ�� ������
        Debug.Log(currentObject.name);
        currentObjectNumber = Convert.ToInt32(currentObject.name[8].ToString());//currentObject.name[8] �� ����ϸ� char�����̹Ƿ� ��Ʈȭ������ �� �ƽ�Ű �ڵ尪���� ���´�.�׷��� ���ڿ� ȭ ���Ѿ��Ѵ�.
        Debug.Log(currentObject.name[8]);
    }
    public void Draging(BaseEventData data)//������ ���� �۵���
    {
        PointerEventData pointer = (PointerEventData)data;
        currentObject.position = pointer.position;//��ũ�� �������̴�.
        Debug.Log(currentObject.position);
    }
}
