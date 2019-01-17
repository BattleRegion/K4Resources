using UnityEngine;
using System.Collections;

public class FriendDetail : MonoBehaviour, EquipmentiIemInterface
{
    public UILabel NickName;
    public UILabel Level;
    public UILabel ID;
    public PlayerAvata PlayerAnime;
    public equipmentItemInterface Weapon;
    public equipmentItemInterface Helmet;
    public equipmentItemInterface Armor;
    public UILabel Skill_1;
    public UILabel Skill_2;
    public UILabel Atk;
    public UILabel Hp;
    public UILabel Def;

    public UITexture PvpRank;

    public GameObject WeaponDetailView;
    public GameObject ArmorDetailView;

    public GameObject PreRmButton;
    public GameObject RmButton = null;
    public GameObject ReturnButton_1;

    public GameObject ConfirmBoard;

    void OnDisable()
    {
        PlayerAnime.ClearAvata();
    }

    public void SetFriendDetail(FriendInfo f)
    {
        NickName.text = f.NickName;
        Level.text = "Lv." + f.FriendLevel.ToString();
        ID.text = f.FriendId.ToString();
        PvpRank.mainTexture = Tools.GetPvPIcon(f.PvPRank);
        if (f.FriendWeapon != null)
        {
            PlayerAnime.AddAvataWare(f.FriendWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
            Weapon.gameObject.SetActive(true);
            Weapon.equipmentItemInter = this;
            Weapon.SetItem(f.FriendWeapon);

            if (!string.IsNullOrEmpty(f.FriendWeapon.CurHardWareData.SkillAffix1))
            {
                SkillData sd = ConfigManager.SkillConfig.GetSkillById(f.FriendWeapon.CurHardWareData.SkillAffix1);
                Skill_1.text = sd.Name;
            }
            if (!string.IsNullOrEmpty(f.FriendWeapon.CurHardWareData.SkillAffix2))
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
            PlayerAnime.AddAvataWare(f.FriendArmor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Armor.gameObject.SetActive(true);
            Armor.equipmentItemInter = this;
            Armor.SetItem(f.FriendArmor);
        }
        else
        {
            Armor.gameObject.SetActive(false);
        }
        Skill_1.text = "-";
        Skill_2.text = "-";
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

        if (RmButton != null)
        {
            RmButton.GetComponent<ButtonRmFriend>().Uid = f.FriendId;
        }
    }


    void Start()
    {
        UIEventListener.Get(PreRmButton).onClick = (g) =>
        {
            ConfirmBoard.SetActive(true);
        };
        UIEventListener.Get(ReturnButton_1).onClick = (g) =>
        {
            ConfirmBoard.SetActive(false);
        };
    }
    public void _OnClickEquipmentItem(int UwareId)
    {

    }

    public void _OnLongPressEquipmentItem(int UwareId)
    {
        UserWare u = null;
        foreach (FriendInfo f in UserManager.CurUserInfo.UserFriends)
        {
            if (UwareId == f.FriendWeapon.UserWareId)
            {
                u = f.FriendWeapon;
                break; 
            }
            else if (UwareId == f.FriendHelmet.UserWareId)
            {
                u = f.FriendHelmet;
                break;
            }
            else if (UwareId == f.FriendArmor.UserWareId)
            {
                u = f.FriendArmor;
                break;
            }
        }
        if (u == null)
        {
            foreach (RequestInfo r in UserManager.CurUserInfo.UserRequests)
            {
                if (UwareId == r.FriendWeapon.UserWareId)
                {
                    u = r.FriendWeapon;
                    break;
                }
                else if (UwareId == r.FriendHelmet.UserWareId)
                {
                    u = r.FriendHelmet;
                    break;
                }
                else if (UwareId == r.FriendArmor.UserWareId)
                {
                    u = r.FriendArmor;
                    break;
                }
            }
        }

        if ((int)u.CurHardWareData.Style < 5)
        {
            WeaponDetailView.SetActive(true);
            WeaponDetailView.GetComponent<WeaponDetail>().SetDetail(UwareId);
        }
        else
        {
            ArmorDetailView.SetActive(true);
            ArmorDetailView.GetComponent<ArmorDetail>().SetDetail(UwareId);
        }
    }

}
