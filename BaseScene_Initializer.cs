using UnityEngine;

public class BaseScene_Initializer : MonoBehaviour//���۽� �������� �ʱ�ȭ �ϰų� ���� �Ҵ����ִ� ��ũ��Ʈ
{
    [SerializeField] private Transform objectsSpace;
    void Awake()
    {
        Debug.Log("awake����");
        BaseScene_OverallManager.objCreationSpace = objectsSpace;
        BaseScene_OverallManager.selectedObjectTransform = null;
    }
}
