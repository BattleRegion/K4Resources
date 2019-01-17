using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface EquipmentBagInterface
{
    void _OnClickEquipmentItemInter(int UserEquipmentID);       //点击装备
    void _OnLongPressEquipmentItemInter(int UserEquipmentID);   //长按查看详情
    void _OnEquipmentClickRemoveInter(int UserEquipmentID);     //点击移除按钮移除当前装备
}

public class EquipmentBagControl : MonoBehaviour, EquipmentiIemInterface, RemovePetInterface
{
    public enum EquipmentBagType
    {
        all = 0,
        weapon = 1,
        defender = 2,
        Helmet = 3,
        Armor = 4
    }

    public EquipmentBagType CurType;

    public UIGrid equipmentBag;
    public GameObject equipmentItem;
    public UILabel count;
    public List<GameObject> equipmentItems = new List<GameObject>();
    public EquipmentBagInterface bagInter;

    public GameObject removeItem;

    private int clickUserEquipmentID;

    public int ClickUserEquipmentID
    {
        get { return clickUserEquipmentID; }
        set { clickUserEquipmentID = value; }
    }

    /// <summary>
    /// 添加equipmentItem
    /// </summary>
    /// <param name="Level"></param>
    /// <param name="Atk"></param>
    /// <param name="type">元素类型</param>
    /// <param name="MonsterName"></param>
    /// <param name="isNew"></param>
    /// <param name="StarNum">item星数</param>
    /// <param name="itemTag"></param>
    public equipmentItemInterface AddEquipmentItem(
        int Level,
        int Atk,
        DungeonEnum.ElementAttributes Type,
        string SkinID,
        int StarNum,
        int UserEquipmentID
        )
    {
        GameObject iTem;
        iTem = NGUITools.AddChild(equipmentBag.gameObject, equipmentItem);
        iTem.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
        equipmentItemInterface ei = iTem.GetComponent<equipmentItemInterface>();
        ei.SetItem(Level, Atk, Type, SkinID, false, StarNum, UserEquipmentID);
        equipmentItems.Add(iTem);
        iTem.name = UserManager.CurUserInfo.FindUserWare(UserEquipmentID).CurHardWareData.Id;
        ei.equipmentItemInter = this;
        //ei.IsEquip(UserManager.CurUserInfo.FindUserWare(UserEquipmentID).IsWare);
        equipmentBag.repositionNow = true;

        SetNum(UserManager.CurUserInfo.UserWares.Count + UserManager.CurUserInfo.UserItems.Count, UserManager.CurUserInfo.WareLimit);
        return ei;
    }

    public equipmentItemInterface AddEquipmentItem(int Uid)
    {
        UserWare uw = UserManager.CurUserInfo.FindUserWare(Uid);
        GameObject iTem;
        iTem = NGUITools.AddChild(equipmentBag.gameObject, equipmentItem);
        iTem.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
        equipmentItemInterface ei = iTem.GetComponent<equipmentItemInterface>();
        ei.SetItem(uw.Level, (int)uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, false, uw.CurHardWareData.Rank, Uid);
        equipmentItems.Add(iTem);
        iTem.name = uw.CurHardWareData.Id;
        ei.equipmentItemInter = this;
        //ei.IsEquip(uw.IsWare);
        equipmentBag.repositionNow = true;

        SetNum(UserManager.CurUserInfo.UserWares.Count + UserManager.CurUserInfo.UserItems.Count, UserManager.CurUserInfo.WareLimit);
        return ei;
    }

    public equipmentItemInterface GetItemByUId(int Id)
    {
        foreach (GameObject g in equipmentItems)
        {
            if (g.GetComponent<equipmentItemInterface>().userEquipmentID == Id)
            {
                return g.GetComponent<equipmentItemInterface>();
            }
        }
        return null;
    }

    /// <summary>
    /// 清空装备背包
    /// </summary>
    public void ClearBag()
    {
        for (int i = equipmentItems.Count - 1; i >= 0; i--)
        {
            GameObject rgo = equipmentItems[i];
            equipmentItems.Remove(equipmentItems[i]);
            Destroy(rgo);
        }
        equipmentBag.repositionNow = true;
    }

    /// <summary>
    /// 删除一个item
    /// </summary>
    /// <param name="ItemTag"></param>
    public void DestroyItem(int UserEquipmentID)
    {
        foreach (GameObject go in equipmentItems)
        {
            if (go.GetComponent<equipmentItemInterface>().userEquipmentID == UserEquipmentID)
            {
                GameObject rgo = go;
                equipmentItems.Remove(go);
                Destroy(rgo);
                break;
            }
        }
        equipmentBag.repositionNow = true;
    }

