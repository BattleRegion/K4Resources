using UnityEngine;
using System.Collections;

public interface EquipmentMenuItemInter
{
    void _OnClickMenuItem(string equipmentID);
}

public class EquipmentMenuItemInterface : MonoBehaviour 
{
    /// <summary>
    /// UI
    /// </summary>
    public UITexture icon;
    public UILabel name;
    public UISprite elementType;

    public UILabel atkOrDef;

    public UILabel atkOrDefNum;
    public UISprite cskillOrSkill;
    public UISprite cskill_Type_1;
    public UILabel cskill_Num_1;
    public UISprite cskill_Type_2;
    public UILabel cskill_Num_2;
    public UISprite cskill_Type_3;
    public UILabel cskill_Num_3;
    public UILabel haveNum;
    public UISprite couldMake;   //材料足够之后可以制作而显示的小锤子

    public UIGrid stars;

    /// <summary>
    ///  装备ID
    /// </summary>
    public string ID;

    public EquipmentMenuItemInter equipInter;

    /// <summary>
    /// 初始化武器菜单条目
    /// </summary>
    public void SetMenuItem(
        string EquipmentID,
        string Name,
        DungeonEnum.ElementAttributes Type,
        int StarNum,
        int Atk,
        int Def,
        int Cs1_Num,
        int Cs2_Num,
        int Cs3_Num,
        int HaveNum,
        bool CouldMake)
    {
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.HardWareConfig.GetHardWareById(EquipmentID).SkinId);
        if (skinData != null)
        {
            Texture t = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
            icon.mainTexture = t;
        }

        name.text = Name;
        elementType.spriteName = Tools.GetHardwareElement(ConfigManager.HardWareConfig.GetHardWareById(EquipmentID).Element);

        ID = EquipmentID;

        stars.GetComponent<SetStars>().SetStar(StarNum);

        if (Atk > 0)         //判断武器还是护甲
        {
            atkOrDef.text = "攻击：";
            atkOrDefNum.text = Atk.ToString();
            if (Cs1_Num > 0) cskill_Num_1.text = Cs1_Num.ToString();
            else cskill_Num_1.gameObject.SetActive(false);
            if (Cs2_Num > 0) cskill_Num_2.text = Cs2_Num.ToString();
            else cskill_Num_2.gameObject.SetActive(false);
            if (Cs3_Num > 0) cskill_Num_3.text = Cs3_Num.ToString();
            else cskill_Num_3.gameObject.SetActive(false);
        }
        else
        {
            atkOrDef.text = "防御：";
            atkOrDefNum.text = Def.ToString();
            cskillOrSkill.gameObject.SetActive(false);
        }

        haveNum.text = HaveNum.ToString();

        if (CouldMake)
        { 
            
        }
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
        if (equipInter != null)
        {
            equipInter._OnClickMenuItem(ID);
        }
    }
}
