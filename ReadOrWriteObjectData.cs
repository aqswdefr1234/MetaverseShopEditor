using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ReadOrWriteObjectData : MonoBehaviour//ObjectDataVisiblePanel와 세트
{
    [SerializeField] private TMP_InputField objectName;
    [SerializeField] private GameObject objectDataVisiblePanel;
    [SerializeField] private Transform positionTransform;
    [SerializeField] private Transform rotationTransform;
    [SerializeField] private Transform scaleTransform;
    [SerializeField] private GameObject localAxis;

    private Transform currentObjectTransform;//트랜스폼 변경 이벤트 발생시 변한 트랜스폼 넣어줌

    private TMP_InputField[] positionInputField = new TMP_InputField[3];
    private TMP_InputField[] rotateInputField = new TMP_InputField[3];
    private TMP_InputField[] scaleInputField = new TMP_InputField[3];
    
    private float preVal = 0f;
    private GameObject currentUIObject;
    private ObjectSceneData osd;

    

    void Start()
    {
        BaseScene_OverallManager.OnSelectTransformChanged += ClickEventHandle;
        BaseScene_OverallManager.OnPositionChanged += ObjectPositionChangeHandle;
        AddListener();
    }
    private void ClickEventHandle(Transform newTransform)
    {
        currentObjectTransform = newTransform;
        Debug.Log(currentObjectTransform);
        WriteObjectData();
        objectDataVisiblePanel.SetActive(true);
    }
    private void ObjectPositionChangeHandle(Vector3 newPosition)
    {
        if(currentObjectTransform != null)
        {
            positionInputField[0].text = newPosition.x.ToString();
            positionInputField[1].text = newPosition.y.ToString();
            positionInputField[2].text = newPosition.z.ToString();
        }
    }
    public void WriteObjectData()//ui에 해당 오브젝트의 정보를 입력한다.
    {
        osd = currentObjectTransform.GetComponent<ObjectSceneData>();
        objectName.text = osd.myName;
        positionInputField[0].text = osd.myPosition.x.ToString();
        positionInputField[1].text = osd.myPosition.y.ToString();
        positionInputField[2].text = osd.myPosition.z.ToString();
        rotateInputField[0].text = osd.myRotation.x.ToString();
        rotateInputField[1].text = osd.myRotation.y.ToString();
        rotateInputField[2].text = osd.myRotation.z.ToString();
        scaleInputField[0].text = osd.myScale.x.ToString();
        scaleInputField[1].text = osd.myScale.y.ToString();
        scaleInputField[2].text = osd.myScale.z.ToString();
    }
    void AddListener()
    {
        for (int i = 0; i < 3; i++)
        {
            positionInputField[i] = positionTransform.GetChild(i).GetComponent<TMP_InputField>();
            rotateInputField[i] = rotationTransform.GetChild(i).GetComponent<TMP_InputField>();
            scaleInputField[i] = scaleTransform.GetChild(i).GetComponent<TMP_InputField>();

            positionInputField[i].onValueChanged.AddListener(OnValueChanged);
            rotateInputField[i].onValueChanged.AddListener(OnValueChanged);
            scaleInputField[i].onValueChanged.AddListener(OnValueChanged);

            positionInputField[i].onEndEdit.AddListener(OnEndEdit_IsZeroValue);
            rotateInputField[i].onEndEdit.AddListener(OnEndEdit_IsZeroValue);
            scaleInputField[i].onEndEdit.AddListener(OnEndEdit_IsZeroValue);

        }
        objectName.onEndEdit.AddListener(NameOnEndEdit);
    }
    void OnValueChanged(string newVal)
    {
        Debug.Log("newVal : " + newVal);
        float floatVal;
        currentUIObject = EventSystem.current.currentSelectedGameObject;
        // 입력된 값이 float 형식으로 변환 가능하면 floatValue에 저장
        if (currentUIObject != null && currentUIObject.GetComponent<TMP_InputField>().text != "" && newVal != "-")
        {
            if (float.TryParse(newVal, out floatVal))
            {
                // 입력된 값이 소수점 셋째 자리까지의 유효한 실수인지 검사
                if (Mathf.Ceil(floatVal * 1000f) == Mathf.Floor(floatVal * 1000f))//Mathf.Floor는 소수점 아래 버림
                {
                    preVal = floatVal;
                    Debug.Log("InputField value changed: " + preVal);
                    // 값이 변경될 때마다 호출되는 함수 실행
                    CorrectValueUpdated(preVal);
                    localAxis.GetComponent<Arrow_Local>().SetNewPostion();
                }
                // 소수점 첫째 자리 이하의 값이 입력된 경우 이전 값을 유지하도록 함
                else
                    currentUIObject.GetComponent<TMP_InputField>().text = preVal.ToString();
            }
            else
                currentUIObject.GetComponent<TMP_InputField>().text = preVal.ToString();
        }
        else if (currentUIObject != null && newVal == "-")
        {
            preVal = float.Parse("-0");
        }
    }
    private void CorrectValueUpdated(float value)
    {
        float vectorX = 0f, vectorY = 0f, vectorZ = 0f;
        if (currentUIObject != null && currentObjectTransform != null)
        {
            if (currentUIObject.transform.parent == positionTransform)
            {
                vectorX = float.Parse(positionInputField[0].text);
                vectorY = float.Parse(positionInputField[1].text);
                vectorZ = float.Parse(positionInputField[2].text);
                currentObjectTransform.position = new Vector3(vectorX, vectorY, vectorZ);
            }
            else if(currentUIObject.transform.parent == rotationTransform)
            {
                vectorX = float.Parse(rotateInputField[0].text);
                vectorY = float.Parse(rotateInputField[1].text);
                vectorZ = float.Parse(rotateInputField[2].text);
                currentObjectTransform.rotation = Quaternion.Euler(new Vector3(vectorX, vectorY, vectorZ));
            }
            else if (currentUIObject.transform.parent == scaleTransform)
            {
                vectorX = float.Parse(scaleInputField[0].text);
                vectorY = float.Parse(scaleInputField[1].text);
                vectorZ = float.Parse(scaleInputField[2].text);
                currentObjectTransform.localScale = new Vector3(vectorX, vectorY, vectorZ);
            }
        }
        if (osd != null)
        {
            osd.TransformChanged();
        }
    }
    private void OnEndEdit_IsZeroValue(string zeroVal)
    {
        if (zeroVal == "" || zeroVal == "-")
        {
            currentUIObject.GetComponent<TMP_InputField>().text = "0";
        }
    }
    void NameOnEndEdit(string newName)
    {
        if(newName != "" && currentObjectTransform != null)
        {
            currentObjectTransform.GetComponent<ObjectSceneData>().myName = newName;
            currentObjectTransform.name = newName;
        }
    }
    public void DeleteObjectButton()
    {
        if(currentObjectTransform != null)
        {
            Destroy(currentObjectTransform.gameObject);
            currentObjectTransform = null;
        }
    }
}
