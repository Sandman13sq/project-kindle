using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMeter : MonoBehaviour
{
    public Color metercolor;
    public Color backcolor;

    [SerializeField] private int metervalue;    // Current value for meter
    private int provisionalvalue;    // White health to show change
    [SerializeField] private int metermax; // Max value for meter

    private float provisionalstep;
    private float provisionaltime = 80.0f;

    [SerializeField] private GameObject meterobj; 
    [SerializeField] private GameObject meterbackobj; 
    [SerializeField] private GameObject metermaskobj; 
    [SerializeField] private GameObject provisionalobj; 
    [SerializeField] private GameObject provisionalmaskobj; 
    [SerializeField] private GameObject numeratorobj; 
    [SerializeField] private GameObject denominatorobj;

    private float flashstep = 0.0f;
    private float flashtime = 20.0f;

    private float spritewidth;

    // Start is called before the first frame update
    void Start()
    {
        meterobj.GetComponent<UnityEngine.UI.Image>().color = metercolor;
        meterbackobj.GetComponent<UnityEngine.UI.Image>().color = backcolor;

        spritewidth = meterobj.GetComponent<UnityEngine.UI.Image>().sprite.rect.width;
        spritewidth -= 15f; // Correct for diagonal part of sprite
    }

    // Update is called once per frame
    void Update()
    {
        // Sync up to real value if provisional is low
        if (provisionalvalue < metervalue)
        {
            provisionalvalue = metervalue;
            provisionalstep = 0.0f;
        }
        // Provisional is greater than real value...
        else if (provisionalvalue > metervalue)
        {
            // Progress timer
            if (provisionalstep > 0.0f)
            {
                provisionalstep -= 1.0f;
            }
            // Decrement provisional value to real value
            else
            {
                provisionalvalue -= 1;
            }
        }

        if (flashstep > 0.0f) 
        {
            flashstep = Mathf.Max(flashstep - 1.0f, 0.0f);

            if (flashstep > 0.0f && Mathf.Repeat(flashstep, 6.0f) < 3.0f)
            {
                meterobj.GetComponent<UnityEngine.UI.Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                meterobj.GetComponent<UnityEngine.UI.Image>().color = metercolor;
            }
        }

        UpdateMeterDisplay();
    }

    // Updates values
    public void SetValue(int _value, bool matchprovisional = false)
    {
        if (_value < 0) {metervalue = 0;}
        else if (_value > metermax) {metervalue = metermax;}
        else {metervalue = _value;}

        if (matchprovisional)
        {
            provisionalvalue = _value;
        }
        else
        {
            provisionalstep = provisionaltime;
        }
        
        UpdateMeterDisplay();
    }

    public void Flash()
    {
        flashstep = flashtime;
    }

    public void SetValueMax(int _value) 
    {
        metermax = (_value < 0)? 0: _value; 
        UpdateMeterDisplay();
    }

    public void MaximizeProvisional()
    {
        provisionalvalue = metermax;
        provisionalstep = provisionaltime;
    }

    // Updates mask graphic and text to represent value
    protected void UpdateMeterDisplay()
    {
        if (metermax == 0) {return;}

        // Move meter objs
        var xx = spritewidth * (1.0f - (float)metervalue/(float)metermax);
        var xx2 = spritewidth * (1.0f - (float)provisionalvalue/(float)metermax);
        
        // Real value
        metermaskobj.transform.localPosition = new Vector2(-xx, 0.0f);  // Move mask left
        meterobj.transform.localPosition = new Vector2(xx, 0.0f);   // Move sprite right to correct for parenting

        // Provisional value
        provisionalmaskobj.transform.localPosition = new Vector2(-xx2, 0.0f);  // Move mask left
        provisionalobj.transform.localPosition = new Vector2(xx2, 0.0f);   // Move sprite right to correct for parenting

        // Update text strings
        numeratorobj.GetComponent<UnityEngine.UI.Text>().text = metervalue.ToString();
        denominatorobj.GetComponent<UnityEngine.UI.Text>().text = metermax.ToString();
    }
}
