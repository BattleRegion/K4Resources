using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;


public class Evolution : MonoBehaviour, evolutionBtn
{
    #region NGUI
    public UIButton preButton;
    public UIButton laterButton;
    public UIButton materialButton_1;
    public UIButton materialButton_2;
    public UIButton materialButton_3;
    public UIButton materialButton_4;
    public UIButton materialButton_5;

    public UIButton evolutionButton;

    public PUAnimationControl EvolutionAnimation;


    /// <summary>
    /// 进化前预设和进化后预设
    /// </summary>
    public GameObject Pre;
    public GameObject Later;

    public BagControl evolutionBagControl;

    public UILabel preName;
    public UILabel laterName;
    public UILabel preLv;
    public UILabel preMaxLv;
    public UILabel laterLv;
    public UILabel laterMaxLv;
    //public UILabel preCost;
    //public UILabel laterCost;
    public UILabel preHp;
    public UILabel laterHp;
    public UILabel preAtk;
    public UILabel laterAtk;
    public UILabel preSkillLv;
    public UILabel laterSkillLv;
    public UISprite preCskillChain;
    public UISprite preCskill;
    public UISprite laterCskill;
    public UISprite laterCskillChain;
    public UILabel goldNeed;
    public UILabel tips;

    public UITexture material_1;
    public UIGrid stars_1;
    public UISprite element_1;
    public UILabel num_1;
    public UITexture material_2;
    public UIGrid stars_2;
    public UISprite element_2;
    public UILabel num_2;
    public UITexture material_3;
    public UIGrid stars_3;
    public UISprite element_3;
    public UILabel num_3;
    public UITexture material_4;
    public UIGrid stars_4;
    public UISprite element_4;
    public UILabel num_4;
    public UITexture material_5;
    public UIGrid stars_5;
    public UISprite element_5;
    public UILabel num_5;
    #endregion

    public SetUser UserInfo;

    public GameObject CoinWarning;

    /// <summary>
    /// 素材PETID
    /// </summary>
    string m_1;
    string m_2;
    string m_3;
    string m_4;
    string m_5;

    /// <summary>
    /// 需要几种素材
    /// </summary>
    int needNum;

    /// <summary>
    /// 拥有几种素材
    /// </summary>
    int haveNum;

    int CurrentEvolutionUid;

    public void SetEvolutionInfo(UserPet u)
    {
        UserPet later = new UserPet(u.CurPetData.Evo, u.CurPetData.MaxLevel);
        CurrentEvolutionUid = u.UserPetId;
        SetEvolutionInfo(u.CurPetData.Name, later.CurPetData.Name, u.UserPetId, u.CurPetData.Id, later.CurPetData.Id, u.Level, u.CurPetData.MaxLevel, later.Level, later.CurPetData.MaxLevel, u.CurPetData.PCost, later.CurPetData.PCost, (int)u.CurHp, (int)later.CurHp, (int)u.CurAtk, (int)later.CurAtk, u.CurPetData.EvoCoin, u.CurPetData.EvoN1, u.CurPetData.EvoN2, u.CurPetData.EvoN3, u.CurPetData.EvoN4, u.CurPetData.EvoN5);
    }

