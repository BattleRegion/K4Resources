using UnityEngine;
using System.Collections;

public class PlayerDetail : MonoBehaviour
{
    #region NGUI
    public UILabel player_level;
    public UILabel player_nickname;

    public UILabel txtName;
    public ArenaStarItemList arenaStarItemList;
    public ArenaIconItem arenaIconItem;

    public UILabel player_hp;
    public UILabel player_attack;
    public UILabel player_defense;
    public UILabel player_cost;
    public UILabel player_warfare;

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

    public GameObject Skill_3;

    public UILabel Skill_Name_3;
    public UILabel Skill_Description_3;
    public UISprite Skill_Map_3;
    #endregion

    public PlayerAvata current_player;

    public GameObject UnitCamera;

    public void SetPlayerInfo()
    {
        player_level.text = UserManager.CurUserInfo.Level.ToString();
        player_nickname.text = UserManager.CurUserInfo.NickName;

        ArenaData arenaData = ConfigManager.ArenaConfig.GetArenaByLv(UserManager.CurUserInfo.ArenaStarLevel);
        if (arenaData != null)
        {
            this.txtName.text = arenaData.RankName;
            if (arenaData.RankStar != 0)
            {
                // 显示状态
                this.arenaStarItemList.ShowData(true, 0);
                // 设置星级
                this.arenaStarItemList.ChangeData(UserManager.CurUserInfo.ArenaStarExp - arenaData.RankExp, arenaData.RankStar);
            }
            else
            {
                // 显示状态
                this.arenaStarItemList.ShowData(false, UserManager.CurUserInfo.ArenaRank);
            }
            // 设置图标
            this.arenaIconItem.ChangeData(UserManager.CurUserInfo.ArenaStarLevel);
        }

        //if (UserManager.pveUserInfo.CurHp > UserManager.CurUserInfo.CurHp)
        //{
        //    player_hp.color = Color.green;
        //}
        //else if (UserManager.pveUserInfo.CurHp < UserManager.CurUserInfo.CurHp)
        //{
        //    player_hp.color = Color.red;
        //}
        //else
        //{
        //    player_hp.color = Color.white;
        //}
        //player_hp.text = UserManager.pveUserInfo.CurHp.ToString();
        player_hp.text = UserManager.CurUserInfo.CurHp.ToString();

        //if (UserManager.CurUserInfo.CurAtk > UserManager.CurUserInfo.CurAtk)
        //{
        //    player_attack.color = Color.green;
        //}
        //else if (UserManager.CurUserInfo.CurAtk < UserManager.CurUserInfo.CurAtk)
        //{
        //    player_attack.color = Color.red;
        //}
        //else
        //{
        //    player_attack.color = Color.white;
        //}
        player_attack.text = UserManager.CurUserInfo.CurAtk.ToString();

        //if (UserManager.CurUserInfo.CurDef > UserManager.CurUserInfo.CurDef)
        //{
        //    player_defense.color = Color.green;
        //}
        //else if (UserManager.CurUserInfo.CurDef < UserManager.CurUserInfo.CurDef)
        //{
        //    player_defense.color = Color.red;
        //}
        //else
        //{
        //    player_defense.color = Color.white;
        //}
        player_defense.text = UserManager.CurUserInfo.CurDef.ToString();

        player_cost.text = UserManager.CurUserInfo.CurHero.Hcost.ToString();

        player_warfare.text = UserManager.CurUserInfo.CurWarfare.ToString();

        if (UserManager.CurUserInfo.CurWeapon != null)
        {
            current_player.AddAvataWare(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
        }
        if (UserManager.CurUserInfo.CurHelmet != null)
        {
            current_player.AddAvataWare(UserManager.CurUserInfo.CurHelmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        if (UserManager.CurUserInfo.CurArmor != null)
        {
            current_player.AddAvataWare(UserManager.CurUserInfo.CurArmor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        current_player.WeaponEffectShow();

        if (ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkillAffix1) != null)
        {
            Skill_1.SetActive(true);
            SkillData skill = ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkillAffix1);
            Skill_Name.text = skill.Name;
            Skill_Description.text = skill.Description;
            Skill_Map.spriteName = skill.SkillIcon;
            Skill_Chain.text = (skill.SkillPower / 10).ToString();
            if (skill.SkillCd > 0) Skill_Cd.text = skill.SkillCd.ToString();
            else Skill_Cd.text = "--";
        }
        else
        {
            Skill_1.SetActive(false);
        }

        if (ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkillAffix2) != null)
        {
            Skill_2.SetActive(true);
            SkillData skill = ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkillAffix2);
            Skill_Name_2.text = skill.Name;
            Skill_Description_2.text = skill.Description;
            Skill_Map_2.spriteName = skill.SkillIcon;
            Skill_Chain_2.text = (skill.SkillPower / 10).ToString();
            if (skill.SkillCd > 0) Skill_Cd_2.text = skill.SkillCd.ToString();
            else Skill_Cd_2.text = "--";
        }
        else
        {
            Skill_2.SetActive(false);
        }

        if(UserManager.CurUserInfo.CurHelmet == null || UserManager.CurUserInfo.CurArmor == null)
        {
            Skill_3.SetActive(false);
        }
        else if(ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurHelmet.CurHardWareData.SkillAffix1) != null && ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurArmor.CurHardWareData.SkillAffix1) != null)
        {
            Skill_3.SetActive(true);
            SkillData skill_helmet = ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurHelmet.CurHardWareData.SkillAffix1);
            SkillData skill_armor = ConfigManager.SkillConfig.GetSkillById(UserManager.CurUserInfo.CurArmor.CurHardWareData.SkillAffix1);
            if(skill_armor.SuitSkillHardwareIds.Contains(UserManager.CurUserInfo.CurHelmet.CurHardWareData.Id) && skill_helmet.SuitSkillHardwareIds.Contains(UserManager.CurUserInfo.CurArmor.CurHardWareData.Id))
            {
                Skill_Name_3.text = skill_helmet.Name;
                Skill_Description_3.text = skill_helmet.Description;
                Skill_Map_3.spriteName = skill_helmet.SkillIcon;
            }
            else
            {
                Skill_3.SetActive(false);
            }
        }
        else
        {
            Skill_3.SetActive(false);
        }
    }

    void OnEnable()
    {
        UnitCamera.SetActive(false);
    }

    void OnDisable()
    {
        if (!UnitCamera.activeSelf)
            UnitCamera.SetActive(true);
        current_player.ClearAvata();
    }
}
