using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Weapon : MonoBehaviour
{

    [SerializeField]
    M_Weapon[] Weapons = new M_Weapon[0];

    [SerializeField]
    int nIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        UpdateWeapon();
    }

    /// <summary>
    /// Gets the references for the weapon, if it has been changed during runtime.
    /// </summary>
    void UpdateWeapon()
    {
        GameObject.FindObjectOfType<C_WeaponMod>().bulletPrefab.GetComponent<C_Bullet>().bullet = Weapons[nIndex].mbulletMod;
        GameObject.FindObjectOfType<C_WeaponMod>().gravityOrbPrefab.GetComponent<C_GravityOrb>().hGOrb = Weapons[nIndex].morbeMod;
        GameObject.FindObjectOfType<C_WeaponMod>().sSondShoot = Weapons[nIndex].ShootSound;
        GameObject.FindObjectOfType<C_WeaponMod>().playerStats = Weapons[nIndex].mplayerMod;
        GameObject.FindObjectOfType<C_Ui>().ChangeSprites(Weapons[nIndex].SpriteOne, Weapons[nIndex].SpriteTwo, Weapons[nIndex].SpriteThree);
        GameObject.FindObjectOfType<C_Ui>().UiCurrentPreset = Weapons[nIndex].muiMod;
        GameObject.FindObjectOfType<C_Ui>().ChangePreset(Weapons[nIndex].PresetName, Weapons[nIndex].SpriteLogo);

    }

    /// <summary>
    /// Changes the current weapon.
    /// </summary>
    /// <param name="nDir"></param>
    /// <param name="GetToSpecificIndex"></param>
    public void ChangeWeapon (int nDir, int GetToSpecificIndex = -1)
    {
        if (GetToSpecificIndex >= 0 && GetToSpecificIndex < Weapons.Length)
            nIndex = GetToSpecificIndex;
        else
            nIndex = nIndex >= Weapons.Length - 1 ? 0 : nIndex + Mathf.RoundToInt(Mathf.Sign(nDir));
        GameObject.FindObjectOfType<C_Ui>().ChangePreset(Weapons[nIndex].PresetName, Weapons[nIndex].SpriteLogo);
        UpdateWeapon();
    }

    void AddWeapon(M_Weapon WeaponAdded)
    {
        M_Weapon[] _Tampon = new M_Weapon[Weapons.Length+1];
        for (int i = 0; i < Weapons.Length; i++)
        {
            _Tampon[i] = Weapons[i];
        }
        _Tampon[Weapons.Length] = WeaponAdded;
        Weapons = _Tampon;
    }

    void RemoveWeapon(int IndexWeaponToRemove, bool bRemoveOnlyLast = false, bool bRemoveAllExceptFirst = false)
    {
        if (Weapons.Length > 0)
        {
            if (bRemoveAllExceptFirst)
            {
                M_Weapon _Tampon = Weapons[0];
                Weapons = new M_Weapon[1];
                Weapons[0] = _Tampon;
            }
            else if (bRemoveOnlyLast)
            {
                M_Weapon[] _Tampon = new M_Weapon[Weapons.Length - 1];
                for (int i = 0; i < _Tampon.Length; i++)
                {
                    _Tampon[i] = Weapons[i];
                }
                Weapons = _Tampon;
            }
            else
            {
                M_Weapon[] _Tampon = new M_Weapon[Weapons.Length - 1];
                int CurrentIndex = 0;
                for (int i = 0; i < Weapons.Length; i++)
                {
                    if (i != IndexWeaponToRemove)
                    {
                        _Tampon[CurrentIndex] = Weapons[i];
                        CurrentIndex++;
                    }
                }
                Weapons = _Tampon;
            }
        }
    }

    public int GetNbWeapons()
    {
        return Weapons.Length;
    }
}