    /// <summary>
    /// 设置背包计数
    /// </summary>
    /// <param name="CurrentItemNum"></param>
    /// <param name="BagSize"></param>
    public void SetNum(int CurrentItemNum, int BagSize)
    {
        count.text = CurrentItemNum.ToString() + "/" + BagSize.ToString();
    }

    public GameObject curPanel;
    public GameObject partyPanel;
    public GameObject WeaponDetailPanel;
    public GameObject ArmorDetailPanel;

    public void SwitchPartyPanel()
    {
        curPanel.SetActive(false);
        partyPanel.SetActive(true);
    }

    public void _OnClickEquipmentItem(int UserEquipmentID)
    {
        if (bagInter != null)
        {
            bagInter._OnClickEquipmentItemInter(UserEquipmentID);
        }
    }

    public void _OnClickRemove()
    {
        if (bagInter != null)
        {
            bagInter._OnEquipmentClickRemoveInter(ClickUserEquipmentID);
        }
    }

    public void _OnLongPressEquipmentItem(int UserEquipmentID)
    {
        if (bagInter != null)
        {
            bagInter._OnLongPressEquipmentItemInter(UserEquipmentID);
            UserWare u = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);
            if ((int)u.CurHardWareData.Style < 5)
            {
                WeaponDetailPanel.SetActive(true);
                WeaponDetailPanel.GetComponent<WeaponDetail>().SetDetail(UserEquipmentID);
            }
            else
            {
                ArmorDetailPanel.SetActive(true);
                ArmorDetailPanel.GetComponent<ArmorDetail>().SetDetail(UserEquipmentID);
            }
        }
    }

    bool sorting = false;

    void SetSortingOver()
    {
        sorting = false;
    }

    void OnEnable()
    {
        switch(CurHardwareSortType)
        {
            case HardwareSort.none: SortTypeLabel.text = "获得"; break;
            case HardwareSort.rank: SortTypeLabel.text = "星数"; break;
            case HardwareSort.level: SortTypeLabel.text = "等级"; break;
            case HardwareSort.atk: SortTypeLabel.text = "攻击"; break;
            case HardwareSort.def: SortTypeLabel.text = "防御"; break;
            default: break;
        }

        Invoke("DelayReposition", 0.05f);

        if(SortButton != null)
        {
            SortButton.SetActive(true);
            UIEventListener.Get(SortButton).onClick = (g) =>
            {
                SwitchSorting();
            };
        }
    }

    void DelayReposition()
    {
        equipmentBag.Reposition();
    }

    void OnDisable()
    {
        if(SortButton != null)
        {
            SortButton.SetActive(false);
        }
        foreach (Transform t in equipmentBag.gameObject.transform)
        {
            if (t.gameObject.name == "000rmIcon")
            {
                Destroy(t.gameObject);
                break;
            }
        }
        if(needReset)
        {
            ResetSort();
        }
    }

    /// <summary>
    /// 设置覆盖
    /// </summary>
    public void SetIswareCover()
    {
        foreach(GameObject g in equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            UserWare uw = UserManager.CurUserInfo.FindUserWare(ei.userEquipmentID);
            ei.IsCover(uw.IsWare);
        }
    }

    public void SetNoEvoCover()
    {
        foreach (GameObject g in equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            UserWare uw = UserManager.CurUserInfo.FindUserWare(ei.userEquipmentID);
            if(string.IsNullOrEmpty(uw.CurHardWareData.Evo))
            {
                ei.IsCover(true);
            }
        }

    }

    #region Sort
    public enum HardwareSort
    {
        none = 0,
        rank = 1,
        level = 2,
        atk = 3,
        def = 4
    }
    public HardwareSort CurHardwareSortType;
    public UILabel SortTypeLabel;
    public GameObject SortButton;

    /// <summary>
    /// 切换排序方式
    /// </summary>
    public void SwitchSorting()
    {
        if (!sorting)
        {
            sorting = true;
            Invoke("SetSortingOver", 0.5f);

            if (CurType == EquipmentBagType.all)
            {
                CurHardwareSortType = (HardwareSort)(((int)CurHardwareSortType + 1) > 4 ? 0 : ((int)CurHardwareSortType + 1));
            }
            else if (CurType == EquipmentBagType.weapon)
            {
                CurHardwareSortType = (HardwareSort)(((int)CurHardwareSortType + 1) > 3 ? 0 : ((int)CurHardwareSortType + 1));
            }
            else
            {
                CurHardwareSortType = (HardwareSort)((((int)CurHardwareSortType + 1) > 4 ? 0 : ((int)CurHardwareSortType + 1)) == 3 ? 4 : (((int)CurHardwareSortType + 1) > 4 ? 0 : ((int)CurHardwareSortType + 1)));
            }

            Sorting(CurHardwareSortType);
        }
    }

    /// <summary>
    /// 用于排序的数组
    /// </summary>
    List<UserWare> SortResult = new List<UserWare>();

    public bool needReset = true;

    public void ResetSort()
    {
        CurHardwareSortType = HardwareSort.none;
        if(SortTypeLabel != null)
        {
            SortTypeLabel.text = "获得";
        }
    }

    /// <summary>
    /// 根据类型排序
    /// </summary>
    public void Sorting(HardwareSort type)
    {
        ClearBag();
        equipmentItems.Clear();
        SortResult.Clear();
        switch(CurType)
        {
            case EquipmentBagType.all:
                {
                    foreach (UserWare up in UserManager.CurUserInfo.UserWares)
                    {
                        SortResult.Add(up);
                    }
                    break;
                }
            case EquipmentBagType.weapon:
                {
                    foreach (UserWare up in UserManager.CurUserInfo.UserWares)
                    {
                        if(up.IsWeapon())
                        {
                            SortResult.Add(up);
                        }
                    }
                    break;
                }
            case EquipmentBagType.defender:
                {

                    foreach (UserWare up in UserManager.CurUserInfo.UserWares)
                    {
                        if (up.IsHelmet() || up.IsArmor())
                        {
                            SortResult.Add(up);
                        }
                    }
                    break;
                }
            case EquipmentBagType.Helmet:
                {
                    foreach (UserWare up in UserManager.CurUserInfo.UserWares)
                    {
                        if (up.CurHardWareData.Style == HardWareData.HardWareType.Head)
                        {
                            SortResult.Add(up);
                        }
                    }
                    break;
                }
            case EquipmentBagType.Armor:
                {
                    foreach (UserWare up in UserManager.CurUserInfo.UserWares)
                    {
                        if (up.CurHardWareData.Style == HardWareData.HardWareType.Cuirass)
                        {
                            SortResult.Add(up);
                        }
                    }
                    break;
                }
            default: break;
        }
        
        switch (CurHardwareSortType)
        {
            case HardwareSort.none:
                {
                    SortTypeLabel.text = "获得";
                    DefaultSort();
                    break;
                }
            case HardwareSort.rank:
                {
                    SortTypeLabel.text = "星数";
                    RankSort();
                    break;
                }
            case HardwareSort.level:
                {
                    SortTypeLabel.text = "等级";
                    LevelSort();
                    break;
                }
            case HardwareSort.atk:
                {
                    SortTypeLabel.text = "攻击";
                    AtkSort();
                    break;
                }
            case HardwareSort.def:
                {
                    SortTypeLabel.text = "防御";
                    DefSort();
                    break;
                }
            default: break;
        }
    }

    /// <summary>
    /// 交换两个位置
    /// </summary>
    void ExchangePosition(List<UserWare> list, int index1, int index2)
    {
        UserWare u = list[index1];
        list[index1] = list[index2];
        list[index2] = u;
    }

    /// <summary>
    /// 默认排序
    /// </summary>
    void DefaultSort()
    {
        foreach(UserWare uw in SortResult)
        {
            equipmentItemInterface ei = AddEquipmentItem(uw.UserWareId);
            if (BagControl.NotInParty)
            {
                if (uw.IsWare)
                {
                    ei.IsEquip(true);
                }
            }
            else
            {
                if (uw.warePartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ei.IsEquip(true);
                }
            }
        }
    }

    /// <summary>
    /// 星级排序
    /// </summary>
    void RankSort()
    {
        RankQuickSort(0, SortResult.Count - 1);
        foreach(UserWare uw in SortResult)
        {
            equipmentItemInterface ei = AddEquipmentItem(uw.UserWareId);
            if (BagControl.NotInParty)
            {
                if (uw.IsWare)
                {
                    ei.IsEquip(true);
                }
            }
            else
            {
                if (uw.warePartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ei.IsEquip(true);
                }
            }
        }
    }

    void RankQuickSort(int left, int right)
    {
        if(left < right)
        {
            int middle = RankPartition(left, right);
            RankQuickSort(left, middle - 1);
            RankQuickSort(middle + 1, right);
        }
    }

    int RankPartition(int left, int right)
    {
        while(left < right)
        {
            while(SortResult[right].CurHardWareData.Rank < SortResult[left].CurHardWareData.Rank)
            {
                right--;
            }
            if(left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while(SortResult[left].CurHardWareData.Rank > SortResult[right].CurHardWareData.Rank)
            {
                left++;
            }
            if(left < right)
            {
                ExchangePosition(SortResult, left, right);
                right--;
            }
        }
        return left;
    }

    /// <summary>
    /// 等级排序
    /// </summary>
    void LevelSort()
    {
        LevelQuickSort(0, SortResult.Count - 1);
        foreach(UserWare uw in SortResult)
        {
            equipmentItemInterface ei = AddEquipmentItem(uw.UserWareId);
            if (BagControl.NotInParty)
            {
                if (uw.IsWare)
                {
                    ei.IsEquip(true);
                }
            }
            else
            {
                if (uw.warePartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ei.IsEquip(true);
                }
            }
        }
    }

    void LevelQuickSort(int left, int right)
    {
        if(left < right)
        {
            int middle = LevelPartition(left, right);
            LevelQuickSort(left, middle - 1);
            LevelQuickSort(middle + 1, right);
        }
    }

    int LevelPartition(int left, int right)
    {
        while(left < right)
        {
            while(SortResult[right].Level < SortResult[left].Level)
            {
                right--;
            }
            if(left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while(SortResult[left].Level > SortResult[right].Level)
            {
                left++;
            }
            if(left < right)
            {
                ExchangePosition(SortResult, left, right);
                right--;
            }
        }
        return left;
    }

    /// <summary>
    /// 攻击排序
    /// </summary>
    void AtkSort()
    {
        AtkQuickSort(0, SortResult.Count - 1);
        foreach(UserWare uw in SortResult)
        {
            equipmentItemInterface ei = AddEquipmentItem(uw.UserWareId);
            ei.levelInfoLbl.text = ((int)uw.CurAtk).ToString();
            if (BagControl.NotInParty)
            {
                if (uw.IsWare)
                {
                    ei.IsEquip(true);
                }
            }
            else
            {
                if (uw.warePartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ei.IsEquip(true);
                }
            }
        }
    }

    void AtkQuickSort(int left, int right)
    {
        if(left < right)
        {
            int middle = AtkPartition(left, right);
            AtkQuickSort(left, middle - 1);
            AtkQuickSort(middle + 1, right);
        }
    }

    int AtkPartition(int left, int right)
    {
        while(left < right)
        {
            while(SortResult[right].CurAtk < SortResult[left].CurAtk)
            {
                right--;
            }
            if(left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while(SortResult[left].CurAtk > SortResult[right].CurAtk)
            {
                left++;
            }
            if (left < right)
            {
                ExchangePosition(SortResult, left, right);
                right--;
            }
        }
        return left;
    }

    /// <summary>
    /// 防御排序
    /// </summary>
    void DefSort()
    {
        DefQuickSort(0, SortResult.Count - 1);
        foreach(UserWare uw in SortResult)
        {
            equipmentItemInterface ei = AddEquipmentItem(uw.UserWareId);
            ei.levelInfoLbl.text = ((int)uw.CurDef).ToString();
            if (BagControl.NotInParty)
            {
                if (uw.IsWare)
                {
                    ei.IsEquip(true);
                }
            }
            else
            {
                if (uw.warePartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ei.IsEquip(true);
                }
            }
        }
    }

    void DefQuickSort(int left, int right)
    {
        if(left < right)
        {
            int middle = DefPartition(left, right);
            DefQuickSort(left, middle - 1);
            DefQuickSort(middle + 1, right);
        }
    }

    int DefPartition(int left, int right)
    {
        while(left < right)
        {
            while(SortResult[right].CurDef < SortResult[left].CurDef)
            {
                right--;
            }
            if(left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while(SortResult[left].CurDef > SortResult[right].CurDef)
            {
                left++;
            }
            if(left < right)
            {
                ExchangePosition(SortResult, left, right);
                right--;
            }
        }
        return left;
    }
    #endregion
}
