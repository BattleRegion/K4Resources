using UnityEngine;
using System.Collections;

public class PetAvata : MonoBehaviour
{
	private static readonly int PVE = 0;
	private static readonly int PVP_SELF = 1;
	private static readonly int PVP_ENEMY = 2;
    /// <summary>
    /// 框的精灵
    /// </summary>
    public SpriteRenderer FrameRender;

    /// <summary>
    /// 头像精灵
    /// </summary>
    public SpriteRenderer AvataRender;

    /// <summary>
    /// 当前怪物
    /// </summary>
    public UserPet CurPet;

    /// <summary>
    /// 框的所有精灵列表
    /// </summary>
    public Sprite[] FrameSprites;

	public Sprite[] AttributeSprites;

    /// <summary>
    /// 是否已经焦点
    /// </summary>
    public bool hasFocus = false;

    /// <summary>
    /// 头像框白底粒子
    /// </summary>
    public GameObject UIFlashObject;

    /// <summary>
    /// 头像闪烁粒子资源
    /// </summary>
    public GameObject[] AvataShineFX;

    /// <summary>
    /// 能量条
    /// </summary>
    public GameObject Progress;

	public UISprite cdItem;

	public SpriteRenderer AttributeRender;

	public InfoLabel powerInfo;

	/// <summary>
	/// 宠物类型
    ///0 pve ; 1 pvp top  ; 2  pvp bottom
	/// </summary>
	public int PetType;

    #region  重写MONO
    void Awake()
    {
        UIEventListener.Get(gameObject).onClick = (go) =>
        {
			// 如果是 PVE
			if(PetType == PetAvata.PVE)
			{
	            PveGameControl gc = GameObject.Find("UI Root").GetComponent<PveGameControl>();
	            PvePet p = gc.FindPet(CurPet);
                p.TryPopSkillDetail(CurPet);
            }
			else if(PetType == PetAvata.PVP_SELF)
            { // 如果是 PVP 自己
				PvpGameControl gc = GameObject.Find("UI Root").GetComponent<PvpGameControl>();
				PvpPet p = gc.FindPet(CurPet);
				p.TryPopSkillDetail(CurPet);
			}else if(PetType == PetAvata.PVP_ENEMY)
			{
				// 如果是 PVP 敌人
				PvpGameControl gc = GameObject.Find("UI Root").GetComponent<PvpGameControl>();
				if(gc.EnemyPetDetail != null)
				{
					gc.EnemyPetDetail.SetDetail(CurPet, gc.PvpUserInfoEnemy, gc);
				}
			}
        };
    }
    #endregion

    #region 公用方法
    /// <summary>
    /// 设置头像
    /// </summary>
    /// <param name="pet"></param>
    public void SetUserPet(UserPet pet)
    {
        CurPet = pet;
		FrameRender.sprite = FrameSprites[pet.CurPetData.Rank - 1];

		this.AttributeRender.sprite = this.AttributeSprites[(int)CurPet.CurPetData.PetPro - 1];

        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(pet.CurPetData.SkinId);
        AvataRender.sprite = Resources.Load<Sprite>(Tools.GetIconPath(CurPet.CurPetData.Id));
        gameObject.SetActive(true);
		FrameRender.transform.localScale = new Vector3(0.95f, 0.95f, 1);

        //AddEmeSprite();
    }
    void AddEmeSprite()
    {
        //添加属性icon
        GameObject ga=NGUITools.AddChild(gameObject, new GameObject());
        SpriteRenderer sr = ga.AddComponent<SpriteRenderer>();
        sr.transform.localPosition = new Vector3(-31,31,0);
        sr.transform.localScale = new Vector3(0.7f,0.7f,1);
        sr.sprite = Resources.Load<Sprite>("Atlas/Fight/pveNewCell/" + (int)CurPet.CurPetData.PetPro);
        sr.sortingOrder = 8; 
    }

	public void SetPowerData()
	{
		if(this.powerInfo == null) return;

        if (this.PetType == PetAvata.PVE)
        {
            //pve
            PveGameControl gc = GameObject.Find("UI Root").GetComponent<PveGameControl>();
            PvePet p = gc.FindPet(CurPet);
            if (p != null && p.MainSkill != null)
            {
				if(p.MainSkill.SkillPower > 0)
				{
					this.powerInfo.SetNum(((int)(p.MainSkill.SkillPower / 10f)).ToString());
				}
				else
				{
					this.powerInfo.SetNum("");
				}
            }
            else
            {
                this.powerInfo.SetNum("");
            }
        }
        else
        {
            //pvp
            PvpGameControl gc = GameObject.Find("UI Root").GetComponent<PvpGameControl>();
            PvpPet p = gc.FindPet(CurPet);
            if (p != null && p.MainSkill != null)
            {
				if(p.MainSkill.SkillPower > 0)
				{
                	this.powerInfo.SetNum(((int)(p.MainSkill.SkillPower / 10f)).ToString());
				}else{
					this.powerInfo.SetNum("");
				}
            }
            else
            {
                this.powerInfo.SetNum("");
            }
        }

		// 这儿写死
		this.powerInfo.transform.localPosition = new Vector3 (-32.5f, 31f, 0f);
	}

