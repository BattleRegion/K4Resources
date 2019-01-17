using UnityEngine;
using System.Collections;

public class HardwareSkillMaterial : MonoBehaviour
{
    public HardWareData MaterialWare;
    public ItemData MaterialItem;

    public GameObject ItemDetail;
    public GameObject WeaponDetail;
    public GameObject ArmorDetail;

    public UILabel Count;
    public UITexture Icon;
    public UISprite Background;
    public UISprite Element;
    public UISprite WareFrame;
    public SetStars Stars;

    public string skinId;

    public void SetMaterial(string Id, int Count)
    {
        if (string.IsNullOrEmpty(Id))
        {
            MaterialWare = null;
            MaterialItem = null;
            SetItemTexture(null);
        }
        else if (ConfigManager.ItemConfig.GetItemById(Id) != null)
        {
            MaterialItem = ConfigManager.ItemConfig.GetItemById(Id);
            skinId = MaterialItem.SkinId;
            MaterialWare = null;
            SetItemTexture(Id);
        }
        else
        {
            MaterialItem = null;
            MaterialWare = ConfigManager.HardWareConfig.GetHardWareById(Id);
            skinId = MaterialWare.SkinId;
            SetWareTexture(Id, Count);
        }
    }

    void SetItemTexture(string ItemID)
    {
        WareFrame.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(ItemID))
        {
            Texture t = null;
            Icon.mainTexture = t;
            Background.spriteName = "icon_nomaterial";
            Count.text = "";
            Stars.GetComponent<SetStars>().SetStar(0);
        }
        else
        {
            ItemData iData = ConfigManager.ItemConfig.GetItemById(ItemID);
            Background.spriteName = "material_bg";
            Icon.mainTexture = Resources.Load<Texture>(Tools.GetIconPath(ItemID));
            Stars.SetStar(iData.Rank);
            Count.text = "持有 " + FindItem(ItemID).ToString();
        }
    }

    void SetWareTexture(string WareID, int WareCount)
    {
        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(WareID);
        WareFrame.gameObject.SetActive(true);
        Element.spriteName = returnElement(hd.Element);
        WareFrame.spriteName = "weapon_thum_base";
        Background.spriteName = "bg_hardware";
        SkinConfigData skin = ConfigManager.SkinConfig.GetSkinDataById(hd.SkinId);
        Icon.mainTexture = Resources.Load<Texture>(Tools.GetIconPath(WareID));
        Stars.SetStar(hd.Rank);
        Count.text = "持有 " + WareCount.ToString();
    }

    int FindItem(string ItemID)
    {
        int count = 0;
        foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
        {
            if (ui.CurItemData.Id == ItemID)
            {
                count++;
            }
        }
        return count;
    }

    int FindWare(string WareID)
    {
        int count = 0;
        foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
        {
            if (uw.CurHardWareData.Id == WareID) count++;
        }
        return count;
    }

    string returnElement(DungeonEnum.ElementAttributes Type)
    {
        switch ((int)Type)
        {
            case 0: return "";
            case 1: return "element_type_earth";
            case 2: return "element_type_fire";
            case 3: return "element_type_wind";
            case 4: return "element_type_water";
            default: return "";
        }
    }

    void OnClick()
    {
        if(MaterialItem != null)
        {
            ItemDetail.SetActive(true);
            ItemDetail.GetComponent<MaterialDetail>().SetDetail(MaterialItem.Id);
        }
        else if(MaterialWare != null)
        {
            UserWare uw = new UserWare(MaterialWare.Id, 1);
            if((int)MaterialWare.Style < 5)
            {
                WeaponDetail.SetActive(true);
                WeaponDetail.GetComponent<WeaponDetail>().SetDetail(uw);
            }
            else
            {
                ArmorDetail.SetActive(true);
                ArmorDetail.GetComponent<ArmorDetail>().SetDetail(uw);
            }
        }
    }
}
