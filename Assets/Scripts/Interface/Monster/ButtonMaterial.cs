using UnityEngine;
using System.Collections;

public class ButtonMaterial : MonoBehaviour
{
    public PetData MaterialPet;
    public ItemData MaterialItem;

    public GameObject PetDetailPanel;
    public GameObject ItemDetailPanel;

    public UITexture Icon;
    public UISprite Element;
    public SetStars Rank;
    public UILabel Count;

    public void SetMaterial(string Id, int count)
    {
        if(string.IsNullOrEmpty(Id))
        {
            MaterialPet = null;
            MaterialItem = null;
            SetItemTexture(Icon, Rank, Element, Count, null);
        }
        else if(ConfigManager.ItemConfig.GetItemById(Id) != null)
        {
            MaterialItem = ConfigManager.ItemConfig.GetItemById(Id);
            MaterialPet = null;
            SetItemTexture(Icon, Rank, Element, Count, MaterialItem.Id);
        }
        else
        {
            MaterialItem = null;
            MaterialPet = ConfigManager.PetConfig.GetPetById(Id);
            SetMaterialTexture(Icon, Rank, Element, Count, MaterialPet.Id, count);
        }
    }

    void SetMaterialTexture(UITexture MaterialTexture, SetStars Stars, UISprite Element, UILabel Number, string PetID, int count)
    {
        if (string.IsNullOrEmpty(PetID))
        {
            Texture t = null;
            MaterialTexture.mainTexture = t;
            Element.spriteName = "icon_nomaterial";
            Number.text = "";
            Stars.GetComponent<SetStars>().SetStar(0);
        }
        else
        {
            PetData pdata = ConfigManager.PetConfig.GetPetById(PetID);
            SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(pdata.SkinId);
            if (skinData != null)
            {
                MaterialTexture.depth = 6;
                Element.depth = 7;
                Texture t = Resources.Load<Texture>("Atlas/PetAvatars/" + skinData.IconId);
                MaterialTexture.mainTexture = t;
                Element.spriteName = returnElement(pdata.PetPro);
                Stars.GetComponent<SetStars>().SetStar(pdata.Rank);
                Number.text = "持有 " + count.ToString();
            }
        }
    }

    void SetItemTexture(UITexture Item, SetStars Stars, UISprite Base, UILabel Number, string ItemID)
    {
        if (string.IsNullOrEmpty(ItemID))
        {
            Texture t = null;
            Item.mainTexture = t;
            Element.spriteName = "icon_nomaterial";
            Number.text = "";
            Stars.GetComponent<SetStars>().SetStar(0);
        }
        else
        {
            ItemData iData = ConfigManager.ItemConfig.GetItemById(ItemID);
            Item.depth = 7;
            Base.depth = 6;
            Base.spriteName = "material_bg";
            Item.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + ConfigManager.SkinConfig.GetSkinDataById(iData.SkinId).IconId);
            Stars.SetStar(iData.Rank);
            Number.text = "持有 " + FindItem(ItemID).ToString();
        }
    }

    public int FindPet(string PetID)
    {
        int count = 0;
        foreach (UserPet up in UserManager.CurUserInfo.UserPets)
        {
            if (up.CurPetData.Id == PetID)
            {
                count++;
            }
        }
        return count;
    }

    public int FindItem(string ItemID)
    {
        int count = 0;
        foreach(UserItem ui in UserManager.CurUserInfo.UserItems)
        {
            if(ui.CurItemData.Id == ItemID)
            {
                count++;
            }
        }
        return count;
    }

    string returnElement(DungeonEnum.ElementAttributes Type)
    {
        switch ((int)Type)
        {
            case 0: return "";
            case 1: return "monster_thum_green";
            case 2: return "monster_thum_red";
            case 3: return "monster_thum_yellow";
            case 4: return "monster_thum_blue";
            default: return "";
        }
    }

    public void ClickToDetail(UserPet u)
    {
        PetDetailPanel.SetActive(true);
        PetDetailPanel.GetComponent<SetMonsterDetail>().SetDetail(u);
    }

    public void ClickToDetail(PetData p)
    {
        UserPet up = new UserPet(p.Id, 1);
        PetDetailPanel.SetActive(true);
        PetDetailPanel.GetComponent<SetMonsterDetail>().SetDetail(up);
    }

    void OnClick()
    {
        if(MaterialPet != null)
        {
            UserPet up = new UserPet(MaterialPet.Id, 1);
            PetDetailPanel.SetActive(true);
            PetDetailPanel.GetComponent<SetMonsterDetail>().SetDetail(up);
        }
        else if(MaterialItem != null)
        {
            ItemDetailPanel.SetActive(true);
            ItemDetailPanel.GetComponent<MaterialDetail>().SetDetail(MaterialItem.Id);
        }
    }
}
