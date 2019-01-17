using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface _OnClickInter
{
    void _OnClickWeaponInter(int parIndex);             //点击武器更换
    void _OnLongPressWeaponInter(int parIndex);         //长按武器查看详情
    void _OnClickHelmetInter(int parIndex);             //点击头盔更换或移除
    void _OnLongPressHelmetInter(int parIndex);         //长按头盔查看详情
    void _OnClickArmorInter(int parIndex);              //点击盔甲更换或移除
    void _OnLongPressArmorInter(int parIndex);          //长按盔甲查看详情
    void _OnClickBlankInter(int parIndex, int position, int UserPetID);  //点击空怪物位置增加或更换怪物
    void _OnLongPressBlankInter(int parIndex, int position, int UserPetID);   //长按查看怪物详情
}

public class SetParty : MonoBehaviour, PartyWeapon, PartyHelmet, PartyArmor, AddMonster, _TurnDirectionInter
{
    /// <summary>
    /// 点击接口
    /// </summary>
    public _OnClickInter partyInter;

    /// <summary>
    /// 队伍编号
    /// </summary>
    public int selfIndex;

    private int curPosition;
    public int CurPosition
    {
        get { return petList.Count + 1; }
        set { curPosition = value; }
    }

    public int returnMonsterID(int Position)
    {
        foreach(GameObject g in petList)
        {
            PetUIController p = g.GetComponent<PetUIController>();
            if(p.position == Position)
                return p.userMonsterID;
        }
        return -1;
    }

    public GameObject BackButton;

    public GameObject partyBoard;
    public UILabel playerHP;
    public UILabel playerATK;
    public UILabel playerDEF;

    GameObject CurPlayerAnime;

    /// <summary>
    /// 更换怪物的怪物背包
    /// </summary>
    public BagControl bag;

    /// <summary>
    /// 更换装备
    /// </summary>
    public EquipmentBagControl equipmentBag;

    /// <summary>
    /// 四个怪物位置的按钮
    /// </summary>
    public GameObject Button_1;
    public GameObject Button_2;
    public GameObject Button_3;
    public GameObject Button_4;

    public PlayerWeaponController weapon;
    public PlayerHelmetController helmet;
    public PlayerArmorController armor;

    //技能信息
    public UILabel weaponSkill;
    public UILabel armorSkill;

    //装备栏套装效果
    public UISprite isSuit;
    public UILabel suitBuff;

    /// <summary>
    /// 总计数
    /// </summary>
    public UILabel totalHP;
    public UILabel totalHPBuff;
    public UILabel totalCost;

    public PlayerAvata CurPlayerAvata;

    public ButtonTurnDirection turnButton;
    DungeonEnum.FaceDirection CurDirection = DungeonEnum.FaceDirection.LeftDown;

    public bool ClearHpBuff = false;

