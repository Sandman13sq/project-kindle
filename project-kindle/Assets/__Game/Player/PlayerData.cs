using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private HUDMeter healthmeter;
    [SerializeField] private HUDMeter energymeter;
    [SerializeField] private HUD_ActiveWeapon weaponinfo;

    [SerializeField] private Weapon[] weapons;
    private Weapon activeweapon;
    private int weaponindex; //0 = Vector, 1 = Dragons Breath

    // Common ==================================================

    // Start is called before the first frame update
    void Start()
    {
        SetActiveWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Methods ==================================================

    public int SetHealth(int value)
    {
        healthmeter.SetValue(value);
        return 0;
    }

    public void HealthFlashMeter()
    {
        healthmeter.Flash();
    }

    public int SetEnergy(int value, int valuemax, bool matchprovisional = false)
    {
        energymeter.SetValue(value, matchprovisional);
        energymeter.SetValueMax(valuemax);
        return 0;
    }

    public int AddEnergy(int e)
    {
        return activeweapon.AddEnergy(e);
    }

    public void EnergyMaximizeProvisional()
    {
        energymeter.MaximizeProvisional();
    }

    public void EnergyFlashMeter()
    {
        energymeter.Flash();
    }
    
    public int SetLevel(int value)
    {
        weaponinfo.SetLevel(value);
        return 0;
    }

    public int SetAmmo(int value, int valuemax)
    {
        weaponinfo.SetAmmo(value);
        weaponinfo.SetAmmoMax(valuemax);
        return 0;
    }

    public void SetActiveWeapon(int _weaponindex)
    {
        // Set all weapons to inactive
        foreach (var w in weapons)
		{
			w.SetActive(false);
		}

        // Set target weapon to active
        weaponindex = _weaponindex;
        activeweapon = weapons[weaponindex];
        activeweapon.SetActive(true);
        weaponinfo.SetWeapon(weaponindex);
        energymeter.SetValueDirect(activeweapon.CurrentLevelEnergy(), activeweapon.CurrentLevelEnergyMax());
    }

    public void NextWeapon()
    {
        SetActiveWeapon((weaponindex+1) % 2);
    }

    public void PrevWeapon()
    {
        SetActiveWeapon(weaponindex==0? weapons.Length-1: weaponindex-1);
    }

    public Weapon GetActiveWeapon() {return activeweapon;}
    public int GetWeaponIndex() {return weaponindex;}
}
