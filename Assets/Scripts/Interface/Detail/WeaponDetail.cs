using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponDetail : MonoBehaviour
{
    /// <summary>
    /// 查看详情时关闭unit层摄像机
    /// </summary>
    public GameObject UnitCamera;
    public PlayerAvata PlayerAnime;

    public UILabel WeaponType;
    public UILabel Atk;
    public UILabel Name;
    public UILabel Level;
    public UISprite ElementType;
    public AlphaMaskBar Exp;
    public UILabel Count;
    public UILabel Price;
    public UIGrid stars;

    public GameObject StarPrefab;
    public GameObject StarOutline;

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

    public ShowNewObject NewObjectController;
    public GameObject ShowNewButton;

    UserWare CurWeapon;

    List<GameObject> TempStars = new List<GameObject>();
    List<Transform> StarOutlineTransforms = new List<Transform>();

    public void SetDetail(int UserWareId)
    {

        UserWare u = UserManager.CurUserInfo.FindUserWare(UserWareId);
        SetDetail(u);
    }

    public void SetDetail(UserWare u)
    {
        UnitCamera.SetActive(false);

        CurWeapon = u;

        PlayerAnime.AddAvataWare(u.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
        switch (u.CurHardWareData.Style)
        {
            case HardWareData.HardWareType.Light: WeaponType.text = "轻武器"; break;
            case HardWareData.HardWareType.Heavy: WeaponType.text = "重武器"; break;
            case HardWareData.HardWareType.Far1: WeaponType.text = "远程武器"; break;
            case HardWareData.HardWareType.Far2: WeaponType.text = "远程武器"; break;
            default: WeaponType.text = ""; break;
        }
        Atk.text = u.CurAtk.ToString();
        Name.text = u.CurHardWareData.Name;
        Level.text = "Lv.[4FFE27]" + u.Level + "[FFFFFF]/" + u.CurHardWareData.LvlMax;
        ElementType.spriteName = Tools.GetHardwareElement(u.CurHardWareData.Element);
        if(u.Level == u.CurHardWareData.LvlMax)
        {
            Exp.value = 1f;
        }
        else
        {
            Exp.value = (float)u.CurExp / (float)u.LevelExp;
        }

        int starCount = u.CurHardWareData.MaxRank;
        while (starCount > 0)
        {
            GameObject go = NGUITools.AddChild(stars.gameObject, StarOutline);
            StarOutlineTransforms.Add(go.transform);
            starCount--;
        }
        stars.Reposition();
        StartCoroutine(AddStars(0.2f, u.CurHardWareData.Rank));

        int WeaponCount = 0;
        foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
        {
            if (uw.CurHardWareData.Id == u.CurHardWareData.Id)
            {
                WeaponCount++;
            }
        }
        Count.text = WeaponCount.ToString();
        Price.text = u.CurHardWareData.Price.ToString();

        if (ConfigManager.SkillConfig.GetSkillById(u.CurHardWareData.SkillAffix1) != null)
        {
            Skill_1.SetActive(true);
            SkillData skill = ConfigManager.SkillConfig.GetSkillById(u.CurHardWareData.SkillAffix1);
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

        if (ConfigManager.SkillConfig.GetSkillById(u.CurHardWareData.SkillAffix2) != null)
        {
            Skill_2.SetActive(true);
            SkillData skill = ConfigManager.SkillConfig.GetSkillById(u.CurHardWareData.SkillAffix2);
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
    }

    bool newStart = false;
    IEnumerator AddStars(float time, int rank)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < rank; i++)
        {
            StartCoroutine(AddStarAnimation(i));
        }
        Invoke("SetNewStart", rank * 0.2f);
    }

    IEnumerator AddStarAnimation(int index)
    {
        yield return new WaitForSeconds(index * 0.2f);
        GameObject g = NGUITools.AddChild(StarOutlineTransforms[index].gameObject, StarPrefab);
        TempStars.Add(g);
        g.transform.localPosition = new Vector3(-0.5f, 0.5f, 0f);
        g.transform.localScale *= 3f;
        AnimationHelper.AnimationScaleTo(Vector3.one, g, iTween.EaseType.linear, null, null, 0.2f);
    }

    void SetNewStart()
    {
        newStart = true;
    }

    void OnEnable()
    {
        UnitCamera.SetActive(false);
        PetUIController.SetLayer(PlayerAnime.transform, LayerHelper.Top);

        UIEventListener.Get(ShowNewButton).onClick = (g) =>
        {
            NewObjectController.ShowNew(0, CurWeapon.CurHardWareData.Id);
        };
    }

    void OnDisable()
    {
        if (!UnitCamera.activeSelf)
            UnitCamera.SetActive(true);
        PlayerAnime.ClearAvata();

        foreach (Transform t in stars.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (GameObject g in TempStars)
        {
            Destroy(g);
        }
        TempStars.Clear();
        StarOutlineTransforms.Clear();
    }
}
