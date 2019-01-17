using UnityEngine;
using System.Collections;

public class PetUIController : MonoBehaviour
{
    public UILabel elementTypeLbl;
    public UILabel levelLbl;
    public UILabel hpLbl;
    public UILabel atkLbl;
    public UILabel costLbl;
    public UISprite baseBrick;

    /// <summary>
    /// 怪物的位置
    /// </summary>
    public int position;

    /// <summary>
    /// 怪物的usermonsterID
    /// </summary>
    public int userMonsterID;

    public DungeonEnum.ElementAttributes elementType;

    /// <summary>
    /// 设置怪物
    /// </summary>
    /// <param name="PartnerLevel">等级</param>
    /// <param name="PartnerHP">血量</param>
    /// <param name="PartNerATK">攻击</param>
    /// <param name="PartnerCost">领导力</param>
    /// <param name="PetAnime">动画预设</param>
    /// <param name="PartnerElementType">元素类型</param>
    public void SetPet(
        int PartnerLevel,
        int PartnerHP,
        int PartNerATK,
        int PartnerCost,
        string PetAnimeID,
        DungeonEnum.ElementAttributes PartnerElementType,
        int UserMonsterID
        )
    {
        UserPet u = UserManager.CurUserInfo.FindPetById(UserMonsterID);
        if (PartnerLevel >= UserManager.CurUserInfo.FindPetById(UserMonsterID).CurPetData.MaxLevel)
        {
            levelLbl.text = "Lv.MAX";
        }
        else
        {
            levelLbl.text = "Lv." + PartnerLevel.ToString();
        }

        if(PartnerHP > u.CurHp)
        {
            hpLbl.color = Color.green;
        }
        else if(PartnerHP < u.CurHp)
        {
            hpLbl.color = Color.red;
        }
        else
        {
            hpLbl.color = new Color(250f / 255f, 200f / 255f, 95f / 255f);
        }
        hpLbl.text = PartnerHP.ToString();

        if (PartNerATK > u.CurAtk)
        {
            atkLbl.color = Color.green;
        }
        else if (PartNerATK < u.CurAtk)
        {
            atkLbl.color = Color.red;
        }
        else
        {
            atkLbl.color = new Color(250f / 255f, 200f / 255f, 95f / 255f);
        }
        atkLbl.text = PartNerATK.ToString();

        //if (u.CurPetData.PetSkillData != null)
        //{
        //    string tempName;
        //    if (u.CurPetData.PetSkillData.Name.Contains(" "))
        //    {
        //        tempName = u.CurPetData.PetSkillData.Name.Split(' ')[0];
        //    }
        //    else
        //    {
        //        tempName = u.CurPetData.PetSkillData.Name;
        //    }
        //    costLbl.text = tempName;
        //}
        //else
        //{
        //    costLbl.text = "";
        //}

        costLbl.text = u.CurPetData.PCost.ToString();
        costLbl.color = new Color(250f / 255f, 200f / 255f, 95f / 255f);

        GameObject temp;
        temp = Resources.Load("PreFabs/Characters/" + PetAnimeID + "60") as GameObject;
        if (temp == null)
            temp = Resources.Load("PreFabs/Characters/S160") as GameObject;

        GameObject S_Animation = null;
        if (temp != null)
        {
            S_Animation = NGUITools.AddChild(gameObject, temp);
        }

        S_Animation.transform.localPosition = new Vector3(-8, 23, -10 * position);

        S_Animation.transform.localScale = new Vector3(50, 50, 1);
        S_Animation.name = "CharacterAnime";
        baseBrick.spriteName = GetBaseSpriteName(PartnerElementType);
        elementType = PartnerElementType;

        userMonsterID = UserMonsterID;

        SetLayer(S_Animation.transform, LayerHelper.Unit);
    }

    string GetBaseSpriteName(DungeonEnum.ElementAttributes type)
    {
        switch ((int)type)
        {
            case 0: return "";
            case 1: return "element_brick_earth";
            case 2: return "element_brick_fire";
            case 3: return "element_brick_wind";
            case 4: return "element_brick_water";
            default: return "";
        }
    }

    /// <summary>
    /// 设置transform的layer为BasicFX以及子物体Sprite renderer组件的sorting layer渲染层级为top（不为default即可）
    /// </summary>
    /// <param name="trans">GameObject的transform</param>
    static public void SetLayer(Transform trans, int tarLayer)
    {
        if (trans.GetComponent<UIWidget>() == null)
        {
            trans.gameObject.layer = tarLayer;
        }
        foreach (Transform t in trans.transform)
        {
            SetLayer(t, tarLayer);
        }
    }
}
