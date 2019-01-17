using UnityEngine;
using System.Collections;

public interface _ButtonMakeArmor
{
    void _OnClickButtonMakeArmor(string EquipmentID);
}

public class ButtonMakeArmor : MonoBehaviour
{
    public _ButtonMakeArmor makeArmorInter;

    public string equipmentID;

    void OnClick()
    {
        if (makeArmorInter != null)
        {
            makeArmorInter._OnClickButtonMakeArmor(equipmentID);
        }
    }
}