    /// <summary>
    /// 设置玩家信息以及装备信息
    /// </summary>
    /// <param name="PlayerHP">玩家血量</param>
    /// <param name="PlayerATK">玩家攻击力</param>
    /// <param name="PlayerDEF">防御力</param>
    /// <param name="WeaponSkill">武器技能（文本）</param>
    /// <param name="ArmorSkill">头盔技能（文本）</param>
    /// <param name="WeaponID">武器的ID，用来生成武器sprite</param>
    /// <param name="WeaponLevel">武器的等级</param>
    /// <param name="WeaponElementType">武器的元素类型</param>
    /// <param name="HelmetID">头盔ID</param>
    /// <param name="HelmetLevel">头盔等级</param>
    /// <param name="HelmetElementType">头盔元素类型</param>
    /// <param name="ArmorID">护甲ID</param>
    /// <param name="ArmorLevel">护甲等级</param>
    /// <param name="ArmorElementType">护甲元素类型</param>
    /// <param name="IsSuit">是否为套装</param>
    /// <param name="SuitBuff">套装技能（文本）</param>
    /// <param name="PlayerModelName">玩家人物预设名</param>
    public void SetPlayerInfo(
        string PlayerPrefabID,
        int PlayerHP,
        int PlayerATK,
        int PlayerDEF,
        string WeaponSkill,
        string ArmorSkill,
        bool IsSuit,
        string SuitBuff,
        int Warefare, 
        //int TotalHpBuff, 
        int CurrentCost,
        int MaxCost
        )
    {
        if (CurPlayerAvata == null)
        {
            GameObject playerAnime;
            playerAnime = Resources.Load("PreFabs/Characters/" + PlayerPrefabID) as GameObject;
            GameObject temp;
            temp = NGUITools.AddChild(partyBoard, playerAnime);

            CurPlayerAvata = temp.GetComponent<PlayerAvata>();

            PetUIController.SetLayer(temp.transform, LayerHelper.Unit);
            temp.transform.localScale = new Vector3(100, 100, 1);
            temp.transform.localPosition = new Vector3(-185f, 85f, -1);

            if (CurPlayerAnime != null)
            {
                Destroy(CurPlayerAnime);
            }
            CurPlayerAnime = temp;
        }

        //if(PlayerHP > UserManager.CurUserInfo.CurHp)
        //{
        //    playerHP.color = Color.green;
        //}
        //else if(PlayerHP < UserManager.CurUserInfo.CurHp)
        //{
        //    playerHP.color = Color.red;
        //}
        //else
        //{
        //    playerHP.color = new Color(250f / 255f, 200f / 255f, 95f / 255f);
        //}
        playerHP.text = PlayerHP.ToString();

        //if (PlayerATK > UserManager.CurUserInfo.CurAtk)
        //{
        //    playerATK.color = Color.green;
        //}
        //else if (PlayerATK < UserManager.CurUserInfo.CurAtk)
        //{
        //    playerATK.color = Color.red;
        //}
        //else
        //{
        //    playerATK.color = new Color(250f / 255f, 200f / 255f, 95f / 255f);
        //}
        playerATK.text = PlayerATK.ToString();

        //if (PlayerDEF > UserManager.CurUserInfo.CurDef)
        //{
        //    playerDEF.color = Color.green;
        //}
        //else if (PlayerDEF < UserManager.CurUserInfo.CurDef)
        //{
        //    playerDEF.color = Color.red;
        //}
        //else
        //{
        //    playerDEF.color = new Color(250f / 255f, 200f / 255f, 95f / 255f);
        //}
        playerDEF.text = PlayerDEF.ToString();

        weaponSkill.text = WeaponSkill;
        armorSkill.text = ArmorSkill;
        weapon.weaponInter = this;
        helmet.helmetInter = this;
        armor.armorInter = this;

        isSuit.gameObject.SetActive(IsSuit);
        if(IsSuit)
        {
            suitBuff.text = SuitBuff;
        }
        else
        {
            suitBuff.text = "--";
        }
        //totalHP.text = Warefare.ToString();
        //if (TotalHpBuff > 0)
        //{
        //    totalHPBuff.color = Color.red;
        //    totalHPBuff.text = "+" + TotalHpBuff.ToString();
        //}
        //else if (TotalHpBuff < 0)
        //{
        //    totalHPBuff.color = Color.blue;
        //    totalHPBuff.text = TotalHpBuff.ToString();
        //}
        //else totalHPBuff.text = null;

        totalCost.text = CurrentCost.ToString() + "/" + MaxCost.ToString();
        totalCost.color = new Color(250f / 255f, 200f / 255f, 95f / 255f);
    }

    public GameObject pet;
    public UIGrid pets;
    public UIGrid MonsterBag;
    public UIGrid EquipmentBag;
    public List<GameObject> petList = new List<GameObject>();


    /// <summary>
    /// 添加怪物
    /// </summary>
    /// <param name="PartnerLevel">等级</param>
    /// <param name="PartnerHP">血量</param>
    /// <param name="PartNerATK">攻击</param>
    /// <param name="PartnerCost">领导力</param>
    /// <param name="PetAnime">动画预设</param>
    /// <param name="PartnerElementType">元素类型</param>
    /// <param name="UserPetID">玩家该怪物的唯一标识</param>
    public void AddPet(
        int PartnerLevel,
        int PartnerHP,
        int PartNerATK,
        int PartnerCost,
        string PetAnimeID,
        DungeonEnum.ElementAttributes PartnerElementType,
        int UserPetID
        )
    {
        GameObject Pet;
        Pet = NGUITools.AddChild(pets.gameObject, pet);
        petList.Add(Pet);
        Pet.GetComponent<PetUIController>().position = petList.Count;
        Pet.GetComponent<PetUIController>().SetPet(PartnerLevel, PartnerHP, PartNerATK, PartnerCost, PetAnimeID, PartnerElementType, UserPetID);
        Pet.name = petList.Count.ToString();
        pets.repositionNow = true;
    }



