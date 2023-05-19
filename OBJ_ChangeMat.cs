using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class OBJ_ChangeMat : MonoBehaviour
{
    private Transform selectedTransform;//다른 스크립트에서 할당해줘야함.
    public Slider hueSlider;// 색상
    public Slider saturationSlider;//채도
    public Slider valueSlider;//명도

    [SerializeField] private TMP_InputField r;
    [SerializeField] private TMP_InputField g;
    [SerializeField] private TMP_InputField b;
    [SerializeField] private Button rgbBtn;
    [SerializeField] private GameObject rgbPanel;

    [SerializeField] private Image saturationSliderBackground;
    [SerializeField] private MeshRenderer myRenderer;
    private float h, s, v;
    private int red = 255, green = 255, blue = 255;
    private int rgbBtnCount = 0;

    void Start()
    {
        OBJScene_DataRepository.OnChildTransformChanged += HandleChildTransformChanged;
        OBJScene_DataRepository.OnChildColorChanged += HandleChildColor;
        saturationSliderBackground = saturationSlider.transform.GetChild(0).GetChild(1).GetComponent<Image>();//background의 right부분

        hueSlider.minValue = 0f;
        hueSlider.maxValue = 1f;
        saturationSlider.minValue = 0f;
        saturationSlider.maxValue = 1f;
        valueSlider.minValue = 0f;
        valueSlider.maxValue = 1f;

        r.onValueChanged.AddListener(OnValueChanged);
        g.onValueChanged.AddListener(OnValueChanged);
        b.onValueChanged.AddListener(OnValueChanged);
        rgbBtn.onClick.AddListener(RGB_ActiveBtn);

        StartCoroutine(MaterialChange());
    }
    void HandleChildTransformChanged(Transform newTransform)
    {
        selectedTransform = newTransform;
        myRenderer = selectedTransform.GetComponent<MeshRenderer>();
        Color.RGBToHSV(myRenderer.material.color, out h, out s, out v);
        SliderValueSet(h, s, v);
        Color32 myColor32 = myRenderer.material.color;
        r.text = myColor32.r.ToString();
        g.text = myColor32.g.ToString();
        b.text = myColor32.b.ToString();
    }
    void HandleChildColor(Color newColor)
    {
        myRenderer.material.color = newColor;
        saturationSliderBackground.color = Color.HSVToRGB(hueSlider.value, 1f, 1f);
    }
    IEnumerator MaterialChange()
    {
        GameObject currentSelectedObject = null;
        WaitForSeconds delay = new WaitForSeconds(0.2f);
        string currentUIName = "null";
        Color currentColor = new Color(1f, 1f, 1f, 1f);
        Color32 currentColor32 = new Color32(255, 255, 255, 255); ;
        Vector3 sliderValue = new Vector3(1f, 1f, 1f);
        while (true)
        {
            currentSelectedObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedObject != null)
            {
                currentUIName = currentSelectedObject.transform.name;
                if (currentUIName == "R" || currentUIName == "G" || currentUIName == "B")
                {
                    currentColor32.r = (byte)red;
                    currentColor32.g = (byte)green;
                    currentColor32.b = (byte)blue;
                    currentColor = currentColor32;
                    OBJScene_DataRepository.CurrentOBJChildColor = currentColor;
                    Debug.Log("currentColor" + currentColor);
                    Color.RGBToHSV(currentColor, out h, out s, out v);
                    SliderValueSet(h, s, v);
                }
                else if (currentUIName == "HueSlider" || currentUIName == "SaturationSlider" || currentUIName == "ValueSlider")
                {
                    sliderValue = SliderValueGet();
                    currentColor = Color.HSVToRGB(sliderValue.x, sliderValue.y, sliderValue.z);
                    OBJScene_DataRepository.CurrentOBJChildColor = currentColor;
                    currentColor32 = currentColor;
                    r.text = currentColor32.r.ToString();
                    g.text = currentColor32.g.ToString();
                    b.text = currentColor32.b.ToString();
                    red = currentColor32.r;
                    green = currentColor32.g;
                    blue = currentColor32.b;
                }
            }
            yield return delay;
        }
    }
    private Vector3 SliderValueGet()
    {
        Vector3 sliderValues = new Vector3(hueSlider.value, saturationSlider.value, valueSlider.value);
        return sliderValues;
    }
    private void SliderValueSet(float hue_val, float saturation_val, float value_val)
    {
        hueSlider.value = hue_val;
        saturationSlider.value = saturation_val;
        valueSlider.value = value_val;
    }
    private void OnValueChanged(string newVal)//비활성화 된 상태에서는 작동하지 않는다.
    {
        string currentInputFieldName = EventSystem.current.currentSelectedGameObject.transform.name;
        TMP_InputField currentField = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
        int intValue = 0;
        Debug.Log("newVal : " + newVal);
        if (int.TryParse(newVal, out intValue))
        {
            if(0 <= intValue && intValue < 256)
            {
                if (currentInputFieldName == "R")
                {
                    Debug.Log("R");
                    red = intValue;
                }
                else if (currentInputFieldName == "G")
                {
                    green = intValue;
                }
                else if (currentInputFieldName == "B")
                {
                    blue = intValue;
                }
            }
            else if(0 > intValue)
            {
                currentField.text = "0";
            }
            else if (255 < intValue)
            {
                currentField.text = "255";
            }
        }
        else if (newVal == "")
        {
            Debug.Log("...");
        }
        else
        {
            currentField.text = "0";
        }
    }
    private void RGB_ActiveBtn()
    {
        if (rgbBtnCount % 2 == 0)
            rgbPanel.SetActive(true);
        else
        {
            rgbPanel.SetActive(false);
        }
        rgbBtnCount++;
    }
}
