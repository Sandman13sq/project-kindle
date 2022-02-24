using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Weapons inherit from this class.
    This class should NOT be used on it's own
*/
public class Weapon : MonoBehaviour
{
    // Internal
    private int ammomax; // Max amount of ammo a weapon can have
    private int ammo;    // Current weapon ammo
    [SerializeField] private int energy;  // Amount of energy weapon has
    [SerializeField] private int levelmax;   // Max weapon level
    [SerializeField] private int level;   // Current weapon level (Index 0 = Level 1)
    private float delaytime;    // Time between shots
    private float delayprogress;    // Used to delay shots
    private float firebuffer;
    private float firebuffertime = 5.0f;

    [SerializeField] public GameObject[] projectiles;
    [SerializeField] public int[] weaponlvlenergy;
    public WeaponLvl activeweaponlvl;

    [SerializeField] private Entity_Move_Manual player;
    private float hsign;    // Horizontal sign. {-1, 1}
    private float vsign;    // Vertical sign. {-1, 0, 1}

    [SerializeField] private PlayerData playerdata;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        levelmax = 2;

        playerdata.SetEnergy(CurrentLevelEnergy(), CurrentLevelEnergyMax());
        playerdata.SetLevel(level);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (player == null) {return;}

        hsign = player.GetHSign();
        vsign = player.GetVSign();

        if (firebuffer > 0.0f)
        {
            firebuffer = Mathf.Max(0.0f, firebuffer-1.0f);
        }

        if ( Input.GetKeyDown("x") )
        {
            firebuffer = firebuffertime;
        }

        if ( UpdateDelay() )
        {
            // temp schüt
            if (firebuffer > 0.0f)
            {
                firebuffer = 0.0f;

                float xaim, yaim;

                if (vsign != 0.0f)
                {
                    xaim = 0.0f;
                    yaim = vsign;
                }
                else
                {
                    xaim = hsign;
                    yaim = 0.0f;
                }

                WeaponProjectile proj = Instantiate(projectiles[level]).GetComponent<WeaponProjectile>();
                proj.transform.position = transform.position + new Vector3(xaim*40.0f, yaim*48.0f, 0.0f);
                proj.SetDirectionRad(Mathf.Atan2(yaim, xaim), hsign);

                player.OnShoot();
            }
        }
    }

    // Methods ====================================================

    public void SetPlayer(Entity_Move_Manual p) {player = p;} 

    // Progress delay timer
    bool UpdateDelay(float ts = 1.0f)
    {
        if (delayprogress > 0.0f) {delayprogress = Mathf.Max(delayprogress-ts, 0.0f);}
        return delayprogress == 0.0f;
    }

    int GetAmmo() {return ammo;}    // Current ammo count
    int GetAmmoMax() {return ammomax;}  // Max ammo count
    int GetEnergy() {return energy;}    // Total weapon energy
    int GetLevel() {return level+1;}  // Current weapon level
    int GetLevelIndex() {return level;}  // Current weapon level index
    int GetLevelMax() {return levelmax;}   // Max weapon level

    // Returns total amount of energy needed for level
    int CalcLevelEnergy(int _level)
    {
        int outenergy = 0;

        for (int i = 0; i <= _level; i++)
        {
            outenergy += weaponlvlenergy[i];
        }
        return outenergy;
    }

    int GetMaxEnergy()
    {
        int e = 0;
        for (int i = 0; i < levelmax; i++) {e += weaponlvlenergy[i];}
        return e;
    }

    // Returns energy value for level (displayed in meter)
    int CurrentLevelEnergy()
    {
        int e = energy;
        for (int i = 0; i < level; i++) {e -= weaponlvlenergy[i];}
        return e;
    }
    
    // Returns energy value for level (displayed in meter)
    int CurrentLevelEnergyMax()
    {
        return weaponlvlenergy[level];
    }

    // Adds to current ammo count
    int AddAmmo(int value)
    {
        ammo = System.Math.Max(System.Math.Min(ammo+value, ammomax), 0);
        return ammo;
    }

    // Adds to max ammo count
    int ExtendAmmo(int value)
    {
        ammomax = System.Math.Max(ammomax+value, 0);
        return ammomax;
    }

    // Return true if weapon has ammo or infinite ammo (ammomax is <= 0)
    bool HasAmmo()
    {
        return ammomax <= 0 || ammo > 0;
    }

    // Adds energy to weapon, increasing level if sufficient
    public int AddEnergy(int value, bool showgraphic = false)
    {
        int e = value; // Leftover energy (if any)
        int elvl;

        // Value is positive
        if (value > 0)
        {
            elvl = CalcLevelEnergy(level);

            // Progress level
            while (energy+e >= elvl && level < levelmax)
            {
                e -= elvl-energy;   // Subtract out enrgy by distance to next level
                energy = elvl;
                level++;
                //activeweaponlvl = weaponlvldata[level];
                elvl = CalcLevelEnergy(level);
            }

            // At max level and new energy exceeds max energy
            if (level == levelmax && (energy+e >= elvl) )
            {
                e -= elvl-energy;   // Subtract out enrgy by distance to next level
                energy = elvl;
            }
            // Just add energy
            else
            {
                energy += e;
                e = 0;
            }
        }
        // Value is negative
        else if (value < 0)
        {
            elvl = CalcLevelEnergy(level-1);

            // New energy drops below current level
            while (level > 0 && energy+e < CalcLevelEnergy(level-1))
            {
                level--;
                e += energy-elvl;
                energy = elvl;
                elvl = CalcLevelEnergy(level-1);
            }

            // Clamp energy
            if (level == 0 && energy+e < 0)
            {
                e += energy-elvl;
                energy = 0;
            }
            // Just add energy
            else
            {
                energy += e;
                e = 0;
            }
        }

        playerdata.SetEnergy(CurrentLevelEnergy(), CurrentLevelEnergyMax(), value > 0);
        playerdata.SetLevel(GetLevel());

        return e;
    }

    // Returns true if weapon delay is above zero
    bool InDelay()
    {
        return delayprogress > 0.0f;
    }


    // Utility ====================================================

}