    public void ClearPets() //清空tableview
    {
        for (int i = petList.Count - 1; i >= 0; i--)
        {
            GameObject tg = petList[i];
            petList.Remove(petList[i]);
            Destroy(tg);
        }
    }

    public void RemoveList(int UserMonsterID)
    {
    }

    /// <summary>
    /// 按UserMonsterID去掉某个怪物
    /// </summary>
    /// <param name="Position"></param>
    public void RemovePet(int UserMonsterID)
    {
        bool positionChange = true;
        for (int i = petList.Count - 1; i >= 0; i--)
        {
            PetUIController p = petList[i].GetComponent<PetUIController>();
            if (positionChange)
            {
                p.position--;
            }
            if (p.userMonsterID == UserMonsterID)
            {
                GameObject tg = petList[i];
                petList.Remove(petList[i]);
                Destroy(tg);
                positionChange = false;
            }
        }
        pets.repositionNow = true;
    }

    /// <summary>
    /// 改变指定位置的monster
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="PartnerLevel"></param>
    /// <param name="PartnerHP"></param>
    /// <param name="PartNerATK"></param>
    /// <param name="PartnerCost"></param>
    /// <param name="PetAnimeID"></param>
    /// <param name="PartnerElementType"></param>
    /// <param name="UserPetID"></param>
    public void ChangeMonster(
        int Position,
        int PartnerLevel,
        int PartnerHP,
        int PartNerATK,
        int PartnerCost,
        string PetAnimeID,
        DungeonEnum.ElementAttributes PartnerElementType,
        int UserPetID)
    {
        foreach (GameObject g in petList)
        {
            PetUIController p = g.GetComponent<PetUIController>();
            if (p.position == Position)
            {
                GameObject tg;
                tg = g.transform.FindChild("CharacterAnime").gameObject;
                Destroy(tg);
                p.SetPet(PartnerLevel, PartnerHP, PartNerATK, PartnerCost, PetAnimeID, PartnerElementType, UserPetID);
                g.name = UserPetID.ToString();
            }
        }
        pets.repositionNow = true;
    }

    public GameObject curPanel;
    public GameObject detailPanel;
    public GameObject WeaponDetailPanel;
    public GameObject ArmorDetailPanel;
    public GameObject PlayerDetail;

    /// <summary>
    /// 跳转到详细信息panel
    /// </summary>
    public void SwitchPetDetailPanel()
    {
        detailPanel.SetActive(true);
    }

    void Start()
    {
		this.StartOperater ();
    }

