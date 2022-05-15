using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMeters : MasterObject
{
    [SerializeField] private HUDMeter vectorMeter;
    [SerializeField] private GameObject vectorLevel; 
    [SerializeField] private HUDMeter dragonMeter;
    [SerializeField] private GameObject dragonLevel; 
    [SerializeField] private HUDMeter tempestMeter;
    [SerializeField] private GameObject tempestLevel; 
    [SerializeField] private HUDMeter meteorMeter;
    [SerializeField] private GameObject meteorLevel;

    [SerializeField] private GameObject vectorEntry;
    [SerializeField] private GameObject dragonEntry;
    [SerializeField] private GameObject tempestEntry;
    [SerializeField] private GameObject meteorEntry;
    [SerializeField] private GameObject unknownEntry1;
    [SerializeField] private GameObject unknownEntry2;
    [SerializeField] private GameObject unknownEntry3;
    [SerializeField] private GameObject unknownEntry4;

    private UnityEngine.UI.Text vectorLevelText;
    private UnityEngine.UI.Text dragonLevelText;
    private UnityEngine.UI.Text tempestLevelText;
    private UnityEngine.UI.Text meteorLevelText;

    private PlayerData playerdata;
    private Weapon[] weapons;
    // Start is called before the first frame update
    void Start()
    {
        playerdata = game.GetPlayerData();
        vectorLevelText = vectorLevel.GetComponent<UnityEngine.UI.Text>();
        dragonLevelText = dragonLevel.GetComponent<UnityEngine.UI.Text>();
        tempestLevelText = tempestLevel.GetComponent<UnityEngine.UI.Text>();
        meteorLevelText = meteorLevel.GetComponent<UnityEngine.UI.Text>();
        weapons = playerdata.GetWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        //Constantly Check the player's weapons array to check if they have unlocked the weapon or not
        int index = 0;
        foreach(Weapon gun in weapons)
        {
            gun.SetIsUnlocked(true);
            bool unlocked = gun.GetIsUnlocked();
            if(unlocked == true)
            {
                //0 = vector, 1 = dragon, 2 = meteor, 3 = tempest
                if(index == 0)
                {
                    vectorEntry.SetActive(true);
                    unknownEntry1.SetActive(false);
                }
                else if(index == 1)
                {
                    dragonEntry.SetActive(true);
                    unknownEntry2.SetActive(false);
                }
                else if(index == 2)
                {
                    meteorEntry.SetActive(true);
                    unknownEntry3.SetActive(false);
                }
                else if(index == 3)
                {
                    tempestEntry.SetActive(true);
                    unknownEntry4.SetActive(false);
                }
            }
            index++;
        }

        //Debug.Log("Vector energy is: " + playerdata.GetWeapons()[0].CurrentLevelEnergy());
        vectorMeter.SetValueMax(weapons[0].CurrentLevelEnergyMax());
        vectorMeter.SetValue(weapons[0].CurrentLevelEnergy());
        vectorLevelText.text = "Lv " + weapons[0].GetLevel().ToString();

        dragonMeter.SetValueMax(weapons[1].CurrentLevelEnergyMax());
        dragonMeter.SetValue(weapons[1].CurrentLevelEnergy());
        dragonLevelText.text = "Lv " + weapons[1].GetLevel().ToString();

        tempestMeter.SetValueMax(weapons[3].CurrentLevelEnergyMax());
        tempestMeter.SetValue(weapons[3].CurrentLevelEnergy());
        tempestLevelText.text = "Lv " + weapons[3].GetLevel().ToString();

        meteorMeter.SetValueMax(weapons[2].CurrentLevelEnergyMax());
        meteorMeter.SetValue(weapons[2].CurrentLevelEnergy());
        meteorLevelText.text = "Lv " + weapons[2].GetLevel().ToString();
    }
}
