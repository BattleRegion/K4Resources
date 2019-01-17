using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class SetPowUp : MonoBehaviour, EquipmentBagInterface, materialBagInter, _PowUpBase, _PowUpMaterial, _PowerUp
{
    public enum selectType
    {
        baseType = 0,
        materialType = 1
    }

    selectType type;

    #region NGUI
    public UITexture baseIcon;
    public UISprite baseFrame;
    public UISprite baseElement;
    public UISprite baseWhiteFrame;
    public SetStars baseStars;
    
    public UIButton powUpButton;
    public UIButton materialArea;
    public UIButton baseArea;

    public UILabel currentLv;
    public UILabel expGet;
    public UILabel lvUpLeft;
    public AlphaMaskBar lvBar;

    public UISprite downArrow;
    public UILabel tarLv;

    int target_level;

    public UILabel atkOrDef;
    public UILabel currentATK;
    public UILabel atkBuff;
    public UILabel goldNeed;
    #endregion

    public GameObject CoinWarning;

    public GameObject levelUpBoard;
    public List<GameObject> evoAnimes = new List<GameObject>();

    public SetUser UserInfo;

    public EquipmentBagControl weaponBag;
    public EquipmentBagControl armorBag;
    public EquipmentBagControl equipmentBag;
    public MaterialBagControl materialBag;
    public SetBagNum weaponBagNum;
    public SetBagNum armorBagNum;
    public SetBagNum equipmentBagNum;
    public SetMbagNum materialBagNum;

    public GameObject PowUpState1;  //武器和护甲
    public GameObject PowUpState2;  //装备和素材

    public GameObject IconFlash;

    public List<PowerUpMaterialcontrol> materials = new List<PowerUpMaterialcontrol>();

    int curPrice = 0;
    int getTotalExp = 0;
    int materialNum = 0;
    int curBaseId = -1;
    List<int> materialEs = new List<int>();
    List<int> materialMs = new List<int>();

    /// <summary>
    /// 动画
    /// </summary>
    public HPUAnimationControl PowUpAnimation;

    public GameObject SortButton;

    /// <summary>
    /// 初始化
    /// </summary>
    void OnEnable()
    {
        type = selectType.baseType;
        foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
        {
            equipmentBag.AddEquipmentItem(uw.Level, uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, uw.CurHardWareData.Rank, uw.UserWareId);
            equipmentItemInterface e = equipmentBag.GetItemByUId(uw.UserWareId);
            if (uw.IsWare)
            {
                e.IsEquip(true);
            }
            if ((int)uw.CurHardWareData.Style < 5)
            {
                weaponBag.AddEquipmentItem(uw.Level, uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, uw.CurHardWareData.Rank, uw.UserWareId);
                equipmentItemInterface ei = weaponBag.GetItemByUId(uw.UserWareId);
                if (uw.IsWare)
                {
                    ei.IsEquip(true);
                }
            }
            else
            {
                armorBag.AddEquipmentItem(uw.Level, uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, uw.CurHardWareData.Rank, uw.UserWareId);
                equipmentItemInterface ei = armorBag.GetItemByUId(uw.UserWareId);
                if (uw.IsWare)
                {
                    ei.IsEquip(true);
                }
            }
        }
        materialBag.SortMaterial();

        weaponBag.bagInter = this;
        armorBag.bagInter = this;
        materialBag.inter = this;
        equipmentBag.bagInter = this;
        powUpButton.GetComponent<ButtonPowerUp>().PowerUpInter = this;
        materialArea.GetComponent<ButtonPowUpMaterial>().PowUpMaterialInter = this;
        baseArea.GetComponent<ButtonPowUpBase>().PowUpBaseIner = this;
        ClearPowUpBoard();
        SwitchState(1);


        UIEventListener.Get(SortButton).onClick = (g) =>
        {
            switch(type)
            {
                case selectType.baseType:
                    {
                        if(weaponBag.gameObject.activeSelf)
                        {
                            weaponBag.SwitchSorting();
                            foreach (GameObject go in weaponBag.equipmentItems)
                            {
                                equipmentItemInterface ei = go.GetComponent<equipmentItemInterface>();
                                foreach (int Uid in materialEs)
                                {
                                    if (Uid == ei.userEquipmentID)
                                    {
                                        ei.IsMaterial(true);
                                        ei.IsCover(true);
                                        ei.SetSelect(true);
                                        break;
                                    }
                                }
                                if (ei.userEquipmentID == curBaseId)
                                {
                                    ei.IsBase(true);
                                    ei.IsCover(true);
                                    ei.SetSelect(true);
                                }
                            }
                        }
                        else if(armorBag.gameObject.activeSelf)
                        {
                            armorBag.SwitchSorting();
                            foreach (GameObject go in armorBag.equipmentItems)
                            {
                                equipmentItemInterface ei = go.GetComponent<equipmentItemInterface>();
                                foreach (int Uid in materialEs)
                                {
                                    if (Uid == ei.userEquipmentID)
                                    {
                                        ei.IsMaterial(true);
                                        ei.IsCover(true);
                                        ei.SetSelect(true);
                                        break;
                                    }
                                }
                                if (ei.userEquipmentID == curBaseId)
                                {
                                    ei.IsBase(true);
                                    ei.IsCover(true);
                                    ei.SetSelect(true);
                                }
                            }
                        }
                        break;
                    }
                case selectType.materialType:
                    {
                        if(equipmentBag.gameObject.activeSelf)
                        {
                            equipmentBag.SwitchSorting();
                            foreach(GameObject go in equipmentBag.equipmentItems)
                            {
                                equipmentItemInterface ei = go.GetComponent<equipmentItemInterface>();
                                foreach (int Uid in materialEs)
                                {
                                    if(Uid == ei.userEquipmentID)
                                    {
                                        ei.IsMaterial(true);
                                        ei.IsCover(true);
                                        ei.SetSelect(true);
                                        break;
                                    }
                                }
                                if(ei.userEquipmentID == curBaseId)
                                {
                                    ei.IsBase(true);
                                    ei.IsCover(true);
                                    ei.SetSelect(true);
                                }
                            }
                        }
                        break;
                    }
            }
        };
    }

    /// <summary>
    /// 清空base板
    /// </summary>
    void ClearPowUpBoard()
    {
        baseIcon.mainTexture = null;
        baseFrame.gameObject.SetActive(false);
        baseElement.spriteName = "";
        baseStars.SetStar(0);
        baseWhiteFrame.gameObject.SetActive(true);
        type = selectType.baseType;
        currentLv.text = "";
        expGet.text = "";
        lvUpLeft.text = "";
        lvBar.value = 0;
        levelUpBoard.SetActive(false);
        curPrice = 0;
        SetPrice(curPrice);
        foreach (PowerUpMaterialcontrol p in materials)
        {
            p.RemoveMaterial();
            p.ShowWhite(false);
        }
        materialEs.Clear();
        materialMs.Clear();
    }

    /// <summary>
    /// 清空base板（延时清空素材）
    /// </summary>
    void DelayClearPowUpBoard()
    {
        baseIcon.mainTexture = null;
        baseFrame.gameObject.SetActive(false);
        baseElement.spriteName = "";
        baseStars.SetStar(0);
        baseWhiteFrame.gameObject.SetActive(true);
        type = selectType.baseType;
        currentLv.text = "";
        expGet.text = "";
        lvUpLeft.text = "";
        lvBar.value = 0;
        levelUpBoard.SetActive(false);
        curPrice = 0;
        SetPrice(curPrice);
        for (int i = 0; i < materialNum; i++)
        {
            StartCoroutine(DelayRemoveMaterial(materials[i], i));
        }
        materialEs.Clear();
        materialMs.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    IEnumerator DelayRemoveMaterial(PowerUpMaterialcontrol p, int index)
    {
        yield return new WaitForSeconds(0.2f * index);
        p.RemoveMaterial();
    }

    /// <summary>
    /// 设置价格和按钮状态
    /// </summary>
    void SetPrice(int price)
    {
        goldNeed.text = price.ToString();
        if (curPrice > UserManager.CurUserInfo.Gold) SetButtonState(false);
        else SetButtonState(true);
    }

    /// <summary>
    /// 设置升级数据显示
    /// </summary>
    void SetLvUpBoard(int Uid, int ExpGet)
    {
        UserWare uw = UserManager.CurUserInfo.FindUserWare(Uid);
        int expLeft = ExpGet;
        int lvAddition = 0;
        int levelUpLeft = 0;
        int levelUpNeed = 0;
        int buff = 0;
        if (expLeft >= (uw.LevelExp - uw.CurExp))
        {
            if (uw.Level >= uw.CurHardWareData.LvlMax) //满级不变
            {
                target_level = uw.CurHardWareData.LvlMax;
                tarLv.text = "MAX";
                if ((int)uw.CurHardWareData.Style < 5)
                {
                    atkOrDef.text = "攻击：";
                    currentATK.text = uw.CurAtk.ToString();
                    buff = lvAddition * uw.CurHardWareData.AttackUp;
                }
                else
                {
                    atkOrDef.text = "防御：";
                    currentATK.text = uw.CurDef.ToString();
                    buff = lvAddition * uw.CurHardWareData.DefendUp;
                }
                levelUpBoard.SetActive(true);
                expGet.text = ExpGet.ToString();
                lvUpLeft.text = "0";
                lvBar.value = 1;
                atkBuff.text = "+ " + buff.ToString();
                return;
            }

            int lvUpNeedExp = uw.LevelExp - uw.CurExp;
            lvAddition++;
            expLeft -= lvUpNeedExp;
            while (expLeft >= ConfigManager.HardwareLevelConfig.GetHardwareLvUpNeedByLv(uw.Level + lvAddition))
            {
                expLeft -= ConfigManager.HardwareLevelConfig.GetHardwareLvUpNeedByLv(uw.Level + lvAddition);
                lvAddition++;
                if (uw.Level + lvAddition >= uw.CurHardWareData.LvlMax)
                {
                    target_level = uw.CurHardWareData.LvlMax;
                    tarLv.text = "MAX";
                    if ((int)uw.CurHardWareData.Style < 5)
                    {
                        atkOrDef.text = "攻击：";
                        currentATK.text = uw.CurAtk.ToString();
                        buff = lvAddition * uw.CurHardWareData.AttackUp;
                    }
                    else
                    {
                        atkOrDef.text = "防御：";
                        currentATK.text = uw.CurDef.ToString();
                        buff = lvAddition * uw.CurHardWareData.DefendUp;
                    }
                    levelUpBoard.SetActive(true);
                    expGet.text = ExpGet.ToString();
                    lvUpLeft.text = "0";
                    lvBar.value = 1;
                    atkBuff.text = "+ " + buff.ToString();
                    return;
                }
            }
            target_level = uw.Level + lvAddition;
            if (target_level == uw.CurHardWareData.LvlMax) tarLv.text = "MAX";
            else tarLv.text = (uw.Level + lvAddition).ToString();
            if ((int)uw.CurHardWareData.Style < 5)
            {
                atkOrDef.text = "攻击：";
                currentATK.text = uw.CurAtk.ToString();
                buff = lvAddition * uw.CurHardWareData.AttackUp;
            }
            else
            {
                atkOrDef.text = "防御：";
                currentATK.text = uw.CurDef.ToString();
                buff = lvAddition * uw.CurHardWareData.DefendUp;
            }
            levelUpBoard.SetActive(true);
        }
        else //不升级
        {
            expGet.text = ExpGet.ToString();
            levelUpNeed = uw.LevelExp - uw.CurExp;
            levelUpLeft = levelUpNeed - expLeft;
            lvUpLeft.text = levelUpLeft.ToString();
            atkBuff.text = "+ " + buff.ToString();
            lvBar.value = (float)(uw.CurExp + expLeft) / (float)uw.LevelExp;
            return;
        }
        expGet.text = ExpGet.ToString();
        levelUpNeed = ConfigManager.HardwareLevelConfig.GetHardwareLvUpNeedByLv(uw.Level + lvAddition);
        levelUpLeft = levelUpNeed - expLeft;
        lvUpLeft.text = levelUpLeft.ToString();
        atkBuff.text = "+ " + buff.ToString();
        if (target_level == uw.CurHardWareData.LvlMax) lvBar.value = 1;
        else lvBar.value = (float)expLeft / (float)levelUpNeed;
    }

    /// <summary>
    /// 清空升级数据显示
    /// </summary>
    void ClearLvUpBoard()
    {
        expGet.text = "";
        lvUpLeft.text = "";
        if(curBaseId != -1)
        {
            UserWare u = UserManager.CurUserInfo.FindUserWare(curBaseId);
            lvBar.value = (float)u.CurExp / (float)u.LevelExp;
            target_level = u.Level;
        }
        else
        {
            target_level = -1;
            lvBar.value = 0f;
        }
        levelUpBoard.SetActive(false);
    }

    /// <summary>
    /// 切换显示的背包
    /// </summary>
    void SwitchState(int i)
    {
        switch (i)
        {
            case 1:
                {
                    type = selectType.baseType;
                    PowUpState1.SetActive(true);
                    PowUpState2.SetActive(false);
                    baseWhiteFrame.gameObject.SetActive(true);
                    foreach (PowerUpMaterialcontrol p in materials)
                    {
                        p.ShowWhite(false);
                    }
                    break;
                }
            case 2:
                {
                    type = selectType.materialType;
                    PowUpState1.SetActive(false);
                    PowUpState2.SetActive(true);
                    baseWhiteFrame.gameObject.SetActive(false);
                    foreach (PowerUpMaterialcontrol p in materials)
                    {
                        p.ShowWhite(true);
                    }
                    break;
                }
            default: break;
        }
    }

    /// <summary>
    /// 设置base信息
    /// </summary>
    void SetBase(int Uid)
    {
        curBaseId = Uid;
        UserWare ui = UserManager.CurUserInfo.FindUserWare(Uid);
        baseFrame.gameObject.SetActive(true);
        SkinConfigData skin = ConfigManager.SkinConfig.GetSkinDataById(ui.CurHardWareData.SkinId);
        Texture t = Resources.Load<Texture>("Atlas/ItemIcons/" + skin.IconId);
        baseIcon.mainTexture = t;
        baseElement.spriteName = Tools.GetHardwareElement(ui.CurHardWareData.Element);
        if (ui.Level >= ui.CurHardWareData.LvlMax) currentLv.text = "Lv.MAX";
        else currentLv.text = "Lv." + ui.Level.ToString();
        lvBar.value = (float)ui.CurExp / (float)ui.LevelExp;
        baseStars.SetStar(ui.CurHardWareData.Rank);
        SetLvUpBoard(curBaseId, getTotalExp);
    }

    /// <summary>
    /// 设置material
    /// </summary>
    void SetMaterial(PowerUpMaterialcontrol.materialType Type, PowerUpMaterialcontrol Material, int Uid)
    {
        Material.SetMaterial(Type, Uid);
    }

    /// <summary>
    /// 去掉material
    /// </summary>
    void RemoveMaterial(PowerUpMaterialcontrol Material)
    {
        Material.RemoveMaterial();
    }


    /// <summary>
    /// 得到装备提供的经验
    /// </summary>
    int GetHardwareExp(int Uid)
    {
        int exp = 0;
        UserWare uw = UserManager.CurUserInfo.FindUserWare(Uid);
        exp = uw.CurHardWareData.Exp + (int)(uw.Exp * ConfigManager.ParamConfig.GetParam().HardwareEXPGrowthRate);
        return exp;
    }

    /// <summary>
    /// 得到消耗该装备需要的金币
    /// </summary>
    int GetHardwareGold(int Uid)
    {
        int gold = 0;
        gold = (int)(GetHardwareExp(Uid) * ConfigManager.ParamConfig.GetParam().HardwareLvUpCoinGrowthRate);
        return gold;
    }

    /// <summary>
    /// 得到材料提供的经验
    /// </summary>
    int GetItemExp(int Uid)
    {
        int exp = 0;
        UserItem ui = UserManager.CurUserInfo.FindItemById(Uid);
        exp = ui.CurItemData.HardwareEXP;
        return exp;
    }

    /// <summary>
    /// 得到消耗该材料需要的金币
    /// </summary>
    int GetItemGold(int Uid)
    {
        int gold = 0;
        gold = (int)(GetItemExp(Uid) * ConfigManager.ParamConfig.GetParam().HardwareLvUpCoinGrowthRate);
        return gold;
    }

    /// <summary>
    /// 添加一个material
    /// </summary>
    void AddMaterial(int Uid, PowerUpMaterialcontrol.materialType Mtype)
    {
        if(materialNum >= 5) return;
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].up == false)
            {
                materials[i].SetMaterial(Mtype, Uid);
                materialNum++;
                break;
            }
        }
    }

    /// <summary>
    /// 重置material位置
    /// </summary>
    void RepositionMaterial(ref List<PowerUpMaterialcontrol> Mlist)
    {
        for (int i = 0; i < Mlist.Count; i++ )
        {
            if (Mlist[i].up == false)
            {
                if (i == Mlist.Count - 1 || Mlist[i + 1].up == false) return;
                Mlist[i].SetMaterial(Mlist[i + 1].mType, Mlist[i + 1].uID);
                Mlist[i + 1].RemoveMaterial();
            }
        }
    }



    /// <summary>
    /// 设置是否为本体
    /// </summary>
    void SetEB(int Uid, bool Value)
    {
        foreach (GameObject g in equipmentBag.equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            if (ei.userEquipmentID == Uid)
            {
                ei.SetSelect(Value);
                ei.IsBase(Value);
            }
        }
        foreach (GameObject g in weaponBag.equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            if (ei.userEquipmentID == Uid)
            {
                ei.SetSelect(Value);
                ei.IsBase(Value);
            }
        }
        foreach (GameObject g in armorBag.equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            if (ei.userEquipmentID == Uid)
            {
                ei.SetSelect(Value);
                ei.IsBase(Value);
            }
        }
    }

    /// <summary>
    /// 设置所有背包中的equipmentItem是否为素材
    /// </summary>
    void SetEM(int Uid, bool Value)
    {
        foreach (GameObject g in equipmentBag.equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            if (ei.userEquipmentID == Uid)
            {
                ei.SetSelect(Value);
                ei.IsMaterial(Value);
            }
        }
        foreach (GameObject g in weaponBag.equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            if (ei.userEquipmentID == Uid)
            {
                ei.SetSelect(Value);
                ei.IsMaterial(Value);
            }
        }
        foreach (GameObject g in armorBag.equipmentItems)
        {
            equipmentItemInterface ei = g.GetComponent<equipmentItemInterface>();
            if (ei.userEquipmentID == Uid)
            {
                ei.SetSelect(Value);
                ei.IsMaterial(Value);
            }
        }
    }

    /// <summary>
    /// 设置所有背包中的item是否为素材
    /// </summary>
    void SetMM(int Uid, bool Value)
    {
        foreach (GameObject g in materialBag.materialItems)
        {
            MaterialItemInterface mi = g.GetComponent<MaterialItemInterface>();
            if (mi.userMaterialId == Uid)
            {
                mi.SetSelect(Value);
                mi.IsMaterial(Value);
            }
        }
    }

    /// <summary>
    /// 刷新所有背包
    /// </summary>
    void RefreshBags()
    {

    }

    /// <summary>
    /// 设置强化按钮的禁用状态
    /// </summary>
    void SetButtonState(bool State)
    {
        if (State) powUpButton.GetComponent<UIWidget>().color = Color.white;
        else powUpButton.GetComponent<UIWidget>().color = Color.gray;
        powUpButton.GetComponent<BoxCollider>().enabled = State;
    }

    /// <summary>
    /// 单个强化动画
    /// </summary>
    IEnumerator DelayAnimeUnit(GameObject g, int index)
    {
        yield return new WaitForSeconds(0.2f * index);
        Instantiate(g, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        StartCoroutine(DelayFlash());
    }

    IEnumerator DelayFlash()
    {
        yield return new WaitForSeconds(0.2f);
        Instantiate(IconFlash, new Vector3(-0.41f, 0.42f, 0), new Quaternion(0, 0, 0, 0));
    }

    /// <summary>
    /// 强化动画
    /// </summary>
    /// <param name="MaterialNum"></param>
    void PowUpAnime(int MaterialNum)
    {
        for (int i = 0; i < MaterialNum; i++)
        {
            StartCoroutine(DelayAnimeUnit(evoAnimes[i], i));
        }
    }

    /// <summary>
    /// disable时清空状态
    /// </summary>
    void OnDisable()
    {
        getTotalExp = 0;
        curPrice = 0;
        materialNum = 0;
        weaponBag.ClearBag();
        equipmentBag.ClearBag();
        materialBag.ClearMaterialBag();
        armorBag.ClearBag();
        ClearPowUpBoard();

        CoinWarning.SetActive(false);

        equipmentBag.ResetSort();
        weaponBag.ResetSort();
        armorBag.ResetSort();
    }

    /// <summary>
    /// 点击装备事件
    /// </summary>
    public void _OnClickEquipmentItemInter(int Id)
    {
        UserWare uw = UserManager.CurUserInfo.FindUserWare(Id);
        if(type == selectType.materialType)
        {
            if (uw.IsWare) return;
            equipmentItemInterface ei = equipmentBag.GetItemByUId(Id);
            if (ei.isBase) return;
            if (curBaseId == -1) return;
            if(ei.isMaterial)
            {
                foreach (PowerUpMaterialcontrol p in materials)
                {
                    if (p.uID == Id)
                    {
                        p.RemoveMaterial();
                        RepositionMaterial(ref materials);
                        getTotalExp -= GetHardwareExp(Id);
                        curPrice -= GetHardwareGold(Id);
                        SetLvUpBoard(curBaseId, getTotalExp);
                        SetPrice(curPrice);
                        SetEM(Id, false);
                        materialEs.Remove(Id);
                        materialNum--;
                        if (materialNum == 0)
                        {
                            ClearLvUpBoard();
                        }
                    }
                }
            }
            else if(materialNum < 5)
            {
                if (target_level >= UserManager.CurUserInfo.FindUserWare(curBaseId).CurHardWareData.LvlMax) return;
                AddMaterial(Id, PowerUpMaterialcontrol.materialType.hardware);
                curPrice += GetHardwareGold(Id);
                getTotalExp += GetHardwareExp(Id);
                SetLvUpBoard(curBaseId, getTotalExp);
                SetPrice(curPrice);
                SetEM(Id, true);
                materialEs.Add(Id);
            }
        }
        else if (type == selectType.baseType)
        {
            if (uw.Level >= uw.CurHardWareData.LvlMax) return;
            foreach (PowerUpMaterialcontrol p in materials)
            {
                if (Id == p.uID) return;
            }
            SetEB(curBaseId, false);
            SetBase(Id);
            SetEB(Id, true);
            ClearLvUpBoard();
            SetLvUpBoard(Id, getTotalExp);
            SwitchState(2);
        }

        if (curPrice > UserManager.CurUserInfo.Gold)
        {
            CoinWarning.SetActive(true);
        }
        else CoinWarning.SetActive(false);
    }

    /// <summary>
    /// 点击材料事件
    /// </summary>
    public void _OnClickMaterialInter(int Id)
    {
        MaterialItemInterface mi = materialBag.GetItemById(Id);
        if (type == selectType.materialType)
        {
            if (curBaseId == -1)
            {
                return;
            }
            if (mi.isMaterial)
            {
                foreach (PowerUpMaterialcontrol p in materials)
                {
                    if (p.uID == Id)
                    {
                        p.RemoveMaterial();
                        RepositionMaterial(ref materials);
                        getTotalExp -= GetItemExp(Id);
                        curPrice -= GetItemGold(Id);
                        SetLvUpBoard(curBaseId, getTotalExp);
                        SetPrice(curPrice);
                        SetMM(Id, false);
                        materialMs.Remove(Id);
                        materialNum--;
                        if (materialNum == 0)
                        {
                            ClearLvUpBoard();
                        }
                    }
                }
            }
            else if (materialNum < 5)
            {
                if (target_level >= UserManager.CurUserInfo.FindUserWare(curBaseId).CurHardWareData.LvlMax) return;

                AddMaterial(Id, PowerUpMaterialcontrol.materialType.item);
                curPrice += GetItemGold(Id);
                getTotalExp += GetItemExp(Id);
                SetLvUpBoard(curBaseId, getTotalExp);
                SetPrice(curPrice);
                SetMM(Id, true);
                materialMs.Add(Id);
            }
        }

        if (curPrice > UserManager.CurUserInfo.Gold)
        {
            CoinWarning.SetActive(true);
        }
        else CoinWarning.SetActive(false);
    }


    public void _OnClickBase()
    {
        SwitchState(1);
    }

    public void _OnClickMaterial()
    {
        SwitchState(2);
    }

    /// <summary>
    /// 强化按钮
    /// </summary>
    public void _OnClickPowerUp()
    {
        if (curBaseId != -1)
        {
            if (materialNum < 1) return;
            if (UserManager.CurUserInfo.Gold >= curPrice)
            {
                UserWare uw = UserManager.CurUserInfo.FindUserWare(curBaseId);
                JsonObject args = new JsonObject();
                JsonArray material_Es = new JsonArray();
                foreach (int i in materialEs)
                {
                    material_Es.Add(i);
                }
                JsonArray material_Ms = new JsonArray();
                foreach (int i in materialMs)
                {
                    material_Ms.Add(i);
                }
                args.Add("house_id", curBaseId);
                args.Add("weapon_ids", material_Es);
                args.Add("item_ids", material_Ms);
                SocketCenter.Request(GameRouteConfig.RiseHardware, args, (result) =>
                {
                    if (result.Code == SocketResult.ResultCode.Success)
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            JsonObject data = (JsonObject)result.Data["weapon"];
                            UserWare newUW = new UserWare(data);

                            newUW.IsWare = uw.IsWare;
                            if (newUW.IsWare)
                            {
                                if (newUW.IsWeapon())
                                {
                                    UserManager.CurUserInfo.CurWeapon = newUW;
                                }
                                else if (newUW.IsHelmet())
                                {
                                    UserManager.CurUserInfo.CurHelmet = newUW;
                                }
                                else if (newUW.IsArmor())
                                {
                                    UserManager.CurUserInfo.CurArmor = newUW;
                                }
                            }

                            //动画
                            PowUpAnimation.gameObject.SetActive(true);
                            PowUpAnimation.Hbase = new UserWare(data);
                            PowUpAnimation.Hbase.CurAtk = uw.CurAtk;
                            PowUpAnimation.Hbase.CurDef = uw.CurDef;
                            PowUpAnimation.Hbase.CurHardWareData = uw.CurHardWareData;
                            PowUpAnimation.Hbase.CurHp = uw.CurHp;
                            PowUpAnimation.Hbase.Exp = uw.Exp;
                            PowUpAnimation.Hbase.IsWare = uw.IsWare;
                            PowUpAnimation.Hbase.UserWareId = uw.UserWareId;
                            PowUpAnimation.HLvUp = new UserWare(data);
                            PowUpAnimation.SetAnimation(uw.CurHardWareData.SkinId, materials[0].SkinId, materials[1].SkinId, materials[2].SkinId, materials[3].SkinId, materials[4].SkinId);

                            UserManager.CurUserInfo.UserWares.Remove(uw);
                            UserManager.CurUserInfo.UserWares.Add(newUW);
                            JsonArray consume = (JsonArray)result.Data["consumes"];
                            UserManager.CurUserInfo.AddUserElements(consume);
                            JsonArray imts = (JsonArray)result.Data["item_materials"];
                            JsonArray emts = (JsonArray)result.Data["weapon_materials"];
                            
                            if ((int)uw.CurHardWareData.Style < 5)
                            {
                                weaponBag.DestroyItem(curBaseId);
                                weaponBag.AddEquipmentItem(newUW.Level, newUW.CurAtk, newUW.CurHardWareData.Element, newUW.CurHardWareData.SkinId, newUW.CurHardWareData.Rank, newUW.UserWareId);
                            }
                            else
                            {
                                armorBag.DestroyItem(curBaseId);
                                armorBag.AddEquipmentItem(newUW.Level, newUW.CurAtk, newUW.CurHardWareData.Element, newUW.CurHardWareData.SkinId, newUW.CurHardWareData.Rank, newUW.UserWareId);
                            }
                            equipmentBag.DestroyItem(curBaseId);
                            equipmentBag.AddEquipmentItem(newUW.Level, newUW.CurAtk, newUW.CurHardWareData.Element, newUW.CurHardWareData.SkinId, newUW.CurHardWareData.Rank, newUW.UserWareId);
                            for (int i = imts.Count - 1; i >= 0; i--)
                            {
                                int tempId = int.Parse(imts[i].ToString());
                                materialBag.RemoveItemById(tempId);
                                UserItem ui = UserManager.CurUserInfo.FindItemById(tempId);
                                UserManager.CurUserInfo.UserItems.Remove(ui);
                            }
                            for (int i = emts.Count - 1; i >= 0; i--)
                            {
                                int tempID = int.Parse(emts[i].ToString());
                                UserWare u = UserManager.CurUserInfo.FindUserWare(tempID);
                                UserManager.CurUserInfo.UserWares.Remove(u);
                                if ((int)u.CurHardWareData.Style < 5)
                                {
                                    weaponBag.DestroyItem(tempID);
                                }
                                else
                                {
                                    armorBag.DestroyItem(tempID);
                                }
                                equipmentBag.DestroyItem(tempID);
                            }
                            //PowUpAnime(materialNum);
                            DelayClearPowUpBoard();
                            ClearLvUpBoard();
                            levelUpBoard.SetActive(false); //暂时关掉升级显示
                            materialNum = 0;
                            getTotalExp = 0;
                            curPrice = 0;
                            weaponBagNum.RefreshNum();
                            armorBagNum.RefreshNum();
                            equipmentBagNum.RefreshNum();
                            materialBagNum.RefreshNum();
                            SetBase(newUW.UserWareId);
                            SwitchState(2);
                            UserInfo.gold.text = UserManager.CurUserInfo.Gold.ToString();
                            UserInfo.diamonds.text = UserManager.CurUserInfo.Diamond.ToString();
                        });
                    }
                }, null, true, true);
            }
        }
    }

    public void _OnLongPressMaterialInter(int Id)
    {
    }

    public void _OnLongPressEquipmentItemInter(int Id)
    {
    }

    public void _OnEquipmentClickRemoveInter(int Id)
    {
    }
}
