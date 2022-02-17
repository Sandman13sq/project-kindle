using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMeter : MonoBehaviour
{
    public Color metercolor;
    public Color backcolor;

    [SerializeField] private int value;    // Current value for meter
    [SerializeField] private int valuemax; // Max value for meter

    [SerializeField] private GameObject meterobj; 
    [SerializeField] private GameObject meterbackobj; 
    [SerializeField] private GameObject metermaskobj; 
    [SerializeField] private GameObject numeratorobj; 
    [SerializeField] private GameObject denominatorobj;

    private float spritewidth = 128.0f;   // TODO: Update to take sprite.width or something

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(metercolor);
        meterobj.GetComponent<UnityEngine.UI.Image>().color = metercolor;
        meterbackobj.GetComponent<UnityEngine.UI.Image>().color = backcolor;
    }

    // Update is called once per frame
    void Update()
    {
        //AddValue(-1);
        UpdateMeterDisplay();
    }

    // Updates values
    void SetValue(int _value)
    {
        if (_value < 0) {value = 0;}
        else if (_value > valuemax) {value = valuemax;}
        else {value = _value;}
    }
    void SetValueMax(int _value) {valuemax = _value < 0? 0: value;}
    void AddValue(int _value) {SetValue(value+_value);}
    void AddValueMax(int _value) {SetValueMax(valuemax+_value);}

    // Updates mask graphic and text to represent value
    void UpdateMeterDisplay()
    {
        RectTransform rect = metermaskobj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2( (float)value/(float)valuemax * spritewidth, rect.sizeDelta.y);

        numeratorobj.GetComponent<UnityEngine.UI.Text>().text = value.ToString();
        denominatorobj.GetComponent<UnityEngine.UI.Text>().text = valuemax.ToString();
    }
}
