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
    [SerializeField] private int energy;  // Amount of energy weapon has
    [SerializeField] private int levelmax;   // Max weapon level
    private int level;   // Current weapon level (Index 0 = Level 1)

    [SerializeField] private int ammomax = 0; // Max amount of ammo a weapon can have
    private int ammo;    // Current weapon ammo
    [SerializeField] private int shotcountmax = 0;
    private int shotcount = 0;

    [SerializeField] private float delaytime;    // Time between shots
    private float delayprogress;    // Used to delay shots
    [SerializeField] private float rechargetime;    // Time between adding ammo
    private float rechargeprogress;    // Used to delay shots
    [SerializeField] private float autofiretime;    // Time between automatically shooting again
    private float autofireprogress;    // Used to delay shots

    private float firebuffer;
    private float firebuffertime = 5.0f;

    [SerializeField] public GameObject[] projectiles;
    [SerializeField] public int[] weaponlvlenergy;  // Value of energy checkpoints for levels, each more than the last
    public WeaponLvl activeweaponlvl;

    [SerializeField] private Entity_Move_Manual player;
    private float hsign;    // Horizontal sign. {-1, 1}
    private float vsign;    // Vertical sign. {-1, 0, 1}

    [SerializeField] private PlayerData playerdata;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ammo = ammomax;
        autofireprogress = autofiretime;

        level = GetCurrentLevelIndex();
        playerdata.SetEnergy(CurrentLevelEnergy(), CurrentLevelEnergyMax());
        playerdata.SetLevel(GetLevel());
        playerdata.SetAmmo(ammo, ammomax);
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

        // Shoot
        if ( Input.GetButtonDown("Fire1") )
        {
            firebuffer = firebuffertime;
        }

        // Autofire
        if ( Input.GetButton("Fire1") )
        {
            if (autofireprogress > 0.0f)
            {
                autofireprogress = Mathf.Max(0.0f, autofireprogress-1.0f);
                if (autofireprogress == 0.0f)
                {
                    autofireprogress = autofiretime;
                    firebuffer = 10.0f;
                }
            }
        }
        // No recharge if key is held down
        else
        {
            if ( ammo < ammomax )   // Ammo is not at max
            {
                if (rechargeprogress < rechargetime)
                {
                    rechargeprogress += 1.0f;
                }
                else
                {
                    rechargeprogress = 0.0f;
                    AddAmmo(1);
                }
            }

            autofireprogress = autofiretime;
        }

        // Check for fire projectile
        if ( UpdateDelay() && BelowShotCount() && HasAmmo() )
        {
            // temp schüt
            if (firebuffer > 0.0f)
            {
                firebuffer = 0.0f;
                delayprogress = delaytime;

                float xaim, yaim;

                if (vsign != 0.0f && !(vsign == -1.0f && player.GetOnGround()) )
                {
                    xaim = 0.0f;
                    yaim = vsign;
                }
                else
                {
                    xaim = hsign;
                    yaim = 0.0f;
                }

                IncrementShotCount();
                AddAmmo(-1);

                WeaponProjectile proj = Instantiate(projectiles[level]).GetComponent<WeaponProjectile>();
                proj.transform.position = transform.position + new Vector3(xaim*40.0f, yaim*48.0f, 0.0f);
                proj.SetDirectionRad(Mathf.Atan2(yaim, xaim), hsign);
                proj.SetSourceWeapon(this);

                player.OnShoot();
            }
        }

        playerdata.SetAmmo(ammo, ammomax);
    }

    // Methods ====================================================

    public void SetPlayer(Entity_Move_Manual p) {player = p;} 

    // Progress delay timer. Returns true if delay is zero
    protected bool UpdateDelay(float ts = 1.0f)
    {
        if (delayprogress > 0.0f) {delayprogress = Mathf.Max(delayprogress-ts, 0.0f);}
        return delayprogress == 0.0f;
    }

    // Returns true if delay is zero
    protected bool NoDelay() {return delayprogress == 0.0f;}

    // Returns true if shot count is less than max, or if there is no shot limit (max <= 0)
    protected bool BelowShotCount() {return (shotcountmax <= 0) || (shotcount < shotcountmax);}

    // Return true if weapon has ammo or infinite ammo (ammomax is <= 0)
    protected bool HasAmmo() {return ammomax <= 0 || ammo > 0;}

    public int GetAmmo() {return ammo;}    // Current ammo count
    public int GetAmmoMax() {return ammomax;}  // Max ammo count
    public int GetEnergy() {return energy;}    // Total weapon energy
    public int GetLevel() {return level+1;}  // Current weapon level
    public int GetLevelIndex() {return level;}  // Current weapon level index
    public int GetLevelMax() {return levelmax;}   // Max weapon level

    // Returns total amount of energy needed for level
    int CalcLevelEnergy(int _level)
    {
        return (GetLevelIndex() > 0)? energy - weaponlvlenergy[GetLevelIndex()]: energy;
    }

    int GetMaxEnergy()
    {
        return weaponlvlenergy[levelmax];
    }

    // Returns energy value for level (displayed in meter)
    int CurrentLevelEnergy()
    {
        return (GetLevelIndex() > 0)? energy - weaponlvlenergy[GetLevelIndex()-1]: energy;
    }
    
    // Returns energy value for level (displayed in meter)
    int CurrentLevelEnergyMax()
    {
        return (GetLevelIndex() == 0)? 
            weaponlvlenergy[0]: 
            weaponlvlenergy[GetLevelIndex()]-weaponlvlenergy[GetLevelIndex()-1];
    }

    // Returns level using energy
    int GetCurrentLevelIndex()
    {
        int e = energy;
        for (int i = 0; i < levelmax; i++)
        {
            if (e < weaponlvlenergy[i]) {return i;}
        }
        return levelmax;
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

    // Adds energy to weapon, increasing level if sufficient
    public int AddEnergy(int value, bool showgraphic = false)
    {
        int e = value; // Leftover energy (if any)
        int lastlevel = level;

        // Value is positive
        if (value > 0)
        {
            if (energy+value > weaponlvlenergy[GetLevelMax()])
            {
                e -= weaponlvlenergy[GetLevelMax()]-energy;
                energy = weaponlvlenergy[GetLevelMax()];
            }
            else
            {
                energy += e;
                e = 0;
            }
        }
        // Value is negative
        else if (value < 0)
        {
            if (energy+value < 0)
            {
                e -= energy;
                energy = 0;
            }
            else
            {
                energy += e;
                e = 0;
            }
        }

        level = GetCurrentLevelIndex();
        playerdata.SetEnergy(CurrentLevelEnergy(), CurrentLevelEnergyMax(), value > 0);
        playerdata.SetLevel(GetLevel());

        if (value > 0 && e != value)
        {
            playerdata.EnergyFlashMeter();
        }

        if (level < lastlevel)
        {
            playerdata.EnergyMaximizeProvisional();
        }

        return e;
    }

    // Returns true if weapon delay is above zero
    bool InDelay()
    {
        return delayprogress > 0.0f;
    }

    public int IncrementShotCount()
    {
        shotcount += 1;
        return shotcount;
    }

    public int DecrementShotCount()
    {
        if (shotcount > 0) {shotcount -= 1;};
        return shotcount;
    }

    // Utility ====================================================

}