	protected virtual void StartOperater()
	{
		Button_1.GetComponent<ButtonAddMonster>().blankInter = this;
		Button_2.GetComponent<ButtonAddMonster>().blankInter = this;
		Button_3.GetComponent<ButtonAddMonster>().blankInter = this;
		Button_4.GetComponent<ButtonAddMonster>().blankInter = this;
		//AddPet(1, 1, 1, 1, "S360", DungeonEnum.ElementAttributes.Fire, 1);
		//AddPet(1, 1, 1, 1, "S360", DungeonEnum.ElementAttributes.Fire, 1);
		//SetPlayerInfo("", 1, 1, 1, "", "", "", 1, DungeonEnum.ElementAttributes.Fire, "", 1, DungeonEnum.ElementAttributes.Fire, "", 1, DungeonEnum.ElementAttributes.Fire, true, "", 1, 1, 1, 1);
		//bag.CreateSetItem(1, 1, 1, 1, DungeonEnum.ElementAttributes.Fire, "", 6, 1);
		//equipmentBag.AddEquipmentItem(1, 1, DungeonEnum.ElementAttributes.Water, "", 1, 1);
		
		UIEventListener.Get(BackButton).onClick = (go) =>
		{
			ClearHpBUFF();
		};
		
		UIEventListener.Get(partyBoard).onClick = (go) =>
		{
			int cur = CurPlayerAnime.GetComponent<Animator>().GetInteger("ActionState");
			if (cur == 10||cur==0)
			{
				int random = Random.Range(0, 2);
				if (random == 0)
				{
					if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Heavy || UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Light)
					{
						CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", 30);
						Invoke("BackWaiting", 1);
					}
					else
					{
						if (CurDirection == DungeonEnum.FaceDirection.LeftDown)
						{
							if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far1)
							{
								CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", (int)PveFightUnit.ActionState.Far1Attack);
							}
							else if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far2)
							{
                                CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", (int)PveFightUnit.ActionState.Far2Attack);
							}
							Invoke("BackWaiting", 1);
						}
					}
				}
				else if (random == 1)
				{
					CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", 40);
				}
			}
			else if (cur == 40)
			{
				int random = Random.Range(0, 2);
				if (random == 0)
				{
					if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Heavy || UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Light)
					{
						CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", 30);
						Invoke("BackWaiting", 1);
					}
					else
					{
						if (CurDirection == DungeonEnum.FaceDirection.LeftDown)
						{
							if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far1)
							{
                                CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", (int)PveFightUnit.ActionState.Far1Attack);
							}
							else if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far2)
							{
                                CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", (int)PveFightUnit.ActionState.Far2Attack);
							}
							Invoke("BackWaiting", 1);
						}
					}
				}
				else if (random == 1)
				{
					CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", 10);
				}
			}
		};
	}

    void BackWaiting()
    {
        CurPlayerAnime.GetComponent<Animator>().SetInteger("ActionState", 10);
    }

    bool blankClicked = false;


    public mViewControl CurMonsterView;

    void OnEnable()
    {
        turnButton.inter = this;
        BagControl.NotInParty = false;

        //foreach(GameObject g in petList)
        //{
        //    PetUIController puc = g.GetComponent<PetUIController>();
        //    puc.SetPet(puc.userMonsterID);
        //}

        if (CurPlayerAvata == null)
        {
            GameObject playerAnime;
            playerAnime = Resources.Load("PreFabs/Characters/S10060") as GameObject;
            GameObject temp;
            temp = NGUITools.AddChild(partyBoard, playerAnime);

            CurPlayerAvata = temp.GetComponent<PlayerAvata>();

            PetUIController.SetLayer(temp.transform, LayerHelper.Unit);
            temp.transform.localScale = new Vector3(100, 100, 1);
            temp.transform.localPosition = new Vector3(-185f, 85f, -1);

            if (UserManager.CurUserInfo.UserPartys[selfIndex].weapon != null)
            {
                CurPlayerAvata.AddAvataWare(UserManager.CurUserInfo.UserPartys[selfIndex].weapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
            }
            if (UserManager.CurUserInfo.UserPartys[selfIndex].helmet != null)
            {
                CurPlayerAvata.AddAvataWare(UserManager.CurUserInfo.UserPartys[selfIndex].helmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            }
            if (UserManager.CurUserInfo.UserPartys[selfIndex].armor != null)
            {
                CurPlayerAvata.AddAvataWare(UserManager.CurUserInfo.UserPartys[selfIndex].armor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            }
            CurPlayerAvata.WeaponEffectShow();

            if (CurPlayerAnime != null)
            {
                Destroy(CurPlayerAnime);
            }
            CurPlayerAnime = temp;
        }
    }


    /// <summary>
    /// 接口实现
    /// </summary>
    public void _OnClickHelmet()
    {
        if (partyInter != null)
        {
            partyInter._OnClickHelmetInter(selfIndex);
        }
    }
    public void _OnLongPressHelmet(int userWareId)
    {
        if (partyInter != null && userWareId != -1)
        {
            partyInter._OnLongPressHelmetInter(selfIndex);
            ArmorDetailPanel.SetActive(true);
            ArmorDetailPanel.GetComponent<ArmorDetail>().SetDetail(userWareId);
        }
    }
    public void _OnClickArmor()
    {
        if (partyInter != null)
        {
            partyInter._OnClickArmorInter(selfIndex);
        }
    }
    public void _OnLongPressArmor(int userWareId)
    {
        if (partyInter != null && userWareId != -1)
        {
            partyInter._OnLongPressArmorInter(selfIndex);
            ArmorDetailPanel.SetActive(true);
            ArmorDetailPanel.GetComponent<ArmorDetail>().SetDetail(userWareId);
        }
    }
    public void _OnClickWeapon()
    {
        if (partyInter != null)
        {
            partyInter._OnClickWeaponInter(selfIndex);
        }
    }
    public void _OnLongPressWeapon(int userWareId)
    {
        if (partyInter != null && userWareId != -1)
        {
            partyInter._OnLongPressWeaponInter(selfIndex);
            WeaponDetailPanel.SetActive(true);
            WeaponDetailPanel.GetComponent<WeaponDetail>().SetDetail(userWareId);
        }
    }

    public void OnClickSkill()
    {
        PlayerDetail.SetActive(true);
        PlayerDetail.GetComponent<PlayerDetail>().SetPlayerInfo();
    }

    public void _OnClickBlank(int position)
    {
        if (partyInter != null && blankClicked == false)
        {
            bag.ClickPosition = position;
            bag.ClickUserMonsterID = returnMonsterID(position);
            if (position > 1 && bag.removeItem != null && position <= petList.Count)
            {
                GameObject g;
                g = NGUITools.AddChild(MonsterBag.gameObject, bag.removeItem);
                g.name = "000rmIcon";
                g.GetComponent<RemovePet>().rInter = bag;
                blankClicked = true;
            }
            partyInter._OnClickBlankInter(selfIndex, position, returnMonsterID(position));
        }
    }

    public void _OnLongPressBlank(int position)
    {
        if (partyInter != null && petList.Count >= position)
        {
            partyInter._OnLongPressBlankInter(selfIndex, position, returnMonsterID(position));
            SwitchPetDetailPanel();
            detailPanel.GetComponent<SetMonsterDetail>().SetDetail(returnMonsterID(position));
        }
    }

    /// <summary>
    /// 向左转身按钮
    /// </summary>
    public void _OnClickTurnButton()
    {
        CurPlayerAnime = CurPlayerAvata.gameObject;
        Destroy(CurPlayerAnime);
        int direction = (int)CurDirection;
        if (direction == 20 || direction == 60) direction += 10;
        direction = (((direction + 10) % 80) > 0) ? ((direction + 10) % 80) : 80;
        CurDirection = (DungeonEnum.FaceDirection)direction;
        PlayerAvata p = Resources.Load<PlayerAvata>("PreFabs/Characters/S100" + ((int)CurDirection).ToString());
        GameObject temp;
        temp = NGUITools.AddChild(partyBoard, p.gameObject);
        CurPlayerAnime = temp;
        CurPlayerAvata = temp.GetComponent<PlayerAvata>();
        PetUIController.SetLayer(temp.transform, LayerHelper.Unit);
        if ((int)CurDirection == 40 || (int)CurDirection == 50 || (int)CurDirection == 80)
        {
            temp.transform.localScale = new Vector3(-100, 100, 1);
        }
        else
        {
            temp.transform.localScale = new Vector3(100, 100, 1);
        }
        temp.transform.localPosition = new Vector3(-185f, 85f, -1);
        Invoke("DelayAddWare", 0.005f);
    }

    public void ClearHpBUFF()
    {
        totalHPBuff.text = null;
    }

    void DelayAddWare()
    {
        CurPlayerAvata.AddAvataWare(UserManager.CurUserInfo.UserPartys[selfIndex].weapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
        if (UserManager.CurUserInfo.UserPartys[selfIndex].helmet != null) CurPlayerAvata.AddAvataWare(UserManager.CurUserInfo.UserPartys[selfIndex].helmet.CurHardWareData.SkinId, CurDirection);
        if (UserManager.CurUserInfo.UserPartys[selfIndex].armor != null) CurPlayerAvata.AddAvataWare(UserManager.CurUserInfo.UserPartys[selfIndex].armor.CurHardWareData.SkinId, CurDirection);
        CurPlayerAvata.WeaponEffectShow();
    }

    void OnDisable()
    {
        ClearHpBuff = true;
        blankClicked = false;
        Destroy(CurPlayerAnime);
    }
}