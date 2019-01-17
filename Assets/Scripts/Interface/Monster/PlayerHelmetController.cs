using UnityEngine;
using System.Collections;

public interface PartyHelmet
{
    void _OnClickHelmet();
    void _OnLongPressHelmet(int UserWareId);
}

public class PlayerHelmetController : MonoBehaviour {

    public UITexture helmet;
    public UILabel helmetLevel;
    public UISprite helmetElementType;
    public PartyHelmet helmetInter;

    public EquipmentBagControl bag;

    public int userEquipmentID = -1;

    public UIGrid EquipmentBag;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="HelmetID">头盔ID</param>
    /// <param name="HelmetLevel">等级</param>
    /// <param name="ElementType">元素类型</param>
    public void PartyHelmetInfo(string HelmetID, int HelmetLevel, DungeonEnum.ElementAttributes ElementType, int UserEquipmentID)
    {
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(HelmetID);
        if (skinData != null)
        {
            helmet.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
        }
        UserWare u = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);
        if (HelmetLevel == u.CurHardWareData.LvlMax)
        {
            helmetLevel.text = "Lv.MAX";
        }
        else
        {
            helmetLevel.text = "Lv." + HelmetLevel.ToString();
        }
        if (UserManager.CurUserInfo.FindUserWare(UserEquipmentID) != null)
        {
            helmetElementType.spriteName = Tools.GetHardwareElement(UserManager.CurUserInfo.FindUserWare(UserEquipmentID).CurHardWareData.Element);
        }
        else
        {
            helmetElementType.spriteName = "";
        }
        userEquipmentID = UserEquipmentID;
    }

    public void RmHelmet()
    {
        helmet.mainTexture = null;
        helmetLevel.text = "";
        helmetElementType.spriteName = "";
        helmet.mainTexture = Resources.Load<Texture>("UI/UI_Assets/Others/icon_add");
    }

    float pressTime = 0f;
    bool press = false;
    public float longPressTime = 1f;

    void OnClick() //短按功能
    {
        if (helmetInter != null && bag != null)
        {
            bag.ClickUserEquipmentID = userEquipmentID;
            if (userEquipmentID != -1)
            {
                GameObject g;
                g = NGUITools.AddChild(EquipmentBag.gameObject, bag.removeItem);
                g.name = "000rmIcon";
                g.GetComponent<RemovePet>().rInter = bag;
            }
            helmetInter._OnClickHelmet();
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
                if (helmetInter != null)
                    helmetInter._OnLongPressHelmet(userEquipmentID);
                press = false;
                pressTime = 0f;
            }
        }
    }
}
