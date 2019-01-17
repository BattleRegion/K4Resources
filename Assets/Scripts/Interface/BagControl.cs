using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface BagInterface
{
    void _OnClickItemInter(int UserMonsterID);       //点击怪物
    void _OnLongPressItemInter(int UserMonsterID);   //长按查看详情
    void _OnClickRemoveInter(int UserMonsterID);     //点击移除按钮移除当前怪物
}

public class BagControl : MonoBehaviour, itemInterface, RemovePetInterface
{
    public UIGrid monsterBag;
    public GameObject item;
    public UILabel bagCount;

    public UILabel CostLabel;

    public GameObject SortButton;

    /// <summary>
    /// 用于移除的item
    /// </summary>
    public GameObject removeItem;

    //当前点击的位置
    private int clickPosition;

    public int ClickPosition
    {
        get { return clickPosition; }
        set { clickPosition = value; }
    }

    private int clickUserMonsterID;

    public int ClickUserMonsterID
    {
        get { return clickUserMonsterID; }
        set { clickUserMonsterID = value; }
    }


    //表示是不是在组队界面中
    public static bool NotInParty = false;

    public List<GameObject> items = new List<GameObject>();
    public BagInterface bagInter;



    /// <summary>
    /// 添加item
    /// </summary>
    /// <param name="Level"></param>
    /// <param name="Cost"></param>
    /// <param name="Hp"></param>
    /// <param name="Atk"></param>
    /// <param name="type">元素类型</param>
    /// <param name="MonsterName"></param>
    /// <param name="isNew"></param>
    /// <param name="StarNum">item星数</param>
    /// <param name="itemTag"></param>
    public ItemInterface CreateSetItem(
        int Level,
        int Cost,
        int Hp,
        int Atk,
        DungeonEnum.ElementAttributes Type,
        string MonsterID,
        int StarNum,
        int UserMonsterID,
        bool isNew
        )
    {
        GameObject iTem;
        iTem = NGUITools.AddChild(monsterBag.gameObject, item);
        iTem.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
        iTem.GetComponent<ItemInterface>().SetItem(Level, Cost, Hp, Atk, Type, MonsterID, isNew, StarNum, UserMonsterID);
        monsterBag.repositionNow = true;
        items.Add(iTem);
        iTem.GetComponent<ItemInterface>().itemInter = this;
        iTem.name = MonsterID;

        return iTem.GetComponent<ItemInterface>();
    }

    public ItemInterface CreateSetItem(int Uid)
    {
        UserPet up = UserManager.CurUserInfo.FindPetById(Uid);
        GameObject iTem;
        iTem = NGUITools.AddChild(monsterBag.gameObject, item);
        iTem.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
        iTem.GetComponent<ItemInterface>().SetItem(up.Level, up.CurPetData.PCost, (int)up.CurHp, (int)up.CurAtk, up.CurPetData.PetPro, up.CurPetData.Id, false, up.CurPetData.Rank, up.UserPetId);
        monsterBag.repositionNow = true;
        items.Add(iTem);
        iTem.GetComponent<ItemInterface>().itemInter = this;
        iTem.name = up.UserPetId.ToString();

        return iTem.GetComponent<ItemInterface>();

    }

    bool sorting = false;

    void SetSortingOver()
    {
        sorting = false;
    }