	/// <summary>
	/// 设置 CD
	/// </summary>
	/// <param name="cd">Cd.</param>
	public void RefreshCd(PvpFightUnit pvpFightUnit)
	{
		// 如果不是 PVP
		if(this.PetType == PetAvata.PVE) return;

		PvpGameControl gc = GameObject.Find("UI Root").GetComponent<PvpGameControl>();
		PvpPet p = gc.FindPet(CurPet);
		if(p != null && p.MainSkill != null)
		{
			PvpSkillCdData pvpSkillCdData = pvpFightUnit.pvpPlayerSkill.GetSkillCdDataBySkillID (p.MainSkill.Id, p.CurUserPet.UserPetId);
			if(pvpSkillCdData != null)
			{
				int skillCd = p.MainSkill.SkillCd;
				if(skillCd == 0) skillCd = 1;

				if(this.cdItem != null)
				{
					this.cdItem.gameObject.SetActive(true);
					this.cdItem.fillAmount = (float)pvpSkillCdData.cd / (float)skillCd;
				}
			}else
			{
				if(this.cdItem != null)
				{
					this.cdItem.gameObject.SetActive(false);
				}
			}
		}else
		{
			if(this.cdItem != null)
			{
				this.cdItem.fillAmount = 0f;
				this.cdItem.gameObject.SetActive(false);
			}
		}
	}
    /// <summary>
    ///Pve  设置 CD
    /// </summary>
    /// <param name="cd">Cd.</param>
    public void PveRefreshCd(PveFightUnit pveFightUnit)
    {      
        // 设置 UI 层
        //cdItem.gameObject.layer = LayerHelper.UI;

        PveGameControl gc = GameObject.Find("UI Root").GetComponent<PveGameControl>();
        PvePet p = gc.FindPet(CurPet);
        if (p != null && p.MainSkill != null)
        {
            PvpSkillCdData pvpSkillCdData = pveFightUnit.pvpPlayerSkill.GetSkillCdDataBySkillID(p.MainSkill.Id, p.CurUserPet.UserPetId);
            if (pvpSkillCdData != null)
            {
                int skillCd = p.MainSkill.SkillCd;
                if (skillCd == 0) skillCd = 1;


                if (this.cdItem != null) {
					cdItem.gameObject.SetActive(true);
					this.cdItem.fillAmount = (float)pvpSkillCdData.cd / (float)skillCd;
				}
                return;
            }
        }
        else
        {
            if (this.cdItem != null) this.cdItem.fillAmount = 0f;
        }
    }

    /// <summary>
    /// 头像焦点动画渲染
    /// </summary>
    /// <param name="focus"></param>
    public void AvatarFocus(bool focus)
    {
        if (hasFocus != focus)
        {
            hasFocus = focus;
            Vector3 v;
            iTween.EaseType ease;
            if (focus)
            {
                v = new Vector3(transform.localPosition.x, -11, transform.localPosition.z);
                ease = iTween.EaseType.easeOutExpo;
            }
            else
            {
                v = new Vector3(transform.localPosition.x, -26, transform.localPosition.z);
                ease = iTween.EaseType.easeInExpo;
            }
            AnimationHelper.AnimationMoveTo(v, gameObject, ease, null, null, 0.2f);
        }
    }
    void FocusEndCallback()
    {
        FrameRender.transform.localScale = new Vector3(0.85f, 0.85f, 1);
    }

    /// <summary>
    /// 头像特效渲染
    /// </summary>
    /// <param name="render"></param>
    GameObject curShineObject;
    public void AvataEffectiveRender(bool render)
    {
        UIFlashObject.SetActive(render);
        if (!render)
        {
            Destroy(curShineObject);
        }
        else
        {
            if (!curShineObject)
            {
                curShineObject = Instantiate(GetAvataShineFX()) as GameObject;
                curShineObject.transform.parent = transform;
                curShineObject.transform.localPosition = new Vector3(-30, 28, 0);
            }
        }
    }

    //能量进度条
    public void AvataProgress(float _pro)
    {      
        Progress.renderer.material.SetFloat("_Progress", _pro);
    }

    /// <summary>
    /// 获取闪烁粒子资源
    /// </summary>
    /// <returns></returns>
    GameObject GetAvataShineFX()
    {
        if (CurPet.CurPetData.TempPetPro  ==  DungeonEnum.ElementAttributes.Earth)
        {
            return AvataShineFX[1];
        }
        else if (CurPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Fire)
        {
            return AvataShineFX[2];
        }
        else if (CurPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Wind)
        {
            return AvataShineFX[3];
        }
        else if (CurPet.CurPetData.TempPetPro == DungeonEnum.ElementAttributes.Water)
        {
            return AvataShineFX[0];
        }
        return null;
    }
    #endregion
}
