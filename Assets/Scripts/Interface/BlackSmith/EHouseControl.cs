using UnityEngine;
using System.Collections;

public class EHouseControl : MonoBehaviour,EquipmentBagInterface
{
    public EquipmentBagControl EquipmentBag;

    public GameObject SortButton;

    void OnEnable()
    {
        EquipmentBag.bagInter = this;
        foreach (UserWare ware in UserManager.CurUserInfo.UserWares)
        {
            EquipmentBag.AddEquipmentItem(ware.Level, ware.CurAtk, ware.CurHardWareData.Element, ware.CurHardWareData.SkinId, ware.CurHardWareData.Rank, ware.UserWareId);
        }
    }

    void OnDisable()
    {
        EquipmentBag.ClearBag();
        EquipmentBag.ResetSort();
    }
    
    public void _OnClickEquipmentItemInter(int UserWareID)
    {
        EquipmentBag._OnLongPressEquipmentItem(UserWareID);
    }

    public void _OnLongPressEquipmentItemInter(int UserMonsterID)
    {
    }

    public void _OnEquipmentClickRemoveInter(int UserMonsterID)
    {
    }
}
