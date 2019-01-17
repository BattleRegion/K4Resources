using UnityEngine;
using System.Collections;

public class RequestDetail : MonoBehaviour, EquipmentiIemInterface
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

    public GameObject AcceptButton;
    public GameObject DenyButton;

    public void SetRequestDetail(RequestInfo r)
    {
        NickName.text = r.NickName;
        Level.text = "Lv." + r.Level.ToString();
        ID.text = r.UserId.ToString();
        PvpRank.mainTexture = Tools.GetPvPIcon(r.PvPRank);
        if (r.FriendWeapon != null)
        {
            PlayerAnime.AddAvataWare(r.FriendWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
            Weapon.gameObject.SetActive(true);
            Weapon.equipmentItemInter = this;
            Weapon.SetItem(r.FriendWeapon);

            if (!string.IsNullOrEmpty(r.FriendWeapon.CurHardWareData.SkillAffix1))
            {
                SkillData sd = ConfigManager.SkillConfig.GetSkillById(r.FriendWeapon.CurHardWareData.SkillAffix1);
                Skill_1.text = sd.Name;
            }
            if (!string.IsNullOrEmpty(r.FriendWeapon.CurHardWareData.SkillAffix2))
            {
                SkillData sd = ConfigManager.SkillConfig.GetSkillById(r.FriendWeapon.CurHardWareData.SkillAffix2);
                Skill_2.text = sd.Name;
            }
        }
        else
        {
            Weapon.gameObject.SetActive(false);
        }
        if (r.FriendHelmet != null)
        {
            PlayerAnime.AddAvataWare(r.FriendHelmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Helmet.gameObject.SetActive(true);
            Helmet.equipmentItemInter = this;
            Helmet.SetItem(r.FriendHelmet);
        }
        else
        {
            Helmet.gameObject.SetActive(false);
        }
        if (r.FriendArmor != null)
        {
            PlayerAnime.AddAvataWare(r.FriendArmor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
            Armor.gameObject.SetActive(true);
            Armor.equipmentItemInter = this;
            Armor.SetItem(r.FriendArmor);
        }
        else
        {
            Armor.gameObject.SetActive(false);
        }
        Skill_1.text = "-";
        Skill_2.text = "-";
        HeroData h = ConfigManager.HeroConfig.GetHeroByLvl(r.Level);
        int attack = h.Attack;
        if (r.FriendWeapon != null)
        {
            attack += r.FriendWeapon.CurAtk;
        }
        Atk.text = attack.ToString();

        int defense = h.Def;
        if (r.FriendHelmet != null)
        {
            defense += r.FriendHelmet.CurDef;
        }
        if (r.FriendArmor != null)
        {
            defense += r.FriendArmor.CurDef;
        }
        Hp.text = h.Hp.ToString();
        Def.text = defense.ToString();

        if (AcceptButton != null)
        {
            AcceptButton.GetComponent<ButtonAcceptRequest>().RequestId = r.RequestId;
        }
        if (DenyButton != null)
        {
            DenyButton.GetComponent<ButtonDenyRequest>().RequestId = r.RequestId;
        }
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

    void OnDisable()
    {
        PlayerAnime.ClearAvata();
    }

}