    public void SetEvolutionInfo(
        string PreName,
        string LaterName,
        int CurEvoUserPetID,
        string PreID,
        string LaterID,
        int PreLv,
        int PreMaxLv,
        int LaterLv,
        int LaterMaxLv,
        int PreCost,
        int LaterCost,
        int PreHP,
        int LaterHP,
        int PreATK,
        int LaterATK,
        //int PreSkillLv,
        //int LaterSkillLv,
        //int PreCskillChain,   
        //int LaterCskillChain,  
        int GoldNeed,
        string Material_1,     
        string Material_2,
        string Material_3,
        string Material_4,
        string Material_5)
    {
        m_1 = string.IsNullOrEmpty(Material_1) ? null : Material_1;
        m_2 = string.IsNullOrEmpty(Material_2) ? null : Material_2;
        m_3 = string.IsNullOrEmpty(Material_3) ? null : Material_3;
        m_4 = string.IsNullOrEmpty(Material_4) ? null : Material_4;
        m_5 = string.IsNullOrEmpty(Material_5) ? null : Material_5;


        needNum = 0;
        haveNum = 0;

        preName.text = PreName;
        laterName.text = LaterName;

        UIEventListener.Get(preButton.gameObject).onClick = (g) =>
        {
            UserPet u = UserManager.CurUserInfo.FindPetById(CurEvoUserPetID);
            preButton.GetComponent<ButtonMaterial>().ClickToDetail(u);
        };
        UIEventListener.Get(laterButton.gameObject).onClick = (g) =>
        {
            PetData p = ConfigManager.PetConfig.GetPetById(LaterID);
            laterButton.GetComponent<ButtonMaterial>().ClickToDetail(p);
        };
        

        GameObject prePrefab, laterPrefab, preAnime, laterAnime;
        string preSkinId = ConfigManager.PetConfig.GetPetById(PreID).SkinId;
        string laterSkinId = ConfigManager.PetConfig.GetPetById(LaterID).SkinId;

        prePrefab = Resources.Load("PreFabs/Characters/" + preSkinId + "60") as GameObject;
        if (prePrefab == null)
            prePrefab = Resources.Load("PreFabs/Characters/S160") as GameObject;

        preAnime = NGUITools.AddChild(preButton.gameObject, prePrefab);
        PetUIController.SetLayer(preAnime.transform, LayerHelper.Unit);
        preAnime.transform.localScale = new Vector3(60, 60, 1);
        preAnime.transform.localPosition = new Vector3(10, -40, 0);

        laterPrefab = Resources.Load("PreFabs/Characters/" + laterSkinId + "60") as GameObject;
        if (laterPrefab == null)
            laterPrefab = Resources.Load("PreFabs/Characters/S160") as GameObject;

        laterAnime = NGUITools.AddChild(laterButton.gameObject, laterPrefab);
        PetUIController.SetLayer(laterAnime.transform, LayerHelper.Unit);
        laterAnime.transform.localScale = new Vector3(60, 60, 1);
        laterAnime.transform.localPosition = new Vector3(10, -40, 0);

        Pre = preAnime;
        Later = laterAnime;

        preMaxLv.text =  "/" + PreMaxLv.ToString();
        laterLv.text = LaterLv.ToString();
        laterMaxLv.text = "/" + LaterMaxLv.ToString();
        //preCost.text = PreCost.ToString();
        //laterCost.text = LaterCost.ToString();
        preHp.text = PreHP.ToString();
        laterHp.text = LaterHP.ToString();
        preAtk.text = PreATK.ToString();
        laterAtk.text = LaterATK.ToString();

        evolutionButton.GetComponent<ButtonEvolution>().id = LaterID;

        //preSkillLv.text = "Lv." + PreSkillLv.ToString();
        //laterSkillLv.text = "Lv." + LaterSkillLv.ToString();

        //if (PreCskillChain <= 0) //进化前Cskill判断
        //{
        //    preCskill.gameObject.SetActive(false);
        //    preCskillChain.spriteName = null;
        //}
        //else
        //{
        //    preCskill.gameObject.SetActive(true);
        //    preCskillChain.spriteName = "";  //需要命名规则
        //}

        //if (LaterCskillChain <= 0)  //进化后Cskill判断
        //{
        //    laterCskill.gameObject.SetActive(false);
        //    laterCskillChain.spriteName = null;
        //}
        //else
        //{
        //    laterCskill.gameObject.SetActive(true);
        //    laterCskillChain.spriteName = "";  //需要命名规则
        //}

        goldNeed.text = GoldNeed.ToString();
        if (GoldNeed > UserManager.CurUserInfo.Gold)
        {
            CoinWarning.SetActive(true);
        }
        else
        {
            CoinWarning.SetActive(false);
        }


        materialButton_1.GetComponent<ButtonMaterial>().SetMaterial(Material_1, FindPet(Material_1));
        materialButton_2.GetComponent<ButtonMaterial>().SetMaterial(Material_2, FindPet(Material_2));
        materialButton_3.GetComponent<ButtonMaterial>().SetMaterial(Material_3, FindPet(Material_3));
        materialButton_4.GetComponent<ButtonMaterial>().SetMaterial(Material_4, FindPet(Material_4));

        if (!string.IsNullOrEmpty(Material_1)) needNum++;
        if (!string.IsNullOrEmpty(Material_2)) needNum++;
        if (!string.IsNullOrEmpty(Material_3)) needNum++;
        if (!string.IsNullOrEmpty(Material_4)) needNum++;

        if (FindPet(Material_1) > 0) haveNum++;
        if (FindPet(Material_2) > 0) haveNum++;
        if (FindPet(Material_3) > 0) haveNum++;
        if (FindPet(Material_4) > 0) haveNum++;

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

    /// <summary>
    /// 寻找是否有当前素材
    /// </summary>
    /// <param name="PetID"></param>
    /// <returns></returns>
    public int FindPet(string PetID)
    {
        int count = 0;
        if (ConfigManager.ItemConfig.GetItemById(PetID) != null)
        {
            foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
            {
                if (ui.CurItemData.Id == PetID)
                {
                    count++;
                }
            }
        }
        else
        {
            foreach (UserPet up in UserManager.CurUserInfo.UserPets)
            {
                if (up.CurPetData.Id == PetID && CurrentEvolutionUid != up.UserPetId)
                {
                    count++;
                }
            }
        }
        return count;
    }

    void OnEnable()
    {
        evolutionButton.GetComponent<ButtonEvolution>().evolutionBtnInter = this;
    }


    public void _OnClickEvolution(int UserPetID)
    {
        UserPet up = UserManager.CurUserInfo.FindPetById(UserPetID);
        JsonObject args = new JsonObject();
        args.Add("house_id", UserPetID);
        SocketCenter.Request(GameRouteConfig.EvoPet, args, (result) =>
        {
            if (result.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {

                    JsonObject data = (JsonObject)result.Data["monster"];
                    UserPet newPet = new UserPet(data);
                    UserManager.CurUserInfo.UserPets.Remove(up);
                    UserManager.CurUserInfo.UserPets.Add(newPet);

                    EvolutionAnimation.HiddenObjects.Add(Pre);
                    EvolutionAnimation.HiddenObjects.Add(Later);

                    EvolutionAnimation.gameObject.SetActive(true);
                    EvolutionAnimation.SetAnimation(
                        up.CurPetData.SkinId,
                        ConfigManager.PetConfig.GetPetById(m_1) == null ? (ConfigManager.ItemConfig.GetItemById(m_1) == null ? null : ConfigManager.ItemConfig.GetItemById(m_1).SkinId)  : ConfigManager.PetConfig.GetPetById(m_1).SkinId,
                        ConfigManager.PetConfig.GetPetById(m_2) == null ? (ConfigManager.ItemConfig.GetItemById(m_2) == null ? null : ConfigManager.ItemConfig.GetItemById(m_2).SkinId) : ConfigManager.PetConfig.GetPetById(m_2).SkinId,
                        ConfigManager.PetConfig.GetPetById(m_3) == null ? (ConfigManager.ItemConfig.GetItemById(m_3) == null ? null : ConfigManager.ItemConfig.GetItemById(m_3).SkinId) : ConfigManager.PetConfig.GetPetById(m_3).SkinId,
                        ConfigManager.PetConfig.GetPetById(m_4) == null ? (ConfigManager.ItemConfig.GetItemById(m_4) == null ? null : ConfigManager.ItemConfig.GetItemById(m_4).SkinId) : ConfigManager.PetConfig.GetPetById(m_4).SkinId,
                        ConfigManager.PetConfig.GetPetById(m_5) == null ? (ConfigManager.ItemConfig.GetItemById(m_5) == null ? null : ConfigManager.ItemConfig.GetItemById(m_5).SkinId) : ConfigManager.PetConfig.GetPetById(m_5).SkinId);
                    EvolutionAnimation.SetEvolutionInfo(up, newPet);
                    //.

                    JsonArray consum = (JsonArray)result.Data["consumes"];
                    UserManager.CurUserInfo.AddUserElements(consum);
                    JsonArray materials = (JsonArray)result.Data["pet_materials"];
                    for (int i = materials.Count - 1; i >= 0; i--)
                    {
                        int tempId = int.Parse(materials[i].ToString());
                        UserPet material = UserManager.CurUserInfo.FindPetById(tempId);
                        if(material != null)
                        {
                            UserManager.CurUserInfo.UserPets.Remove(material);
                        }
                        else
                        {
                            UserItem materialItem = UserManager.CurUserInfo.FindItemById(tempId);
                            UserManager.CurUserInfo.UserItems.Remove(materialItem);
                        }
                    }

                    UserInfo.SetInfo();

                    foreach(UserParty party in UserManager.CurUserInfo.UserPartys)
                    {
                        int totalCost = 0;
                        foreach(UserPet p in party.pets)
                        {
                            totalCost += up.CurPetData.PCost;
                        }
                        if(totalCost > UserManager.CurUserInfo.CurHero.Hcost)
                        {
                            party.petUids.Remove(newPet.UserPetId);
                        }
                    }

                    mViewControl.refreshFlag = true;
                });
            }
        }, null, true,true);
        FinishEvo();
    }

    public void FinishEvo()
    {
        evolutionBagControl.ClearBag();
        foreach (UserPet pet in UserManager.CurUserInfo.UserPets)
        {
            ItemInterface item = evolutionBagControl.CreateSetItem(pet.Level, pet.CurPetData.PCost, (int)pet.CurHp, (int)pet.CurAtk, pet.CurPetData.PetPro, pet.CurPetData.Id, pet.CurPetData.Rank, pet.UserPetId, false);
            if (pet.inParty)
            {
                item.InParty(true);
            }
            else
            {
                item.InParty(false);
            }
        }
        evolutionBagControl.SetNum(UserManager.CurUserInfo.UserPets.Count, UserManager.CurUserInfo.PetHouseLimit);
        evolutionBagControl.monsterBag.Reposition();
    }

    void OnDisable()
    {
        Destroy(Pre);
        Destroy(Later);
    }
}
