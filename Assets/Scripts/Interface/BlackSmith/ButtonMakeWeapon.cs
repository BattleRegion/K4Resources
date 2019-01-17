using UnityEngine;
using System.Collections;

public interface _ButtonMakeWeapon
{
    void _OnClickButtonMakeWeapon(string EquipmentID);
}

public class ButtonMakeWeapon : MonoBehaviour 
{
    public _ButtonMakeWeapon makeWeaponInter;

    public string equipmentID;

    void OnClick()
    {
        if (makeWeaponInter != null)
        {
            makeWeaponInter._OnClickButtonMakeWeapon(equipmentID);
        }
    }
}
