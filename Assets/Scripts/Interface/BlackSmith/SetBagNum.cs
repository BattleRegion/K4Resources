using UnityEngine;
using System.Collections;

public class SetBagNum : MonoBehaviour
{
    public EquipmentBagControl bag;

    void OnEnable()
    {
        bag.SetNum(bag.equipmentItems.Count, UserManager.CurUserInfo.WareLimit);
    }

    public void RefreshNum()
    {
        bag.SetNum(bag.equipmentItems.Count, UserManager.CurUserInfo.WareLimit);
    }
}
