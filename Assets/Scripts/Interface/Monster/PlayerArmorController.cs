using UnityEngine;
using System.Collections;

public interface PartyArmor
{
    void _OnClickArmor();
    void _OnLongPressArmor(int UserWareId);
}

public class PlayerArmorController : MonoBehaviour 
{
    public UITexture armor;
    public UILabel armorLevel;
    public UISprite armorElementType;
    public PartyArmor armorInter;

    public EquipmentBagControl bag;

    public int userEquipmentID = -1;

    public UIGrid EquipmentBag;

    public void PartyArmorInfo(string ArmorID, int ArmorLevel, DungeonEnum.ElementAttributes ElementType, int UserEquipmentID)
    {
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(ArmorID);
        if (skinData != null)
        {
            armor.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
        }
        UserWare u = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);
        if (ArmorLevel == u.CurHardWareData.LvlMax)
        {
            armorLevel.text = "Lv.MAX";
        }
        else
        {
            armorLevel.text = "Lv." + ArmorLevel.ToString();
        }
        if (UserManager.CurUserInfo.FindUserWare(UserEquipmentID) != null)
        {
            armorElementType.spriteName = Tools.GetHardwareElement(UserManager.CurUserInfo.FindUserWare(UserEquipmentID).CurHardWareData.Element);
        }
        else
        {
            armorElementType.spriteName = "";
        }
        userEquipmentID = UserEquipmentID;
    }

    public void RmArmor()
    {
        armor.mainTexture = null;
        armorLevel.text = "";
        armorElementType.spriteName = "";
        armor.mainTexture = Resources.Load<Texture>("UI/UI_Assets/Others/icon_add");
    }


    float pressTime = 0f;
    bool press = false;
    public float longPressTime = 1f;

    void OnClick() //短按功能
    {
		if (armorInter != null && bag != null)
        {
            bag.ClickUserEquipmentID = userEquipmentID;
            if (userEquipmentID != -1)
            {
                GameObject g;
                g = NGUITools.AddChild(EquipmentBag.gameObject, bag.removeItem);
                g.name = "000rmIcon";
                g.GetComponent<RemovePet>().rInter = bag;
            }
            armorInter._OnClickArmor();
        }
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            press = true;
        }
        else
        {
            press = false;
            pressTime = 0f;
        }
    }


    void Update()   //计时实现按钮长按功能
    {
        if (press)
        {
            pressTime += Time.deltaTime;
            if (pressTime > longPressTime)
            {
                if (armorInter != null)
                    armorInter._OnLongPressArmor(userEquipmentID);
                press = false;
                pressTime = 0f;
            }
        }
    }
}
