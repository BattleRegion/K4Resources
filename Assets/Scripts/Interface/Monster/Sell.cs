using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;
public class Sell : MonoBehaviour ,BagInterface
{
    public UILabel sellPrice;

    public BagControl bagControl;

    public UIScrollView dragPanel;

    public SetUser UserInfo;

    public GameObject SortButton;

    public void Awake()
    {
        bagControl.bagInter = this;
        BagControl.NotInParty = true;
    }

    void OnEnable()
    {
        bagControl.ClearBag();
        foreach (UserPet pet in UserManager.CurUserInfo.UserPets)
        {
            ItemInterface item = bagControl.CreateSetItem(pet.Level, pet.CurPetData.PCost, (int)pet.CurPetData.Hp, (int)pet.CurPetData.Attack, pet.CurPetData.PetPro, pet.CurPetData.Id, pet.CurPetData.Rank, pet.UserPetId, false);
            if (pet.inParty)
            {
                item.InParty(true);
            }
            else
            {
                item.InParty(false);
            }
        }
        bagControl.SetNum(UserManager.CurUserInfo.UserPets.Count, UserManager.CurUserInfo.PetHouseLimit);
        CurSellPet.Clear();
        SetPrice(0);

        UIEventListener.Get(SortButton).onClick = (g) =>
        {
            SellItems.Clear();
            CurSellPet.Clear();
            SetPrice(0);
            sellFull = false;

            bagControl.SwitchSorting();
        };
    }



    public void SetPrice(int Price)
    {
        sellPrice.text = Price.ToString();
    }

    public void Clear()
    {
        bagControl.ClearBag();
    }


    public List<UserPet> CurSellPet = new List<UserPet>();

    public List<ItemInterface> SellItems = new List<ItemInterface>();

    bool sellFull = false;

    public ItemInterface GetItemById(int Uid)
    {
        foreach (GameObject g in bagControl.items)
        {
            if (g.GetComponent<ItemInterface>().userMonsterID == Uid)
            {
                return g.GetComponent<ItemInterface>();
            }
        }
        return null;
    }

    public void SetSellNum()
    {
        int i = 1;
        if (SellItems == null) return;
        foreach (ItemInterface ii in SellItems)
        {
            ii.SetSell(true, i++);
            if (i > 99)
            {
                i = 99;
                sellFull = true;
                break;
            }
        }
    }

    public void _OnClickItemInter(int UserMonsterID)
    {
        UserPet up = UserManager.CurUserInfo.FindPetById(UserMonsterID);
        if (up.inParty) return;
        ItemInterface ii = GetItemById(UserMonsterID);
        if (CurSellPet.Contains(up))
        {
            CurSellPet.Remove(up);
            SellItems.Remove(ii);
            ii.SetSelect(false);
            ii.SetSell(false);
            sellFull = false;
        }
        else if (sellFull)
        {
            return;
        }
        else
        {
            CurSellPet.Add(up);
            SellItems.Add(ii);
            ii.SetSelect(true);
        }
        int allPrice = 0;
        foreach (UserPet p in CurSellPet)
        {
            allPrice = allPrice + p.CurPetData.Price;
        }
        SetPrice(allPrice);
        SetSellNum();
    }

    public void _OnLongPressItemInter(int UserMonsterID)
    {
    }

    public void _OnClickRemoveInter(int UserMonsterID)
    {
    }

    public void SellCur()
    {
        if (CurSellPet.Count == 0) return;
        JsonArray ids = new JsonArray();
        foreach (UserPet up in CurSellPet)
        {
            ids.Add(up.UserPetId);
        }
        JsonObject args = new JsonObject();
        args.Add("house_ids", ids);
        SocketCenter.Request(GameRouteConfig.SalePet, args, (result) =>
        {
            if (result.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    foreach (UserPet up in CurSellPet)
                    {
                        UserManager.CurUserInfo.UserPets.Remove(up);
                        bagControl.DestroyItem(up.UserPetId);
                    }
                    UserManager.CurUserInfo.AddUserElements((JsonArray)result.Data["elements"]);
                    CurSellPet.Clear();
                    SetPrice(0);
                    bagControl.SetNum(UserManager.CurUserInfo.UserPets.Count, UserManager.CurUserInfo.PetHouseLimit);
                    dragPanel.SetDragAmount(0, 0, false);
                    dragPanel.UpdateScrollbars();
                    UserInfo.SetInfo();
                    CancelAllSell();
                });
            }
        }, null, true,true);
    }

    public void CancelAllSell()
    {
        foreach (ItemInterface ii in SellItems)
        {
            ii.SetSell(false);
            ii.SetSelect(false);
        }
        SellItems.Clear();
        CurSellPet.Clear();
        SetPrice(0);
        sellFull = false;
    }

    void OnDisable()
    {
        CancelAllSell();
    }
}
