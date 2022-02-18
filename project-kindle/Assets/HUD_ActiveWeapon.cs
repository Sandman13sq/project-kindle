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

    // Start is called before the first frame update
    void Start()
    {
        SetLevel(1);
        SetAmmo(70);
        SetAmmoMax(100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method ===============================================

    void SetLevel(int _value)
    {
        level = _value;
        levelstringobj.GetComponent<UnityEngine.UI.Text>().text = "Lv " + level.ToString();
    }

    void SetAmmo(int _value)
    {
        ammo = _value;
        numeratorobj.GetComponent<UnityEngine.UI.Text>().text = ammo.ToString();
    }

    void SetAmmoMax(int _value)
    {
        ammomax = _value;
        denominatorobj.GetComponent<UnityEngine.UI.Text>().text = ammomax.ToString();
    }
}
