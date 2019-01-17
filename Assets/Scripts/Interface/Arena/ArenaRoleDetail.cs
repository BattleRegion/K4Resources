using UnityEngine;
using System.Collections;

public class ArenaRoleDetail : MonoBehaviour, EquipmentiIemInterface
{
    public UILabel NickName;
    public UILabel Level;
    public UILabel ID;
    public PlayerAvata PlayerAnime;
    public equipmentItemInterface Weapon;
    public equipmentItemInterface Helmet;
    public equipmentItemInterface Armor;

    public UserWare WeapenData;
    public UserWare HelmetData;
    public UserWare ArmorData;

    public UILabel Skill_1;
    public UILabel Skill_2;
    public UILabel Atk;
    public UILabel Hp;
	public UILabel Def;
	
	public UITexture PvPRank;

    public GameObject WeaponDetailView;
    public GameObject ArmorDetailView;

	public GameObject ResultBoard;
	public UILabel InfoLabel;

	private ArenaPrevRankInfo rankInfo;

    public void SetRoleDetail(ArenaPrevRankInfo rankInfo)
    {
		this.rankInfo = rankInfo;

		NickName.text = rankInfo.name;
		Level.text = "Lv." + rankInfo.level.ToString();
		ID.text = rankInfo.id.ToString();
		PvPRank.mainTexture = Tools.GetPvPIcon(rankInfo.pvpRank);
		if (rankInfo.weapon != null)
        {
			WeapenData = rankInfo.weapon;
			PlayerAnime.AddAvataWare(rankInfo.weapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
            Weapon.gameObject.SetActive(true);
            Weapon.equipmentItemInter = this;
			Weapon.SetItem(rankInfo.weapon.Level, (int)rankInfo.weapon.CurAtk, rankInfo.weapon.CurHardWareData.Element, rankInfo.weapon.CurHardWareData.SkinId, false, rankInfo.weapon.CurHardWareData.Rank, rankInfo.weapon.UserWareId);

			if(!string.IsNullOrEmpty(rankInfo.weapon.CurHardWareData.SkillAffix1))
			{
				SkillData sd = ConfigManager.SkillConfig.GetSkillById(rankInfo.weapon.CurHardWareData.SkillAffix1);
				Skill_1.text = sd.Name;
			}
			if(!string.IsNullOrEmpty(rankInfo.weapon.CurHardWareData.SkillAffix2))
			{
				SkillData sd = ConfigManager.SkillConfig.GetSkillById(rankInfo.weapon.CurHardWareData.SkillAffix2);
				Skill_2.text = sd.Name;
			}
        }
        else
        {
            Weapon.gameObject.SetActive(false);
        }
		if (rankInfo.helmet != null)
        {
			HelmetData = rankInfo.helmet;
			PlayerAnime.AddAvataWare(rankInfo.helmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Helmet.gameObject.SetActive(true);
            Helmet.equipmentItemInter = this;
			Helmet.SetItem(rankInfo.helmet.Level, (int)rankInfo.helmet.CurAtk, rankInfo.helmet.CurHardWareData.Element, rankInfo.helmet.CurHardWareData.SkinId, false, rankInfo.helmet.CurHardWareData.Rank, rankInfo.helmet.UserWareId);
        }
        else
        {
            Helmet.gameObject.SetActive(false);
        }
		if (rankInfo.armor != null)
        {
			ArmorData = rankInfo.armor;
			PlayerAnime.AddAvataWare(rankInfo.armor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Armor.gameObject.SetActive(true);
            Armor.equipmentItemInter = this;
			Armor.SetItem(rankInfo.armor.Level, (int)rankInfo.armor.CurAtk, rankInfo.armor.CurHardWareData.Element, rankInfo.armor.CurHardWareData.SkinId, false, rankInfo.armor.CurHardWareData.Rank, rankInfo.armor.UserWareId);
        }
        else
        {
            Armor.gameObject.SetActive(false);
        }
		HeroData h = ConfigManager.HeroConfig.GetHeroByLvl(rankInfo.level);
        int attack = h.Attack;
		if (rankInfo.weapon != null)
        {
			attack += rankInfo.weapon.CurAtk;
        }
        Atk.text = attack.ToString();

        int defense = h.Def;
		if (rankInfo.helmet != null)
        {
			defense += rankInfo.helmet.CurDef;
        }
		if (rankInfo.armor != null)
        {
			defense += rankInfo.armor.CurDef;
        }
        Hp.text = h.Hp.ToString();
        Def.text = defense.ToString();
    }
    public void _OnClickEquipmentItem(int UwareId)
    {
		if (UwareId == WeapenData.UserWareId)
		{
			WeaponDetailView.SetActive(true);
			WeaponDetailView.GetComponent<WeaponDetail>().SetDetail(WeapenData);
		}
		else if (UwareId == HelmetData.UserWareId)
		{
			ArmorDetailView.SetActive(true);
			ArmorDetailView.GetComponent<ArmorDetail>().SetDetail(HelmetData);
		}
		else
		{
			ArmorDetailView.SetActive(true);
			ArmorDetailView.GetComponent<ArmorDetail>().SetDetail(ArmorData);
		}
    }

    public void _OnLongPressEquipmentItem(int UwareId)
    {
        if (UwareId == WeapenData.UserWareId)
        {
            WeaponDetailView.SetActive(true);
            WeaponDetailView.GetComponent<WeaponDetail>().SetDetail(WeapenData);
        }
        else if (UwareId == HelmetData.UserWareId)
        {
            ArmorDetailView.SetActive(true);
            ArmorDetailView.GetComponent<ArmorDetail>().SetDetail(HelmetData);
        }
        else
        {
            ArmorDetailView.SetActive(true);
            ArmorDetailView.GetComponent<ArmorDetail>().SetDetail(ArmorData);
        }
    }

	public void AddFriendClickHandler()
	{
		if(this.rankInfo == null) return;

		FriendControl.SendFriendRequest(this.rankInfo.id, (r) =>
		{
			if (r == FriendControl.FriendMessageResult.Success)
			{
				Loom.QueueOnMainThread(() =>
				{
					this.InfoLabel.text = "申请已发送";
					this.ResultBoard.gameObject.SetActive(true);
				});
			}
			else
			{
				Loom.QueueOnMainThread(() =>
				{
					this.InfoLabel.text = "申请发送失败";
					this.ResultBoard.gameObject.SetActive(true);
				});
			}
		});
	}

	public void ResultBoardHidden()
	{
		this.ResultBoard.gameObject.SetActive(false);
	}

    void OnDisable()
    {
		this.gameObject.SetActive (false);
        PlayerAnime.ClearAvata();
    }

}
