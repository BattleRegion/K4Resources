using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class SellMaterial : MonoBehaviour, materialBagInter, _ButtonSell, _ButtonClearSell
{
    public UILabel sellPrice;

    public ButtonSell SellButton;

    public ButtonClearSell ClearButton;

    public MaterialBagControl bagControl;

    public SetUser UserInfo;

    public SellEquipment sellSwitch;

    void OnEnable()
    {
        bagControl.inter = this;
        SellButton.inter = this;
        ClearButton.inter = this;
        bagControl.DelaySortMaterial();
        CurSellItem.Clear();
        SetPrice(0);
        Invoke("DelayResetPosition", 0.2f);
    }

    void Update()
    {
        bagControl.SetCount();
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
        bagControl.ClearMaterialBag();
    }


    public List<UserItem> CurSellItem = new List<UserItem>();

    public List<MaterialItemInterface> SellItems = new List<MaterialItemInterface>();

    bool sellFull = false;

    public MaterialItemInterface GetMaterialById(int Id)
    {
        foreach (GameObject g in bagControl.materialItems)
        {
            if (g.GetComponent<MaterialItemInterface>().userMaterialId == Id)
            {
                return g.GetComponent<MaterialItemInterface>();
            }
        }
        return null;
    }

    public void SetSellNum()
    {
        int i = 1;
        foreach (MaterialItemInterface mi in SellItems)
        {
            mi.SetSellSequence(true, i++);
            if (i > 99)
            {
                i = 99;
                sellFull = true;
                break;
            }
        }
    }

    public void _OnClickMaterialInter(int Id)
    {
        UserItem ui = UserManager.CurUserInfo.FindItemById(Id);
        MaterialItemInterface mi = GetMaterialById(Id);
        if (CurSellItem.Contains(ui))
        {
            CurSellItem.Remove(ui);
            SellItems.Remove(mi);
            mi.SetSelect(false);
            mi.SetSellSequence(false);
            sellFull = false;
        }
        else if (sellFull)
        {
            return;
        }
        else
        {
            CurSellItem.Add(ui);
            SellItems.Add(mi);
            mi.SetSelect(true);
        }
        int allPrice = 0;
        foreach (UserItem i in CurSellItem)
        {
            allPrice = allPrice + i.CurItemData.Price;
        }
        SetPrice(allPrice);
        SetSellNum();
    }

    public void _OnLongPressMaterialInter(int Id)
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
        if(CurSellItem.Count != 0)
        {
            JsonArray ids = new JsonArray();
            foreach (UserItem ui in CurSellItem)
            {
                ids.Add(ui.UserItemId);
            }
            JsonObject args = new JsonObject();
            args.Add("house_ids", ids);
            SocketCenter.Request(GameRouteConfig.SellItem, args, (result) =>
            {
                if (result.Code == SocketResult.ResultCode.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        foreach (UserItem ui in CurSellItem)
                        {
                            UserManager.CurUserInfo.UserItems.Remove(ui);
                            bagControl.RemoveItemById(ui.UserItemId);
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
        CurSellItem.Clear();
        foreach (MaterialItemInterface mi in SellItems)
        {
            mi.SetSellSequence(false);
            mi.SetSelect(false);
        }
        SellItems.Clear();
        SetPrice(0);
        sellFull = false;
    }

    void OnDisable()
    {
        CancleAllSell();
        bagControl.ClearMaterialBag();
        SellButton.inter = sellSwitch;
        ClearButton.inter = sellSwitch;
    }
}
