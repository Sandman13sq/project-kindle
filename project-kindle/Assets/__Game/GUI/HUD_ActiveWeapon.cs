using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_ActiveWeapon : MonoBehaviour
{
    [SerializeField] private int level; 
    [SerializeField] private int ammo; 
    [SerializeField] private int ammomax; 
    
    [SerializeField] private GameObject levelstringobj; 
    [SerializeField] private GameObject numeratorobj; 
    [SerializeField] private GameObject denominatorobj;
    [SerializeField] private UnityEngine.UI.Image weaponiconrdr;
    [SerializeField] private Sprite[] weaponiconsprites;

    // Start is called before the first frame update
    void Start()
    {
        SetLevel(1);
        SetAmmo(100);
        SetAmmoMax(100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method ===============================================

    public void SetLevel(int _value)
    {
        level = _value;
        levelstringobj.GetComponent<UnityEngine.UI.Text>().text = "Lv " + level.ToString();
    }

    public void SetAmmo(int _value)
    {
        ammo = _value;
        numeratorobj.GetComponent<UnityEngine.UI.Text>().text = ammo.ToString();

        if (ammomax > 0 && ammo == 0) // No ammo, color red
        {
            numeratorobj.GetComponent<UnityEngine.UI.Text>().color = new Color(1.0f, 0.0f, 0.0f);
            denominatorobj.GetComponent<UnityEngine.UI.Text>().color = new Color(1.0f, 0.0f, 0.0f);
        }
        else
        {
            numeratorobj.GetComponent<UnityEngine.UI.Text>().color = new Color(1.0f, 1.0f, 1.0f);
            denominatorobj.GetComponent<UnityEngine.UI.Text>().color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    public void SetAmmoMax(int _value)
    {
        ammomax = _value;
        denominatorobj.GetComponent<UnityEngine.UI.Text>().text = ammomax.ToString();
    }

    public void SetWeapon(int weaponindex)
    {
        weaponiconrdr.sprite = weaponiconsprites[weaponindex];
    }
}
