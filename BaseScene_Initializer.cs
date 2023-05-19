using UnityEngine;

public class BaseScene_Initializer : MonoBehaviour//시작시 변수들을 초기화 하거나 값을 할당해주는 스크립트
{
    [SerializeField] private Transform objectsSpace;
    void Awake()
    {
        Debug.Log("awake실행");
        BaseScene_OverallManager.objCreationSpace = objectsSpace;
        BaseScene_OverallManager.selectedObjectTransform = null;
    }
}
