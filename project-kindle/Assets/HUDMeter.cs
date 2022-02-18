using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMeter : MonoBehaviour
{
    public Color metercolor;
    public Color backcolor;

    [SerializeField] private int value;    // Current value for meter
    private int valueprovisional;    // White health to show change
    [SerializeField] private int valuemax; // Max value for meter

    private float provisionalstep;
    private float provisionaltime = 80.0f;

    [SerializeField] private GameObject meterobj; 
    [SerializeField] private GameObject meterbackobj; 
    [SerializeField] private GameObject metermaskobj; 
    [SerializeField] private GameObject provisionalobj; 
    [SerializeField] private GameObject provisionalmaskobj; 
    [SerializeField] private GameObject numeratorobj; 
    [SerializeField] private GameObject denominatorobj;

    private float spritewidth;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(metercolor);
        meterobj.GetComponent<UnityEngine.UI.Image>().color = metercolor;
        meterbackobj.GetComponent<UnityEngine.UI.Image>().color = backcolor;

        spritewidth = meterobj.GetComponent<UnityEngine.UI.Image>().sprite.rect.width;
        spritewidth -= 15f; // Correct for diagonal part of sprite
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("h")) {AddValue(1);}
        if (Input.GetKeyDown("j")) {AddValue(-5);}
        //AddValue(-1);

        // Sync up to real value if provisional is low
        if (valueprovisional < value)
        {
            valueprovisional = value;
            provisionalstep = 0.0f;
        }
        // Provisional is greater than real value...
        else if (valueprovisional > value)
        {
            // Progress timer
            if (provisionalstep > 0.0f)
            {
                provisionalstep -= 1.0f;
            }
            // Decrement provisional value to real value
            else
            {
                valueprovisional -= 1;
            }
        }

        UpdateMeterDisplay();
    }

    // Updates values
    void SetValue(int _value)
    {
        if (_value < 0) {value = 0;}
        else if (_value > valuemax) {value = valuemax;}
        else {value = _value;}

        provisionalstep = provisionaltime;
    }
    void SetValueMax(int _value) {valuemax = _value < 0? 0: value;}
    void AddValue(int _value) {SetValue(value+_value);}
    void AddValueMax(int _value) {SetValueMax(valuemax+_value);}

    // Updates mask graphic and text to represent value
    void UpdateMeterDisplay()
    {
        // Move meter objs
        var xx = spritewidth * (1.0f - (float)value/(float)valuemax);
        var xx2 = spritewidth * (1.0f - (float)valueprovisional/(float)valuemax);
        
        // Real value
        metermaskobj.transform.localPosition = new Vector2(-xx, 0.0f);  // Move mask left
        meterobj.transform.localPosition = new Vector2(xx, 0.0f);   // Move sprite right to correct for parenting

        // Provisional value
        provisionalmaskobj.transform.localPosition = new Vector2(-xx2, 0.0f);  // Move mask left
        provisionalobj.transform.localPosition = new Vector2(xx2, 0.0f);   // Move sprite right to correct for parenting

        // Update text strings
        numeratorobj.GetComponent<UnityEngine.UI.Text>().text = value.ToString();
        denominatorobj.GetComponent<UnityEngine.UI.Text>().text = valuemax.ToString();
    }
}
