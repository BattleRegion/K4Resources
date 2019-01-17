using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class EquipmentEvolution : MonoBehaviour, EquipmentEvolutionInter
{

    #region NGUI
    public UIButton preButton;
    public UIButton laterButton;

    public UIButton evolutionButton;

    public SetUser UserInfo;
    /// <summary>
    /// 进化前预设和进化后预设
    /// </summary>
    public PlayerAvata Pre;
    public PlayerAvata Later;

    public UITexture preIcon;
    public UISprite preElement;
    public SetStars preStar;
    public UITexture laterIcon;
    public UISprite laterElement;
    public SetStars laterStar;

    public UILabel preName;
    public UILabel laterName;
    public UILabel preLv;
    public UILabel preMaxLv;
    public UILabel laterLv;
    public UILabel laterMaxLv;

    public UILabel preAtkOrDef;
    public UILabel laterAtkOrDef;

    public UILabel preAtk;
    public UILabel laterAtk;
    //public UILabel preSkillLv;
    //public UILabel laterSkillLv;
    //public UISprite preCskillChain;
    //public UISprite preCskill;
    //public UISprite laterCskill;
    //public UISprite laterCskillChain;
    public UILabel goldNeed;
    public UILabel tips;

    public UITexture material_1;
    public UIGrid stars_1;
    public UISprite bg_1;
    public UISprite element_1;
    public UILabel haveNum_1;

    public UITexture material_2;
    public UIGrid stars_2;
    public UISprite bg_2;
    public UISprite element_2;
    public UILabel haveNum_2;

    public UITexture material_3;
    public UIGrid stars_3;
    public UISprite bg_3;
    public UISprite element_3;
    public UILabel haveNum_3;

    public UITexture material_4;
    public UIGrid stars_4;
    public UISprite bg_4;
    public UISprite element_4;
    public UILabel haveNum_4;

    public UITexture material_5;
    public UIGrid stars_5;
    public UISprite bg_5;
    public UISprite element_5;
    public UILabel haveNum_5;
    #endregion

    public GameObject CoinWarning;

    int needNum;
    int haveNum;

    string mskinId_1;
    string mskinId_2;
    string mskinId_3;
    string mskinId_4;
    string mskinId_5;

    public HardwareSkillMaterial MaterialButton_1;
    public HardwareSkillMaterial MaterialButton_2;
    public HardwareSkillMaterial MaterialButton_3;
    public HardwareSkillMaterial MaterialButton_4;

    int CurrentUid;

    public void SetEvolutionInfo(int Uid)
    {
        CurrentUid = Uid;
        UserWare u = UserManager.CurUserInfo.FindUserWare(Uid);
        UserWare uEvo = new UserWare(u.CurHardWareData.Evo, u.CurHardWareData.LvlMax);
        if((int)u.CurHardWareData.Style < 5)
        {
            SetEvolutionInfo(u.CurHardWareData.Name, uEvo.CurHardWareData.Name, u.UserWareId, u.CurHardWareData.Id, uEvo.CurHardWareData.Id, u.Level, u.CurHardWareData.LvlMax, uEvo.Level, uEvo.CurHardWareData.LvlMax, (int)u.CurAtk, (int)uEvo.CurAtk, u.CurHardWareData.EvoCoin, u.CurHardWareData.EvoN1, u.CurHardWareData.EvoN2, u.CurHardWareData.EvoN3, u.CurHardWareData.EvoN4, u.CurHardWareData.EvoN5);
        }
        else
        {
            SetEvolutionInfo(u.CurHardWareData.Name, uEvo.CurHardWareData.Name, u.UserWareId, u.CurHardWareData.Id, uEvo.CurHardWareData.Id, u.Level, u.CurHardWareData.LvlMax, uEvo.Level, uEvo.CurHardWareData.LvlMax, (int)u.CurDef, (int)uEvo.CurDef, u.CurHardWareData.EvoCoin, u.CurHardWareData.EvoN1, u.CurHardWareData.EvoN2, u.CurHardWareData.EvoN3, u.CurHardWareData.EvoN4, u.CurHardWareData.EvoN5);
        }
    }

    /// <summary>
    /// 进化界面初始化
    /// </summary>
    public void SetEvolutionInfo(
        string PreName,
        string LaterName,
        int CurEvoUserWareID,
        string PreID,
        string LaterID,
        int PreLv,
        int PreMaxLv,
        int LaterLv,
        int LaterMaxLv,
        int PreAtkOrDef,
        int LaterAtkOrDef,
        int GoldNeed,
        string Material_1,
        string Material_2,
        string Material_3,
        string Material_4,
        string Material_5)
    {
        HardWareData hdPre = ConfigManager.HardWareConfig.GetHardWareById(PreID);
        HardWareData hdLater = ConfigManager.HardWareConfig.GetHardWareById(LaterID);
        SkinConfigData skin1 = ConfigManager.SkinConfig.GetSkinDataById(hdPre.SkinId);
        SkinConfigData skin2 = ConfigManager.SkinConfig.GetSkinDataById(hdLater.SkinId);

        needNum = 0;
        haveNum = 0;

        preName.text = PreName;
        laterName.text = LaterName;

        //preElement.spriteName = returnElement(hdPre.Element);
        //laterElement.spriteName = returnElement(hdLater.Element);

        //Texture t = Resources.Load<Texture>("Atlas/ItemIcons/" + skin1.IconId);
        //preIcon.mainTexture = t;
        //t = Resources.Load<Texture>("Atlas/ItemIcons/" + skin2.IconId);
        //laterIcon.mainTexture = t;

        //preButton.GetComponent<ButtonMaterial>().userPetID = CurEvoUserWareID;
        //laterButton.GetComponent<ButtonMaterial>().petID = LaterID;

        if ((int)hdPre.Style < 5)
        {
            Pre.AddAvataWare(hdPre.SkinId, DungeonEnum.FaceDirection.None);
            Later.AddAvataWare(hdLater.SkinId, DungeonEnum.FaceDirection.None);
        }
        else
        {
            Pre.AddAvataWare(hdPre.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Later.AddAvataWare(hdLater.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }

        PetUIController.SetLayer(Pre.transform, LayerHelper.Unit);
        PetUIController.SetLayer(Later.transform, LayerHelper.Unit);

        preMaxLv.text = "/" + PreMaxLv.ToString();

        laterLv.text = PreMaxLv.ToString();
        laterMaxLv.text = "/" + LaterMaxLv.ToString();

        if ((int)ConfigManager.HardWareConfig.GetHardWareById(PreID).Style < 5)
        {
            preAtkOrDef.text = "攻击：";
            laterAtkOrDef.text = "攻击：";
        }
        else
        {
            preAtkOrDef.text = "防御：";
            laterAtkOrDef.text = "防御：";
        }
        preAtk.text = PreAtkOrDef.ToString();
        laterAtk.text = LaterAtkOrDef.ToString();

        evolutionButton.GetComponent<ButtonEquipmentEvolution>().id = LaterID;


        goldNeed.text = GoldNeed.ToString();

        if(GoldNeed > UserManager.CurUserInfo.Gold)
        {
            CoinWarning.SetActive(true);
        }
        else
        {
            CoinWarning.SetActive(false);
        }

        MaterialButton_1.SetMaterial(Material_1, GetMaterialNum(Material_1));
        MaterialButton_2.SetMaterial(Material_2, GetMaterialNum(Material_2));
        MaterialButton_3.SetMaterial(Material_3, GetMaterialNum(Material_3));
        MaterialButton_4.SetMaterial(Material_4, GetMaterialNum(Material_4));
        mskinId_1 = MaterialButton_1.skinId;
        mskinId_2 = MaterialButton_2.skinId;
        mskinId_3 = MaterialButton_3.skinId;
        mskinId_4 = MaterialButton_4.skinId;

        if (PreLv < PreMaxLv)
        {
            tips.text = "等级不够";
            evolutionButton.GetComponent<UIWidget>().color = Color.gray;
            evolutionButton.GetComponent<BoxCollider>().enabled = false;
            preLv.color = Color.red;
        }
        else if (haveNum < needNum)
        {
            tips.text = "素材不足";
            evolutionButton.GetComponent<UIWidget>().color = Color.gray;
            evolutionButton.GetComponent<BoxCollider>().enabled = false;
            preLv.color = Color.red;
        }
        else
        {
            tips.text = "";
            evolutionButton.GetComponent<UIWidget>().color = Color.white;
            evolutionButton.GetComponent<BoxCollider>().enabled = true;
            preLv.color = Color.white;
        }
        preLv.text = PreLv.ToString();
    }

    int GetMaterialNum(string MaterialID)
    {
        if (ConfigManager.ItemConfig.GetItemById(MaterialID) != null)
        {
            return FindItem(MaterialID);
        }
        else
        {
            return FindWare(MaterialID);
            }
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
            if (uw.CurHardWareData.Id == WareID && CurrentUid != uw.UserWareId) count++;
        }
        return count;
    }


    /// <summary>
    /// 设置素材
    /// </summary>
    void SetMaterial(UITexture Material, UISprite Background, UIGrid Stars, UISprite Element, UILabel HaveNum, string MaterialID, ref string SkinId)
    {
        
        if (MaterialID != null && MaterialID != "")
        {
            needNum++;
            ItemData id = ConfigManager.ItemConfig.GetItemById(MaterialID);
            SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(id.SkinId);
            if (id != null)
            {
                SkinId = id.SkinId;
                Material.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
                Background.spriteName = "material_bg";
                Background.depth = 7;
                Element.spriteName = "";
                Stars.GetComponent<SetStars>().SetStar(id.Rank);
                int hN = GetMaterialNum(MaterialID);
                HaveNum.text = "持有  " + hN.ToString();
                if (hN > 0)
                {
                    haveNum++;
                }
            }
            else
            {
                HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(MaterialID);
                if (hd != null)
                {
                    SkinConfigData skin = ConfigManager.SkinConfig.GetSkinDataById(hd.SkinId);
                    SkinId = hd.SkinId;
                    if (skin != null)
                        Material.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + skin.IconId);
                    Background.spriteName = "weapon_thum_base";
                    Background.depth = 9;
                    Element.spriteName = returnElement(hd.Element);
                    Stars.GetComponent<SetStars>().SetStar(hd.Rank);
                    int num = FindWare(MaterialID);
                    HaveNum.text = "持有  " + num.ToString();
                    if (num > 0)
                    {
                        haveNum++;
                    }
                }
            }
        }
        else
        {
            SkinId = null;
            Material.mainTexture = null;
            Element.spriteName = "";
            Background.spriteName = "icon_nomaterial";
            HaveNum.text = "";
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

    void OnEnable()
    {
        evolutionButton.GetComponent<ButtonEquipmentEvolution>().inter = this;
    }

    void OnDisable()
    {
        Pre.ClearAvata();
        Later.ClearAvata();
    }

    /// <summary>
    /// 动画
    /// </summary>
    public HPUAnimationControl PowUpAnimation;

    public void _OnClickEquipmentEvoButton(int UserWareId)
    {
        UserWare uw = UserManager.CurUserInfo.FindUserWare(UserWareId);
        JsonObject args = new JsonObject();
        args.Add("house_id", UserWareId);
        SocketCenter.Request(GameRouteConfig.EvoHardware, args, (result) =>
        {
            if (result.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    Debug.Log(result.Data);

                    JsonObject data = (JsonObject)result.Data["weapon"];
                    UserWare newWare = new UserWare(data);

                    UserManager.CurUserInfo.UserWares.Remove(uw);
                    UserManager.CurUserInfo.UserWares.Add(newWare);
                    JsonArray consum = (JsonArray)result.Data["consumes"];
                    UserManager.CurUserInfo.AddUserElements(consum);
                    JsonArray materials = (JsonArray)result.Data["item_materials"];

                    string[] Ms = new string[5];
                    for (int i = materials.Count - 1; i >= 0; i--)
                    {
                        int tempId = int.Parse(materials[i].ToString());
                        UserWare material = UserManager.CurUserInfo.FindUserWare(tempId);
                        if(material != null)
                        {
                            UserManager.CurUserInfo.UserWares.Remove(material);
                            Ms[i] = material.CurHardWareData.SkinId;
                        }
                        else
                        {
                            UserItem item_material = UserManager.CurUserInfo.FindItemById(tempId);
                            UserManager.CurUserInfo.UserItems.Remove(item_material);
                            Ms[i] = item_material.CurItemData.SkinId;
                        }
                    }

                    UserInfo.SetInfo();

                    PowUpAnimation.gameObject.SetActive(true);
                    PowUpAnimation.SetEvolutionInfo(uw, newWare);

                    PowUpAnimation.SetAnimation(uw.CurHardWareData.SkinId, Ms[0], Ms[1], Ms[2], Ms[3], Ms[4]);
                });
            }
        }, null, true,true);
    }
}
