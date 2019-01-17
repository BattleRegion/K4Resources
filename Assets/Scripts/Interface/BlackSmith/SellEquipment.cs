using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class SellEquipment : MonoBehaviour, EquipmentBagInterface, _ButtonClearSell, _ButtonSell
{
    public UILabel sellPrice;

    public ButtonClearSell ClearButton;

    public ButtonSell SellButton;

    public EquipmentBagControl bagControl;

    public SetUser UserInfo;

    public SellMaterial sellSwitch;

    public GameObject SortButton;

    void Awake()
    {
        
    }

    void OnEnable()
    {
        SortButton.SetActive(true);
        bagControl.Sorting(bagControl.CurHardwareSortType);
        bagControl.SetIswareCover();

        bagControl.bagInter = this;
        ClearButton.inter = this;
        SellButton.inter = this;
        CurSellWare.Clear();
        SetPrice(0);
        Invoke("DelayResetPosition", 0.2f);

        UIEventListener.Get(SortButton).onClick = (g) =>
        {
            CancleAllSell();
            bagControl.SwitchSorting();
            bagControl.SetIswareCover();
        };
    }

    void DelayResetPosition()
    {
        this.GetComponent<UIScrollView>().SetDragAmount(0, 0, false);
        this.GetComponent<UIScrollView>().UpdateScrollbars();
    }

    public void SetPrice(int Price)
    {
        sellPrice.text = Price.ToString();
    }

    public void Clear()
    {
        bagControl.ClearBag();
    }


    public List<UserWare> CurSellWare = new List<UserWare>();
    public List<equipmentItemInterface> SellItems = new List<equipmentItemInterface>();
    bool sellFull = false;

    public equipmentItemInterface GetItemById(int UserWareID)
    {
        foreach (GameObject g in bagControl.equipmentItems)
        {
            if (g.GetComponent<equipmentItemInterface>().userEquipmentID == UserWareID)
            {
                return g.GetComponent<equipmentItemInterface>();
            }
        }
        return null;
    }

    public void SetSellNum()
    {
        int i = 1;
        foreach (equipmentItemInterface ei in SellItems)
        {
            ei.SetSellSequence(true, i++);
            if (i > 99)
            {
                i = 99;
                sellFull = true;
                break;
            }
        }
    }

    public void _OnClickEquipmentItemInter(int UserWareID)
    {
        UserWare uw = UserManager.CurUserInfo.FindUserWare(UserWareID);
        equipmentItemInterface ei = GetItemById(UserWareID);
        if (uw.IsWare) return;
        if (CurSellWare.Contains(uw))
        {
            CurSellWare.Remove(uw);
            SellItems.Remove(ei);
            ei.SetSelect(false);
            ei.SetSellSequence(false);
            sellFull = false;
        }
        else if (sellFull)
        {
            return;
        }
        else
        {
            CurSellWare.Add(uw);
            SellItems.Add(ei);
            ei.SetSelect(true);
        }
        int allPrice = 0;
        foreach (UserWare w in CurSellWare)
        {
            allPrice = allPrice + w.CurHardWareData.Price;
        }
        SetPrice(allPrice);
        SetSellNum();
    }

    public void _OnLongPressEquipmentItemInter(int UserMonsterID)
    {
    }

    public void _OnEquipmentClickRemoveInter(int UserMonsterID)
    {
    }

    public void _OnClickClear()
    {
        CancleAllSell();
    }

    public void _OnClickSell()
    {
        SellCur();
        this.GetComponent<UIScrollView>().SetDragAmount(0, 0, false);
        this.GetComponent<UIScrollView>().UpdateScrollbars();
    }

    public void SellCur()
    {
        if(CurSellWare.Count != 0)
        {
            JsonArray ids = new JsonArray();
            foreach (UserWare uw in CurSellWare)
            {
                ids.Add(uw.UserWareId);
            }
            JsonObject args = new JsonObject();
            args.Add("house_ids", ids);
            SocketCenter.Request(GameRouteConfig.SellHardware, args, (result) =>
            {
                if (result.Code == SocketResult.ResultCode.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        foreach (UserWare uw in CurSellWare)
                        {
                            UserManager.CurUserInfo.UserWares.Remove(uw);
                            bagControl.DestroyItem(uw.UserWareId);
                        }
                        UserManager.CurUserInfo.AddElements((JsonArray)result.Data["elements"]);
                        CancleAllSell();
                        UserInfo.SetInfo();
                    });
                }
            }, null, true, true);
        }
    }

    public void CancleAllSell()
    {
        CurSellWare.Clear();
        foreach (equipmentItemInterface ei in SellItems)
        {
            ei.SetSellSequence(false);
            ei.SetSelect(false);
        }
        SellItems.Clear();
        SetPrice(0);
        sellFull = false;
    }

    void OnDisable()
    {
        SortButton.SetActive(false);

        CancleAllSell();
        bagControl.ClearBag();
        ClearButton.inter = sellSwitch;
        SellButton.inter = sellSwitch;
    }
}
