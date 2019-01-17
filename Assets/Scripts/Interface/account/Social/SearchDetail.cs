using UnityEngine;
using System.Collections;

public class SearchDetail : MonoBehaviour, EquipmentiIemInterface
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

    public GameObject AddButton;
    public GameObject ResultBoard;
    public GameObject ReturnButton_1;

    public void SetFriendDetail(FriendInfo f)
    {
        NickName.text = f.NickName;
        Level.text = "Lv." + f.FriendLevel.ToString();
        ID.text = f.FriendId.ToString();
        Debug.Log(f.PvPRank);
        PvPRank.mainTexture = Tools.GetPvPIcon(f.PvPRank);
        if (f.FriendWeapon != null)
        {
            WeapenData = f.FriendWeapon;
            PlayerAnime.AddAvataWare(f.FriendWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
            Weapon.gameObject.SetActive(true);
            Weapon.equipmentItemInter = this;
            Weapon.SetItem(f.FriendWeapon);

            if(!string.IsNullOrEmpty(f.FriendWeapon.CurHardWareData.SkillAffix1))
            {
                SkillData sd = ConfigManager.SkillConfig.GetSkillById(f.FriendWeapon.CurHardWareData.SkillAffix1);
                Skill_1.text = sd.Name;
            }
            if(!string.IsNullOrEmpty(f.FriendWeapon.CurHardWareData.SkillAffix2))
            {
                SkillData sd = ConfigManager.SkillConfig.GetSkillById(f.FriendWeapon.CurHardWareData.SkillAffix2);
                Skill_2.text = sd.Name;
            }
        }
        else
        {
            Weapon.gameObject.SetActive(false);
        }
        if (f.FriendHelmet != null)
        {
            HelmetData = f.FriendHelmet;
            PlayerAnime.AddAvataWare(f.FriendHelmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Helmet.gameObject.SetActive(true);
            Helmet.equipmentItemInter = this;
            Helmet.SetItem(f.FriendHelmet);
        }
        else
        {
            Helmet.gameObject.SetActive(false);
        }
        if (f.FriendArmor != null)
        {
            ArmorData = f.FriendArmor;
            PlayerAnime.AddAvataWare(f.FriendArmor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Armor.gameObject.SetActive(true);
            Armor.equipmentItemInter = this;
            Armor.SetItem(f.FriendArmor);
        }
        else
        {
            Armor.gameObject.SetActive(false);
        }
        HeroData h = ConfigManager.HeroConfig.GetHeroByLvl(f.FriendLevel);
        int attack = h.Attack;
        if (f.FriendWeapon != null)
        {
            attack += f.FriendWeapon.CurAtk;
        }
        Atk.text = attack.ToString();

        int defense = h.Def;
        if (f.FriendHelmet != null)
        {
            defense += f.FriendHelmet.CurDef;
        }
        if (f.FriendArmor != null)
        {
            defense += f.FriendArmor.CurDef;
        }
        Hp.text = h.Hp.ToString();
        Def.text = defense.ToString();

        AddButton.GetComponent<ButtonAddFriend>().Uid = f.FriendId;
    }


    void Start()
    {
        UIEventListener.Get(ReturnButton_1).onClick = (g) =>
        {
            ResultBoard.SetActive(false);
        };
    }
    public void _OnClickEquipmentItem(int UwareId)
    {

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

    void OnDisable()
    {
        PlayerAnime.ClearAvata();
    }

}