    void OnEnable()
    {
        foreach (UserPet up in UserManager.CurUserInfo.UserPets)
        {
            ItemInterface item = FindItem(up.UserPetId);
            if (item)
            {
                if (NotInParty)
                {
                    if (up.inParty)
                    {
                        item.InParty(true);
                    }
                }
                else
                {
                    if (up.inPartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                    {
                        item.InParty(true);
                    }
                }
            }
        }
        Invoke("Reposition", 0.05f);

        if(SortButton != null)
        {
            UIEventListener.Get(SortButton).onClick = (g) =>
            {
                SwitchSorting();
            };
        }
    }

    public void DelayReposition()
    {
        Invoke("Reposition", 0.05f);
    }

    void Reposition()
    {
        monsterBag.Reposition();
    }

    public ItemInterface FindItem(int Id)
    {
        foreach (GameObject go in items)
        {
            ItemInterface item = go.GetComponent<ItemInterface>();
            if (item.userMonsterID == Id)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 清空怪物背包
    /// </summary>
    public void ClearBag()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            GameObject rgo = items[i];
            items.Remove(items[i]);
            Destroy(rgo);
        }
        monsterBag.repositionNow = true;
    }

    /// <summary>
    /// 设置背包计数
    /// </summary>
    public void SetNum(int CurrentItemNum, int BagSize)
    {
        bagCount.text = CurrentItemNum.ToString() + "/" + BagSize.ToString();
    }

    public void SetCost()
    {
        if(CostLabel != null)
        {
            int currentCost = 0;
            foreach(UserPet u in UserManager.CurUserInfo.UserPartys[UserManager.CurUserInfo.CurPartyIndex].pets)
            {
                currentCost += u.CurPetData.PCost;
            }
            CostLabel.text = currentCost.ToString() + "/" + UserManager.CurUserInfo.CurHero.Hcost.ToString();
        }
    }

    /// <summary>
    /// 删除一个item
    /// </summary>
    /// <param name="ItemTag"></param>
    public void DestroyItem(int UserMonsterID)
    {
        foreach (GameObject go in items)
        {
            if (go.GetComponent<ItemInterface>().userMonsterID == UserMonsterID)
            {
                GameObject rgo = go;
                items.Remove(go);
                Destroy(rgo);
                break;
            }
        }
        monsterBag.repositionNow = true;
    }

    /// <summary>
    /// 根据UserMonsterID设置其在队伍
    /// </summary>
    /// <param name="UserMonsterID"></param>
    public void SetInParty(int UserMonsterID)
    {
        foreach (Transform g in monsterBag.gameObject.transform)
        { 
            ItemInterface i = g.gameObject.GetComponent<ItemInterface>();
            if (i.userMonsterID == UserMonsterID)
            {
                i.InParty(true);
            }
        }
    }

    public GameObject curPanel;
    public GameObject partyPanel;
    public GameObject detailPanel;
    /// <summary>
    /// 跳转到详细信息panel
    /// </summary>
    public void SwitchDetailPanel()
    {
        detailPanel.SetActive(true);
    }

    public void SwitchPartyPanel()
    {
        curPanel.SetActive(false);
        partyPanel.SetActive(true);
    }




    public void _OnClickItem(int UserMonsterID)
    {
        if (bagInter != null)
        {
            bagInter._OnClickItemInter(UserMonsterID);
        }
    }

    public void _OnLongPressItem(int UserMonsterID)
    {
        if (bagInter != null && !GuiderLocal.TriggerPve())
        {
            bagInter._OnLongPressItemInter(UserMonsterID);
            SwitchDetailPanel();
            detailPanel.GetComponent<SetMonsterDetail>().SetDetail(UserMonsterID);
        }
    }

    public void _OnClickRemove()
    {
        if (bagInter != null)
        {
            bagInter._OnClickRemoveInter(ClickUserMonsterID);
        }
    }

    void OnDisable()
    {
        ResetSort();
        foreach (Transform t in monsterBag.gameObject.transform)
        {
            if (t.gameObject.name == "000rmIcon")
            {
                Destroy(t.gameObject);
                break;
            }
        }
    }

    #region Sort

    public void SetNoEvoCover()
    {
        foreach(GameObject g in items)
        {
            ItemInterface i = g.GetComponent<ItemInterface>();
            UserPet pet = UserManager.CurUserInfo.FindPetById(i.userMonsterID);
            if(string.IsNullOrEmpty(pet.CurPetData.Evo))
            {
                i.IsCover(true);
            }
        }
    }

    public enum PetSort
    {
        none = -1,
        rank = 0,
        level = 1,
        cost = 2,
        atk = 3,
        hp = 4
    }
    public PetSort CurPetSortType;
    public UILabel SortTypeLabel;

    /// <summary>
    /// 排序界面分类，不同界面需要不同处理
    /// </summary>
    public enum SortView
    {
        Normal = 0,
        Evolution = 1
    }

    /// <summary>
    /// 切换排序方式
    /// </summary>
    public void SwitchSorting(SortView view = SortView.Normal)
    {
        if(!sorting)
        {
            sorting = true;
            Invoke("SetSortingOver", 0.5f);

            CurPetSortType = (PetSort)(((int)CurPetSortType + 1) > 4 ? -1 : ((int)CurPetSortType + 1));
            Sorting(CurPetSortType, view);
        }
    }

    /// <summary>
    /// 用于排序的数组
    /// </summary>
    List<UserPet> SortResult = new List<UserPet>();

    public void ResetSort()
    {
        CurPetSortType = PetSort.none;
        if(SortTypeLabel != null)
        {
            SortTypeLabel.text = "获得";
        }
    }

    /// <summary>
    /// 根据类型排序
    /// </summary>
    public void Sorting(PetSort type, SortView view = SortView.Normal)
    {
        ClearBag();
        items.Clear();
        SortResult.Clear();
        switch(view)
        {
            case SortView.Normal:
                {
                    foreach (UserPet up in UserManager.CurUserInfo.UserPets)
                    {
                        SortResult.Add(up);
                    }
                    break;
                }
            case SortView.Evolution:
                {
                    foreach (UserPet up in UserManager.CurUserInfo.UserPets)
                    {
                        if(!string.IsNullOrEmpty(up.CurPetData.Evo))
                        {
                            SortResult.Add(up);
                        }
                    }
                    break;
                }
            default: break;
        }

        switch(CurPetSortType)
        {
            case PetSort.none:
                {
                    SortTypeLabel.text = "获得";
                    DefaultSort();
                    break;
                }
            case PetSort.rank:
                {
                    SortTypeLabel.text = "星数";
                    RankSort();
                    break;
                }
            case PetSort.level:
                {
                    SortTypeLabel.text = "等级";
                    LevelSort();
                    break;
                }
            case PetSort.cost:
                {
                    SortTypeLabel.text = "领导力";
                    CostSort();
                    break;
                }
            case PetSort.atk:
                {
                    SortTypeLabel.text = "攻击";
                    AtkSort();
                    break;
                }
            case PetSort.hp:
                {
                    SortTypeLabel.text = "生命";
                    HpSort();
                    break;
                }
            default: break;
        }
    }

    /// <summary>
    /// 交换两个位置
    /// </summary>
    void ExchangePosition(List<UserPet> list, int index1, int index2)
    {
        UserPet u = list[index1];
        list[index1] = list[index2];
        list[index2] = u;
    }

    /// <summary>
    /// 默认排序实现
    /// </summary>
    void DefaultSort()
    {
        foreach(UserPet up in SortResult)
        {
            ItemInterface ii = CreateSetItem(up.UserPetId);
            if(NotInParty)
            {
                if (up.inParty)
                {
                    ii.InParty(true);
                }
            }
            else
            {
                if(up.inPartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ii.InParty(true);
                }
            }
        }
    }

    /// <summary>
    /// 星级排序实现
    /// </summary>
    void RankSort()
    {
        RankQuickSort(0, SortResult.Count - 1);
        foreach(UserPet up in SortResult)
        {
            ItemInterface ii = CreateSetItem(up.UserPetId);
            if (NotInParty)
            {
                if (up.inParty)
                {
                    ii.InParty(true);
                }
            }
            else
            {
                if (up.inPartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ii.InParty(true);
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
        while (left < right)
        {
            while (SortResult[right].CurPetData.Rank < SortResult[left].CurPetData.Rank)
            {
                right--;
            }
            if (left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while (SortResult[left].CurPetData.Rank > SortResult[right].CurPetData.Rank)
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
    /// 等级排序
    /// </summary>
    void LevelSort()
    {
        LevelQuickSort(0, SortResult.Count - 1);
        foreach (UserPet up in SortResult)
        {
            ItemInterface ii = CreateSetItem(up.UserPetId);
            if (NotInParty)
            {
                if (up.inParty)
                {
                    ii.InParty(true);
                }
            }
            else
            {
                if (up.inPartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ii.InParty(true);
                }
            }
        }
    }

    void LevelQuickSort(int left, int right)
    {
        if (left < right)
        {
            int middle = LevelPartition(left, right);
            LevelQuickSort(left, middle - 1);
            LevelQuickSort(middle + 1, right);
        }
    }

    int LevelPartition(int left, int right)
    {
        while (left < right)
        {
            while (SortResult[right].Level < SortResult[left].Level)
            {
                right--;
            }
            if (left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while (SortResult[left].Level > SortResult[right].Level)
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
    /// cost排序
    /// </summary>
    void CostSort()
    {
        CostQuickSort(0, SortResult.Count - 1);
        foreach (UserPet up in SortResult)
        {
            ItemInterface ii = CreateSetItem(up.UserPetId);
            ii.levelInfoLbl.text = up.CurPetData.PCost.ToString();
            if (NotInParty)
            {
                if (up.inParty)
                {
                    ii.InParty(true);
                }
            }
            else
            {
                if (up.inPartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ii.InParty(true);
                }
            }
        }
    }

    void CostQuickSort(int left, int right)
    {
        if (left < right)
        {
            int middle = CostPartition(left, right);
            CostQuickSort(left, middle - 1);
            CostQuickSort(middle + 1, right);
        }
    }

    int CostPartition(int left, int right)
    {
        while (left < right)
        {
            while (SortResult[right].CurPetData.PCost < SortResult[left].CurPetData.PCost)
            {
                right--;
            }
            if (left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while (SortResult[left].CurPetData.PCost > SortResult[right].CurPetData.PCost)
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
    /// 攻击力排序
    /// </summary>
    void AtkSort()
    {
        AtkQuickSort(0, SortResult.Count - 1);
        foreach (UserPet up in SortResult)
        {
            ItemInterface ii = CreateSetItem(up.UserPetId);
            ii.levelInfoLbl.text = ((int)up.CurAtk).ToString();
            if (NotInParty)
            {
                if (up.inParty)
                {
                    ii.InParty(true);
                }
            }
            else
            {
                if (up.inPartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ii.InParty(true);
                }
            }
        }
    }

    void AtkQuickSort(int left, int right)
    {
        if (left < right)
        {
            int middle = AtkPartition(left, right);
            AtkQuickSort(left, middle - 1);
            AtkQuickSort(middle + 1, right);
        }
    }

    int AtkPartition(int left, int right)
    {
        while (left < right)
        {
            while (SortResult[right].CurAtk < SortResult[left].CurAtk)
            {
                right--;
            }
            if (left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while (SortResult[left].CurAtk > SortResult[right].CurAtk)
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
    /// 生命值排序
    /// </summary>
    void HpSort()
    {
        HpQuickSort(0, SortResult.Count - 1);
        foreach (UserPet up in SortResult)
        {
            ItemInterface ii = CreateSetItem(up.UserPetId);
            ii.levelInfoLbl.text = ((int)up.CurHp).ToString();
            if (NotInParty)
            {
                if (up.inParty)
                {
                    ii.InParty(true);
                }
            }
            else
            {
                if (up.inPartys.Contains(UserManager.CurUserInfo.CurPartyIndex))
                {
                    ii.InParty(true);
                }
            }
        }
    }

    void HpQuickSort(int left, int right)
    {
        if (left < right)
        {
            int middle = HpPartition(left, right);
            HpQuickSort(left, middle - 1);
            HpQuickSort(middle + 1, right);
        }
    }

    int HpPartition(int left, int right)
    {
        while (left < right)
        {
            while (SortResult[right].CurHp < SortResult[left].CurHp)
            {
                right--;
            }
            if (left < right)
            {
                ExchangePosition(SortResult, left, right);
                left++;
            }
            while (SortResult[left].CurHp > SortResult[right].CurHp)
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

    #endregion
}
