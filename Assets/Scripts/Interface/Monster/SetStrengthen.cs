using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class SetStrengthen : MonoBehaviour, BagInterface, _PowUpBase, _PowUpMaterial, _PowerUp
{
    /// <summary>
    /// 进化动画
    /// </summary>
    public PUAnimationControl Animation;


    public SetUser UserInfo;

    public UIButton PowUpButton;

    public UILabel sortType;
    public UILabel currentLv;
    public UILabel expGet;
    public UILabel lvUpLeft;
    public AlphaMaskBar lvBar;
    public UISprite downArrow;
    public UILabel tarLv;
    public UILabel tarLvNum;
    public UILabel currentHP;
    public UILabel hpBuff;
    public UILabel currentATK;
    public UILabel atkBuff;
    public UILabel goldNeed;
    public BagControl TableControl;
    public ButtonPowUpBase pu;
    public ButtonPowUpMaterial pm;
    public ButtonPowerUp bpu;
    public GameObject CurPW;

    public GameObject SortButton;

    public GameObject CoinWarning;


    void Awake()
    {
        TableControl.bagInter = this;
        pm.PowUpMaterialInter = this;
        pu.PowUpBaseIner = this;
        bpu.PowerUpInter = this;
    }


    void OnEnable()
    {
        SelectType = 0;
        BagControl.NotInParty = true;
        CurPW.SetActive(true);
        showEleSelect(false);
        TableControl.SetNum(UserManager.CurUserInfo.UserPets.Count, UserManager.CurUserInfo.PetHouseLimit);
        goldNeed.text = "0";
        foreach (UserPet pet in UserManager.CurUserInfo.UserPets)
        {
            ItemInterface item = TableControl.CreateSetItem(pet.Level, pet.CurPetData.PCost, (int)pet.CurHp, (int)pet.CurAtk, pet.CurPetData.PetPro, pet.CurPetData.Id, pet.CurPetData.Rank, pet.UserPetId, false);
            if(pet.Level == pet.CurPetData.MaxLevel)
            {
                item.IsCover(true);
            }
            else
            {
                item.IsCover(false);
            }
            if (pet.inParty)
            {
                item.InParty(true);
            }
            else
            {
                item.InParty(false);
            }
        }

        UpPetAvata.mainTexture = null;
        UpPetFrame.mainTexture = baseFrame;
        CurUpPet = null;
        RefreshBaseInfo(false);
        RefreshUpInfo();

        foreach (MaterialControl c in MaterialControls)
        {
            c.RemovePet();
        }
        ElementsList.Clear();

        UIEventListener.Get(SortButton).onClick = (g) =>
        {
            TableControl.SwitchSorting();
            foreach(GameObject go in TableControl.items)
            {
                ItemInterface ii = go.GetComponent<ItemInterface>();
                UserPet u = UserManager.CurUserInfo.FindPetById(ii.userMonsterID);
                if(SelectType == 0)
                {
                    if (u.Level == u.CurPetData.MaxLevel)
                    {
                        ii.IsCover(true);
                    }
                    else
                    {
                        ii.IsCover(false);
                    }
                }
                foreach(UserPet up in ElementsList)
                {
                    if(ii.userMonsterID == up.UserPetId)
                    {
                        ii.IsMaterial(true);
                        ii.SetSelect(true);
                    }
                }
                if(CurUpPet != null && (ii.userMonsterID == CurUpPet.UserPetId))
                {
                    ii.IsBase(true);
                    ii.SetSelect(true);
                }
            }
        };
    }

    void OnDisable()
    {
        CoinWarning.SetActive(false);

        TableControl.ClearBag();
    }

    public void _OnClickBase()
    {
        //BagControl.NotInParty = true;

        CurPW.SetActive(true);
        showEleSelect(false);
        SelectType = 0;
        foreach(UserPet pet in UserManager.CurUserInfo.UserPets)
        {
            ItemInterface item = TableControl.FindItem(pet.UserPetId);
            if (pet.Level == pet.CurPetData.MaxLevel)
            {
                item.IsCover(true);
            }
            else
            {
                item.IsCover(false);
            }
        }
    }

    public void _OnClickMaterial()
    {
        //BagControl.NotInParty = false;

        CurPW.SetActive(false);
        showEleSelect(true);
        SelectType = 1;
        foreach(GameObject g in TableControl.items)
        {
            g.GetComponent<ItemInterface>().IsCover(false);
        }
    }

    void showEleSelect(bool show)
    {
        foreach (MaterialControl mc in MaterialControls)
        {
            mc.ShowWhite(show);
        }
    }

    public Texture baseFrame;
//创建背包内物体脚本为Script/interface/BagControl.cs 

    bool IsOnElement(UserPet pet)
    {
        foreach (MaterialControl mc in MaterialControls)
        {
            if (mc.CurMaterialPet == pet)
            {
                return true;
            }
        }
        return false;
    }


    public void _OnClickItemInter(int UserMonsterID)
    {
        ItemInterface item = TableControl.FindItem(UserMonsterID);
        UserPet ElementPet = UserManager.CurUserInfo.FindPetById(UserMonsterID);
        if (SelectType == 0)
        {
            if (ElementPet.Level >= ElementPet.CurPetData.MaxLevel) return;
            if (ElementPet != CurUpPet && !IsOnElement(ElementPet))
            {
                if (CurUpPet != null)
                {
                    ItemInterface preItem = TableControl.FindItem(CurUpPet.UserPetId);
                    preItem.IsBase(false);
                    preItem.SetSelect(false);
                }
                SelectType = 1;
                SetNeedUpMonster(UserMonsterID);
                RefreshUpInfo();
                CurPW.SetActive(true);
                item.SetSelect(true);
                item.IsBase(true);
            }
            else if(ElementPet == CurUpPet)
            {
                Debug.Log(ElementPet.CurPetData.Id);
                UpPetAvata.mainTexture = null;
                UpPetFrame.mainTexture = baseFrame;
                CurUpPet = null;
                RefreshBaseInfo(false);
                item.SetSelect(false);
                item.IsBase(false);
            }
        }
        else
        {
            if (ElementPet.inParty) return;
            if (!IsOnElement(ElementPet) && ElementPet!=CurUpPet)
            {
                if (ElementsList.Count < 5)
                {
                    MaterialControl mc = MaterialControls[ElementsList.Count];
                    mc.SetPet(ElementPet);
                    ElementsList.Add(ElementPet);
                    RefreshUpInfo();
                    item.SetSelect(true);
                    item.IsMaterial(true);
                }
            }
            else
            {
                MaterialControl mc = FindMaterial(ElementPet);
                if (mc)
                {
                    ElementsList.Remove(ElementPet);
                    item.SetSelect(false);
                    if (mc.CurMaterialPet.Level == mc.CurMaterialPet.CurPetData.MaxLevel) item.IsCover(true);
                    item.IsMaterial(false);
                    mc.RemovePet();
                }
                //重排
                foreach (MaterialControl c in MaterialControls)
                {
                    c.RemovePet();
                }

                int i = 0;
                foreach (UserPet up in ElementsList)
                {
                    MaterialControl c = MaterialControls[i];
                    c.SetPet(up);
                    i++;
                }
                RefreshUpInfo();
            }
        }
    }

    MaterialControl FindMaterial(UserPet ElementPet)
    {
        foreach (MaterialControl mc in MaterialControls)
        {
            if (mc.CurMaterialPet == ElementPet)
            {
                return mc;
            }
        }
        return null;
    }

    public void _OnLongPressItemInter(int UserMonsterID)
    {

    }

    public UITexture UpPetAvata;
    public UITexture UpPetFrame;
    public int SelectType; //0 = base / 1 = material
    public UserPet CurUpPet;
    public List<UserPet> ElementsList = new List<UserPet>();
    public List<MaterialControl> MaterialControls = new List<MaterialControl>();
    public void _OnClickRemoveInter(int UserMonsterID)
    { 
        
    }

    void SetNeedUpMonster(int userMonsterId)
    {
        CurUpPet = UserManager.CurUserInfo.FindPetById(userMonsterId);
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(CurUpPet.CurPetData.SkinId);
        string frame = Tools.GetRankFrame(CurUpPet.CurPetData.Rank);
        Texture ft = Resources.Load<Texture>("UI/UI_Assets/others/" + frame);
        Texture at = Resources.Load<Texture>("Atlas/PetAvatars/" + skinData.IconId);
        UpPetFrame.mainTexture = ft;
        UpPetAvata.mainTexture = at;
        CurPW.SetActive(false);
        showEleSelect(true);
    }


    public void _OnClickPowerUp()
    {
        if (CurUpPet != null)
        {
            if (ElementsList.Count == 0) return;
            //if (int.Parse(goldNeed.text) <= UserManager.CurUserInfo.Gold)
            if(true)
            {
                JsonArray ids = new JsonArray();
                foreach (UserPet p in ElementsList)
                {
                    ids.Add(p.UserPetId);
                }
                JsonObject args = new JsonObject();
                args.Add("house_id", CurUpPet.UserPetId);
                args.Add("pet_ids", ids);
                SocketCenter.Request(GameRouteConfig.UpgradePet, args, (r) =>
                {
                    if (r.Code == SocketResult.ResultCode.Success)
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            int eNum = ElementsList.Count;
                            JsonObject data = (JsonObject)r.Data["monster"];
                            JsonArray consum = (JsonArray)r.Data["consumes"];

                            //动画:
                            Animation.gameObject.SetActive(true);
                            string[] materialPetIds = new string[5];
                            for (int i = 0; i < ElementsList.Count; i++)
                            {
                                materialPetIds[i] = ElementsList[i].CurPetData.SkinId;
                            }
                            Animation.SetAnimation(CurUpPet.CurPetData.SkinId, materialPetIds[0], materialPetIds[1], materialPetIds[2], materialPetIds[3], materialPetIds[4]);
                            
                            Animation.Pbase = new UserPet(data);
                            Animation.Pbase.CurAtk = CurUpPet.CurAtk;
                            Animation.Pbase.CurHp = CurUpPet.CurHp;
                            Animation.Pbase.CurPetData = CurUpPet.CurPetData;
                            Animation.Pbase.Exp = CurUpPet.Exp;
                            Animation.Pbase.Level = CurUpPet.Level;
                            Animation.Pbase.UserPetId = CurUpPet.UserPetId;
                            //.

                            foreach (UserPet u in ElementsList)
                            {
                                UserManager.CurUserInfo.UserPets.Remove(u);
                            }
                            ElementsList.Clear();                    
                            UserManager.CurUserInfo.AddUserElements(consum);
                            CurUpPet.Exp = int.Parse(data["exp"].ToString());

                            //动画:
                            Animation.PLvUp = CurUpPet;
                            //.

                            foreach (MaterialControl c in MaterialControls)
                            {
                                c.RemovePet();
                            }

                            TableControl.SetNum(UserManager.CurUserInfo.UserPets.Count, UserManager.CurUserInfo.PetHouseLimit);
                            TableControl.ClearBag();
                            foreach (UserPet pet in UserManager.CurUserInfo.UserPets)
                            {
                                ItemInterface tem = TableControl.CreateSetItem(pet.Level, pet.CurPetData.PCost, (int)pet.CurHp, (int)pet.CurAtk, pet.CurPetData.PetPro, pet.CurPetData.Id, pet.CurPetData.Rank, pet.UserPetId, false);
                                if (pet.Level == pet.CurPetData.MaxLevel)
                                {
                                    tem.IsCover(true);
                                }
                                else
                                {
                                    tem.IsCover(false);
                                }

                                if (pet.inParty)
                                {
                                    tem.InParty(true);
                                }
                                else
                                {
                                    tem.InParty(false);
                                }
                            }
                            ItemInterface item = TableControl.FindItem(CurUpPet.UserPetId);
                            item.SetSelect(true);
                            item.IsBase(true);
                            RefreshUpInfo();
                            UserInfo.gold.text = UserManager.CurUserInfo.Gold.ToString();
                            UserInfo.diamonds.text = UserManager.CurUserInfo.Diamond.ToString();

                            mViewControl.refreshFlag = true;
                        });
                    }
                }, null, true,true);
            }
        }
    }

    public void RefreshUpInfo()
    {
        if (CurUpPet!=null)
        {
            downArrow.gameObject.SetActive(true);
            expGet.gameObject.SetActive(true);
            atkBuff.gameObject.SetActive(true);
            hpBuff.gameObject.SetActive(true);
            tarLv.gameObject.SetActive(true);
            tarLvNum.gameObject.SetActive(true);

            int getTotalExp = 0;
            foreach (UserPet up in ElementsList)
            {
                int additionExp = (int)(up.CurPetData.Exp + up.Exp * ConfigManager.ParamConfig.GetParam().PetEXPGrowthRate);
                getTotalExp = getTotalExp + additionExp;
            }
            if(getTotalExp > (ConfigManager.PetLevelConfig.GetExpByLevel(CurUpPet.CurPetData.MaxLevel) - CurUpPet.Exp))
            {
                getTotalExp = ConfigManager.PetLevelConfig.GetExpByLevel(CurUpPet.CurPetData.MaxLevel) - CurUpPet.Exp;
            }
            goldNeed.text = ((int)getTotalExp*ConfigManager.ParamConfig.GetParam().PetLvUpCoinGrowthRate).ToString();

            if ((int)getTotalExp * ConfigManager.ParamConfig.GetParam().PetLvUpCoinGrowthRate > UserManager.CurUserInfo.Gold)
            {
                CoinWarning.SetActive(true);
            }
            else
            {
                CoinWarning.SetActive(false);
            }

            expGet.text = getTotalExp.ToString();
            int lastExp = getTotalExp + CurUpPet.Exp;
            int toLevel = ConfigManager.PetLevelConfig.GetCurLevel(lastExp) > CurUpPet.CurPetData.MaxLevel ? CurUpPet.CurPetData.MaxLevel : ConfigManager.PetLevelConfig.GetCurLevel(lastExp);
            int additionHp = (toLevel - CurUpPet.Level) * CurUpPet.CurPetData.HpUp;
            int additionAtk = (toLevel - CurUpPet.Level) * CurUpPet.CurPetData.AtkUp;
            atkBuff.text = "+" + additionAtk.ToString();
            hpBuff.text = "+" + additionHp.ToString();

            if (toLevel >= CurUpPet.CurPetData.MaxLevel)
            {
                tarLvNum.text = "MAX";
                lvUpLeft.text = "0";
                lvBar.value = 1;
            }
            else
            {
                tarLvNum.text = toLevel.ToString();
                int nextLevel = CurUpPet.Level + 1;
                if (toLevel >= nextLevel)
                {
                    nextLevel = toLevel + 1;
                }
                lvUpLeft.text = (ConfigManager.PetLevelConfig.GetExpByLevel(nextLevel) - lastExp).ToString();

                float value = (float)(getTotalExp - ConfigManager.PetLevelConfig.GetExpByLevel(nextLevel - 1) + CurUpPet.CurrentExp) / (float)(ConfigManager.PetLevelConfig.GetExpByLevel(nextLevel) - ConfigManager.PetLevelConfig.GetExpByLevel(nextLevel - 1));
                lvBar.value = value;


            }

            RefreshBaseInfo(true);
        }
        

        if (ElementsList.Count == 0||CurUpPet == null)
        {
            downArrow.gameObject.SetActive(false);
            expGet.gameObject.SetActive(false);
            atkBuff.gameObject.SetActive(false);
            hpBuff.gameObject.SetActive(false);
            tarLv.gameObject.SetActive(false);
            tarLvNum.gameObject.SetActive(false);

            RefreshBaseInfo(true);
        }
    }

    void SetLvLbl(UILabel LvLabel, int Lv)
    {
        if (Lv >= CurUpPet.CurPetData.MaxLevel)
        {
            LvLabel.text = "Lv.[ff0000]MAX";
        }
        else
        {
            LvLabel.color = Color.white;
            LvLabel.text = "Lv." + CurUpPet.Level;
        }
    }

    public void RefreshBaseInfo(bool show, int expGet = 0)
    {
        if(CurUpPet != null)
        {
            currentLv.gameObject.SetActive(true);
            currentHP.gameObject.SetActive(true);
            currentATK.gameObject.SetActive(true);
        }
        else
        {
            currentLv.gameObject.SetActive(false);
            currentHP.gameObject.SetActive(false);
            currentATK.gameObject.SetActive(false);
        }
        lvUpLeft.gameObject.SetActive(show);
        if (show)
        {
            if (CurUpPet != null)
            {
                SetLvLbl(currentLv, CurUpPet.Level);
                currentHP.text = CurUpPet.CurHp.ToString();
                currentATK.text = CurUpPet.CurAtk.ToString();
                //lvBar.value = (float)CurUpPet.CurrentExp / (float)CurUpPet.CurLvlExp;
            }
        }
        else
        {
            lvBar.value = 0;
        }
    }
}

