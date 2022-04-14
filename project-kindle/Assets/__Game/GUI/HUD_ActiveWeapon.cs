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
    private UnityEngine.UI.Text levelstringtextcomponent;
    private UnityEngine.UI.Text numeratortextcomponent;
    private UnityEngine.UI.Text denominatortextcomponent;
    [SerializeField] private UnityEngine.UI.Image weaponiconrdr;
    [SerializeField] private Sprite[] weaponiconsprites;

    // Start is called before the first frame update
    void Start()
    {
        levelstringtextcomponent = levelstringobj.GetComponent<UnityEngine.UI.Text>();
        numeratortextcomponent = numeratorobj.GetComponent<UnityEngine.UI.Text>();
        denominatortextcomponent = denominatorobj.GetComponent<UnityEngine.UI.Text>();

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
        levelstringtextcomponent.text = "Lv " + level.ToString();
    }

    public void SetAmmo(int _value)
    {
        ammo = _value;
        numeratortextcomponent.text = ammo.ToString();

        if (ammomax > 0 && ammo == 0) // No ammo, color red
        {
            numeratortextcomponent.color = new Color(1.0f, 0.0f, 0.0f);
            denominatortextcomponent.color = new Color(1.0f, 0.0f, 0.0f);
        }
        else
        {
            numeratortextcomponent.color = new Color(1.0f, 1.0f, 1.0f);
            denominatortextcomponent.color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    public void SetAmmoMax(int _value)
    {
        ammomax = _value;
        denominatortextcomponent.text = ammomax.ToString();
    }

    public void SetWeapon(int weaponindex)
    {
        weaponiconrdr.sprite = weaponiconsprites[weaponindex];
    }
}
