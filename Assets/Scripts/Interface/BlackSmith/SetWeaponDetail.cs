using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class SetWeaponDetail : MonoBehaviour, _ButtonMakeWeapon 
{
    public ButtonMakeWeapon makeWeaponButton;

    public PlayerAvata playerAnime;

    public SetUser UserInfo;

    #region NGUI
    public UILabel name;
    public UITexture weaponIcon;
    public UISprite weaponElementType;
    public UILabel maxLevel;
    public UIGrid stars_weapon;
    public UILabel atk;

    public GameObject Skill_1;

    public UILabel Skill_Name;
    public UILabel Skill_Description;
    public UISprite Skill_Map;
    public UILabel Skill_Chain;
    public UILabel Skill_Cd;

    public GameObject Skill_2;

    public UILabel Skill_Name_2;
    public UILabel Skill_Description_2;
    public UISprite Skill_Map_2;
    public UILabel Skill_Chain_2;
    public UILabel Skill_Cd_2;

    public UITexture material_1;
    public UISprite bg_1;
    public UILabel materialNum_1;
    public UILabel materialQuantity_1;
    public UIGrid stars_1;

    public UITexture material_2;
    public UISprite bg_2;
    public UILabel materialNum_2;
    public UILabel materialQuantity_2;
    public UIGrid stars_2;

    public UITexture material_3;
    public UISprite bg_3;
    public UILabel materialNum_3;
    public UILabel materialQuantity_3;
    public UIGrid stars_3;

    public UITexture material_4;
    public UISprite bg_4;
    public UILabel materialNum_4;
    public UILabel materialQuantity_4;
    public UIGrid stars_4;

    public UITexture material_5;
    public UISprite bg_5;
    public UILabel materialNum_5;
    public UILabel materialQuantity_5;
    public UIGrid stars_5;

    public UILabel goldNeed;

    public UILabel disableInfo;
    #endregion

    public GameObject Button_1;
    public GameObject Button_2;
    public GameObject Button_3;
    public GameObject Button_4;
    public GameObject DetailView;

    int leftNum_1;
    int leftNum_2;
    int leftNum_3;
    int leftNum_4;
    int leftNum_5;
    string sId;

    /// <summary>
    /// 设置武器制作详细信息
    /// </summary>
    public void SetWeaponInfo(
        string WeaponID,
        //int Cit,
        //string SkillName,
        //int SkillChain,   
        //string SkillMap,
        //string SkillEffect,
        string CskillName,
        string CskillMap_1,
        string CskillMap_2,
        string CskillMap_3,
        int CskillNum_1, 
        int CskillNum_2,
        int CskillNum_3,
        string CskillEffect,
        string MaterialID_1,
        int NeedNum_1,
        int HaveNum_1,
        string MaterialID_2,
        int NeedNum_2,
        int HaveNum_2,
        string MaterialID_3,
        int NeedNum_3,
        int HaveNum_3,
        string MaterialID_4,
        int NeedNum_4,
        int HaveNum_4,
        string MaterialID_5,
        int NeedNum_5,
        int HaveNum_5,
        int GoldNeed)
    {
        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(WeaponID);
        name.text = hd.Name;
        playerAnime.AddAvatarWareOutline(hd.SkinId, DungeonEnum.FaceDirection.None, true);

        sId = hd.SkinId;

        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(hd.SkinId);
        if (skinData != null)
        {
            weaponIcon.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
        }
        maxLevel.text = hd.LvlMax.ToString();
        weaponElementType.spriteName = Tools.GetHardwareElement(hd.Element);
        stars_weapon.GetComponent<SetStars>().SetStar(hd.Rank);
        atk.text = hd.Attack.ToString();

        if (ConfigManager.SkillConfig.GetSkillById(hd.SkillAffix1) != null)
        {
            Skill_1.SetActive(true);
            SkillData skill = ConfigManager.SkillConfig.GetSkillById(hd.SkillAffix1);
            Skill_Name.text = skill.Name;
            Skill_Description.text = skill.Description;
            Skill_Map.spriteName = skill.SkillIcon;
            Skill_Chain.text = skill.Bparameter.ToString();
            if (skill.SkillCd > 0) Skill_Cd.text = skill.SkillCd.ToString();
            else Skill_Cd.text = "--";
        }
        else
        {
            Skill_1.SetActive(false);
        }

        if (ConfigManager.SkillConfig.GetSkillById(hd.SkillAffix2) != null)
        {
            Skill_2.SetActive(true);
            SkillData skill = ConfigManager.SkillConfig.GetSkillById(hd.SkillAffix2);
            Skill_Name_2.text = skill.Name;
            Skill_Description_2.text = skill.Description;
            Skill_Map_2.spriteName = skill.SkillIcon;
            Skill_Chain_2.text = "0";
            if (skill.SkillCd > 0) Skill_Cd_2.text = skill.SkillCd.ToString();
            else Skill_Cd_2.text = "--";
        }
        else
        {
            Skill_2.SetActive(false);
        }
        

        leftNum_1 = HaveNum_1 - NeedNum_1;
        leftNum_2 = HaveNum_2 - NeedNum_2;
        leftNum_3 = HaveNum_3 - NeedNum_3;
        leftNum_4 = HaveNum_4 - NeedNum_4;
        leftNum_5 = HaveNum_5 - NeedNum_5;

        SetMaterial(material_1, bg_1, stars_1, materialNum_1, NeedNum_1, materialQuantity_1, HaveNum_1, MaterialID_1);
        SetMaterial(material_2, bg_2, stars_2, materialNum_2, NeedNum_2, materialQuantity_2, HaveNum_2, MaterialID_2);
        SetMaterial(material_3, bg_3, stars_3, materialNum_3, NeedNum_3, materialQuantity_3, HaveNum_3, MaterialID_3);
        SetMaterial(material_4, bg_4, stars_4, materialNum_4, NeedNum_4, materialQuantity_4, HaveNum_4, MaterialID_4);
        SetMaterial(material_5, bg_5, stars_5, materialNum_5, NeedNum_5, materialQuantity_5, HaveNum_5, MaterialID_5);

        if (HaveNum_1 < NeedNum_1 || HaveNum_2 < NeedNum_2 || HaveNum_3 < NeedNum_3 || HaveNum_4 < NeedNum_4 || HaveNum_5 < NeedNum_5)
        {
            disableInfo.text = "素材不足";
            makeWeaponButton.GetComponent<UIWidget>().color = Color.gray;
            makeWeaponButton.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            disableInfo.text = "";
            makeWeaponButton.GetComponent<UIWidget>().color = Color.white;
            makeWeaponButton.GetComponent<BoxCollider>().enabled = true;
        }
        goldNeed.text = GoldNeed.ToString();

        makeWeaponButton.makeWeaponInter = this;
        makeWeaponButton.equipmentID = WeaponID;

        if (!string.IsNullOrEmpty(MaterialID_1))
        {
            UIEventListener.Get(Button_1).onClick = (g) =>
            {
                DetailView.SetActive(true);
                DetailView.GetComponent<MaterialDetail>().SetDetail(MaterialID_1);
            };
        }

        if (!string.IsNullOrEmpty(MaterialID_2))
        {
            UIEventListener.Get(Button_2).onClick = (g) =>
            {
                DetailView.SetActive(true);
                DetailView.GetComponent<MaterialDetail>().SetDetail(MaterialID_2);
            };
        }

        if (!string.IsNullOrEmpty(MaterialID_3))
        {
            UIEventListener.Get(Button_3).onClick = (g) =>
            {
                DetailView.SetActive(true);
                DetailView.GetComponent<MaterialDetail>().SetDetail(MaterialID_3);
            };
        }

        if (!string.IsNullOrEmpty(MaterialID_4))
        {
            UIEventListener.Get(Button_4).onClick = (g) =>
            {
                DetailView.SetActive(true);
                DetailView.GetComponent<MaterialDetail>().SetDetail(MaterialID_4);
            };
        }

    }


    Texture GetIconTexture(string ID)
    {
        Texture t = Resources.Load<Texture>("Atlas/ItemIcons" + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.HardWareConfig.GetHardWareById(ID).SkinId).IconId);
        return t;
    }

    /// <summary>
    /// 设置素材
    /// </summary>
    void SetMaterial(UITexture Material, UISprite Background, UIGrid Stars, UILabel NeedNum, int Nm, UILabel HaveNum, int Hn, string MaterialID)
    {
        if (MaterialID != null && MaterialID != "")
        {
            ItemData id = ConfigManager.ItemConfig.GetItemById(MaterialID);
            if (id != null)
            {
                Material.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + ConfigManager.SkinConfig.GetSkinDataById(id.SkinId).IconId);
                Background.spriteName = "material_bg";
                Stars.GetComponent<SetStars>().SetStar(id.Rank);
                NeedNum.text = "X  " + Nm.ToString();
                HaveNum.text = "持有  " + Hn.ToString();
            }
        }
        else
        {
            Material.mainTexture = null;
            Background.spriteName = "icon_nomaterial";
            NeedNum.text = "";
            HaveNum.text = "";
        }
    }

    /// <summary>
    /// 清除素材的星星
    /// </summary>
    void ClearStar()
    {
        foreach (Transform t in stars_1.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in stars_2.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in stars_3.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in stars_4.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in stars_5.transform)
        {
            Destroy(t.gameObject);
        }
    }


    string GetElement(DungeonEnum.ElementAttributes Type)
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

    /// <summary>
    /// 合成后刷新数量
    /// </summary>
    void RefreashNum()
    {
        if (materialQuantity_1.text != "") materialQuantity_1.text = leftNum_1.ToString();
        if (materialQuantity_2.text != "") materialQuantity_2.text = leftNum_2.ToString();
        if (materialQuantity_3.text != "") materialQuantity_3.text = leftNum_3.ToString();
        if (materialQuantity_4.text != "") materialQuantity_4.text = leftNum_4.ToString();
        if (materialQuantity_5.text != "") materialQuantity_5.text = leftNum_5.ToString();
    }

    void RefreashWeapon(string SkinId)
    {
        playerAnime.ClearAvata();
        playerAnime.AddAvatarWareOutline(SkinId, DungeonEnum.FaceDirection.None, false);
    }

    void OnEnable()
    {
        made = false;
    }

    void OnDisable()
    {
        playerAnime.AddAvatarWareOutline(sId, DungeonEnum.FaceDirection.None, false);
        playerAnime.ClearAvata();
        ClearStar();
    }

    bool made = false;

    public HPUAnimationControl PowUpAnimation;

    public void _OnClickButtonMakeWeapon(string HardwareID)
    {
        if (made) return;
        JsonObject args = new JsonObject();
        args.Add("weapon_id", HardwareID);
        SocketCenter.Request(GameRouteConfig.MakeHardware, args, (result) =>
        {
            if (result.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    JsonArray elements = (JsonArray)result.Data["elements"];
                    UserManager.CurUserInfo.AddElements(elements);
                    JsonArray materials = (JsonArray)result.Data["item_materials"];

                    string[] Ms = new string[5];
                    for (int i = 0; i < (materials.Count > 5 ? 5 :  materials.Count); i++)
                    {
                        int tempId = int.Parse(materials[i].ToString());
                        UserItem material = UserManager.CurUserInfo.FindItemById(tempId);
                        UserManager.CurUserInfo.UserItems.Remove(material);
                        Ms[i] = material.CurItemData.SkinId;
                    }

                    PowUpAnimation.gameObject.SetActive(true);
                    PowUpAnimation.Hbase = UserManager.CurUserInfo.UserWares[UserManager.CurUserInfo.UserWares.Count - 1];
                    PowUpAnimation.SetAnimation(PowUpAnimation.Hbase.CurHardWareData.SkinId, Ms[0], Ms[1], Ms[2], Ms[3], Ms[4]);

                    RefreashNum();
                    RefreashWeapon(sId);
                    UserInfo.gold.text = UserManager.CurUserInfo.Gold.ToString();
                    UserInfo.diamonds.text = UserManager.CurUserInfo.Diamond.ToString();
                    made = true;
                });
            }
        }, null, true,true);
    }

    /// <summary>
    /// 当前场景
    /// </summary>
    public GameObject CurView;

    /// <summary>
    /// 目标场景
    /// </summary>
    public GameObject TargetView;

    /// <summary>
    /// 返回条（用于动画退出）
    /// </summary>
    public GameObject BackBar;

    /// <summary>
    /// 主要界面（动画退出）
    /// </summary>
    public GameObject MainBoard;

    void SwitchMakeView()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
    }

    void SceneSwitch()
    {
        CurView.SetActive(false);
        if (TargetView != null)
        {
            TargetView.SetActive(true);
        }
    }
}
