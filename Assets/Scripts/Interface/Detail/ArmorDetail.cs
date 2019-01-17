using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmorDetail : MonoBehaviour
{
    /// <summary>
    /// 查看详情时关闭unit层摄像机
    /// </summary>
    public GameObject UnitCamera;
    public PlayerAvata PlayerAnime;

    public UILabel ArmorType;
    public UILabel Def;
    public UILabel Name;
    public UILabel Level;
    public UISprite ElementType;
    public AlphaMaskBar Exp;
    public UILabel Count;
    public UILabel Price;
    public UIGrid stars;

    public GameObject StarPrefab;
    public GameObject StarOutline;

    public ShowNewObject NewObjectController;
    public GameObject ShowNewButton;

    public GameObject SkillObject;
    public UILabel SuitSkillName;
    public UILabel SuitSkillEffect;
    public UISprite SuitSkillIcon;

    public UILabel SuitName;
    public equipmentItemInterface Suit;

    UserWare CurArmor;

    List<GameObject> TempStars = new List<GameObject>();
    List<Transform> StarOutlineTransforms = new List<Transform>();

    public void SetDetail(int UserWareId)
    {
        UserWare u = UserManager.CurUserInfo.FindUserWare(UserWareId);

        SetDetail(u);
    }

    public void SetDetail(UserWare u)
    {
        CurArmor = u;

        PlayerAnime.AddAvataWare(u.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        switch (u.CurHardWareData.Style)
        {
            case HardWareData.HardWareType.Head: ArmorType.text = "头盔"; break;
            case HardWareData.HardWareType.Cuirass: ArmorType.text = "护甲"; break;
            default: ArmorType.text = ""; break;
        }
        Def.text = u.CurDef.ToString();
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

        int starCount = u.CurHardWareData.Rank;
        while (starCount > 0)
        {
            GameObject go = NGUITools.AddChild(stars.gameObject, StarOutline);
            StarOutlineTransforms.Add(go.transform);
            starCount--;
        }
        stars.Reposition();
        StartCoroutine(AddStars(0.2f, u.CurHardWareData.Rank));

        int ArmorCount = 0;
        foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
        {
            if (uw.CurHardWareData.Id == u.CurHardWareData.Id)
            {
                ArmorCount++;
            }
        }
        Count.text = ArmorCount.ToString();
        Price.text = u.CurHardWareData.Price.ToString();


        if(ConfigManager.SkillConfig.GetSkillById(u.CurHardWareData.SkillAffix1) != null)
        {
            SkillData sk = ConfigManager.SkillConfig.GetSkillById(u.CurHardWareData.SkillAffix1);
            SuitSkillName.text = sk.Name;
            SuitSkillIcon.spriteName = sk.SkillIcon;
            SuitSkillEffect.text = sk.Description;

            string suitId = null;
            foreach(string id in sk.SuitSkillHardwareIds)
            {
                if(id != u.CurHardWareData.Id)
                {
                    suitId = id;
                    break;
                }
            }
            UserWare sWare = new UserWare(suitId, 1);
            SuitName.text = sWare.CurHardWareData.Name;
            Suit.SetItem(sWare);

            SkillObject.SetActive(true);
        }
        else
        {
            SkillObject.SetActive(false);
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
            NewObjectController.ShowNew(0, CurArmor.CurHardWareData.Id);
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
