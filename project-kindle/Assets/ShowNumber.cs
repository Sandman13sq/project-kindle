using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNumber : MonoBehaviour
{

    public GameObject targetentity;

    public UnityEngine.UI.Text textcomponent;
    public UnityEngine.UI.Text textdropcomponent;
    public int nodeindex = 0;

    public int value;
    public float flashstep;
    public float flashtime = 20.0f;

    private int lifemax = 120;
    private int life;

    public enum NumberType
    {
        NONE = 0,
        HEALTH = 1,
        ENERGY = 2
    } 

    [SerializeField] private Color[] colors = new Color[5];
    [SerializeField] private int colorindex;

    // Start is called before the first frame update
    protected void Start()
    {
        flashstep = flashtime;
        life = lifemax;

        UpdateDisplay();
    }

    // Update is called once per frame
    protected void Update()
    {
        // Progress flash step
        if (flashstep > 0.0f)
        {
            flashstep = Mathf.Max(flashstep-1.0f, 0.0f);
        }

        // Progress Life
        if (life > 0)
        {
            life -= 1;

            if (life <= 0)
            {
                if (targetentity != null)
                {
                    targetentity.GetComponent<Entity>().ClearNumberObject();
                }

                Destroy(gameObject);
                return;
            }
        }

        UpdateDisplay();
    }

    protected void LateUpdate()
    {
        if (targetentity != null)
        {
            transform.position = targetentity.transform.position + new Vector3(0.0f, 16.0f, 0.0f);
        }
    }

    protected void UpdateDisplay()
    {
        string s = value > 0? "+"+value.ToString() : value.ToString();

        textcomponent.text = s;
        textdropcomponent.text = s;

        textcomponent.color = colors[colorindex];

        // Flash timer
        if (flashstep > 0.0f)
        {
            if ( Mathf.Repeat(flashstep, 4.0f) >= 2.0f )
            {
                textcomponent.color = colors[0];
            }
        }
    }

    public void ZeroValue() {value = 0;}

    public void AddValue(NumberType _numbertype, int v)
    {
        value += v;
        colorindex = (int)_numbertype;
        UpdateDisplay();
    }

    public void SetTargetObject(GameObject e) 
    {
        targetentity = e;
        LateUpdate();
    }
    
    public void ClearTargetEntity() {targetentity = null;}
}
