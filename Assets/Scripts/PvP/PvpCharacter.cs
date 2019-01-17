using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class PvpCharacter : PvpOwnUnit
{
    #region 属性
    public PlayerArrow Arrow;
    
    ///当前关卡 剩余回合数    
    public int CurBoutNu;

	public List<SkillData> PetSkillList;
	
	public GameObject Element_Enemy;

	//public GameObject FaceObject;
	public List<string> faceList;

	#endregion
	
	public void ShowElement()
	{
		this.Element_Enemy.SetActive (true);
		/*Element = DungeonEnum.ElementAttributes.Water;
		SpriteRenderer sr= Element_Enemy.transform.GetComponent<SpriteRenderer>();
		sr.sprite = Resources.Load<Sprite>("Atlas/Fight/pveNewCell/" + (int)Element);*/
	}

    #region 重写

	/// <summary>
	/// 显示表情
	/// </summary>
	/// <param name="faceName">Face name.</param>
	public void ShowFace(string faceName)
	{
		PvpFaceManager.ShowFace (this, faceName);
	}

    public override void TryAttack(Action attackEnd)
    {
		//Debug.Log ("PvpCharacter 开始攻击 --------------------------------------->");
        this.curFightUnits = FindEnemies();
        if (this.PvpUserInfo.CurWeapon != null)
        {
			if (this.PvpUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far1 ||
			    this.PvpUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far2)
            {
                attackEnd();
            }
            else
            {
                base.TryAttack(attackEnd);
            }
        }
        else
        {
            base.TryAttack(attackEnd);
        }
    }

	/// <summary>
	/// 反击
	/// </summary>
	/// <param name="sourceItem">Source item.</param>
	public void StrickeAttack(PvpFightUnit sourceItem, Action<string> strikeCallback)
	{
		Debug.Log ("Strike ----------> here !!!!" + strikeCallback);
		// 反击被击状态
		sourceItem.StrickeHurtStatus = true;
		// 反击为真
		this.StrickeStatus = true;
		this.StrickeFightUnit = sourceItem;
		this.StrikeCallback = strikeCallback;

		this.curFightUnits = new List<PvpFightUnit> ();
		this.curFightUnits.Add (sourceItem);

		DungeonEnum.FaceDirection direction =  GetTargetDirection(curFightUnits[0]);

		this.UnitAttack(direction);
	}

    public override void SetName()
    {
        name = "Character:" + XPosition + "," + YPosition;
    }
    public override void UnitVertigo(DungeonEnum.FaceDirection direction)
    {
        ChangeAnimation(direction, ActionState.Vertigo);
    }
	public override float getAtk()
	{
		return this.PvpUserInfo.CurAtk;
	}

    public override void BeHurt(PvpFightUnit fu, int index = -1)
	{
		//Debug.Log ("PvpCharacter 开始计算伤害 --------------------------------------->");
		if(fu.GetType() != typeof(PvpPet))
		{
			if(fu.GetType() == typeof(PvpCharacter))
			{
				Debug.Log("Stricke Status ::: -> " + this.StrickeStatus + ":" + this.PvpUserInfo.UserId);
			}
			// 如果自己不是反击，并且自己也不是被击，并且不是眩晕动作
			if(!this.StrickeStatus && !this.StrickeHurtStatus && this.CurActionState != ActionState.Vertigo) this.ChangeAnimation(CurFaceDirection, ActionState.Hurt);
		}
		// 如果攻击者是角色
		if(fu.GetType() == typeof(PvpCharacter))
		{
			this.avoidStatus = false;
		}

		bool critBool = false;
		bool missBool = false;

		float lastDamage = this.BeHurtValue (fu, index, ref critBool, ref missBool);

		//如果是暴击
		if(critBool) 
		{          
			CameraControl.ShakeCamera();
		}
		// 如果闪避 并且 未暴击
		if (missBool && !critBool) 
		{      
			// 如果攻击者是角色
			if(fu.GetType() == typeof(PvpCharacter))
			{
				// 设置闪避状态
				this.avoidStatus = true;
			}

			ShowMissEff();
		}else
		{
			HurtAnimation();
		}
        CurHp = CurHp - lastDamage;

		Debug.Log ("当前步骤结束最终的剩余血量为：" + CurHp);

		HurtLabelShow(Math.Abs(lastDamage), fu, DungeonEnum.ElementAttributes.None, critBool, missBool);

		SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(fu.SkinData.Id);
		string hitSound = skinData.FireFXName;
		if (missBool) hitSound = "FX7";
		if (critBool) hitSound = "FX6";

		PvpGameObjectManager.Create("PreFabs/FX/" + hitSound, (GameObject beHitObject)=>
		{
			beHitObject.transform.parent = transform;
			beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
		});
        
		this.RefreshHp ();
	}
	
	GameObject MissObje;
	/// <summary>
	/// 闪避效果
	/// </summary>
	void ShowMissEff()
	{
		
		Debug.Log("XX " + MissObje);
		if (MissObje != null) Destroy(MissObje);
		
		MissObje = new GameObject();
		SpriteRenderer newspriteMiss = MissObje.AddComponent<SpriteRenderer>();
		newspriteMiss.sprite = Resources.Load<Sprite>("Atlas/Fight/miss");
		MissObje.transform.parent = transform;
		MissObje.layer = gameObject.layer;
		MissObje.transform.localPosition = new Vector3(0f, 0.23f, 0f);
		
		MissObje.transform.localScale = new Vector3(0.001760563f, 0.001760563f, 1f);
		newspriteMiss.sortingOrder = 220;
		AnimationHelper.AnimationFadeTo(0, MissObje, iTween.EaseType.easeOutQuad, gameObject, "MissObjeDestroy", 2f);
		
		Vector3 v3 = MissObje.transform.localPosition + new Vector3(0f, 0.2f, 0f);
		AnimationHelper.AnimationMoveTo(v3, MissObje, iTween.EaseType.easeOutQuad, null, null, 2f);
	}
	/// <summary>
	/// 销毁闪避效果
	/// </summary>
	void MissObjeDestroy()
	{
		if (MissObje != null) Destroy(MissObje);
	}


    //回合为0，每次行动扣除体力 jc
    public void BeHurt_MapRoundDotHp(float los)
    {
        CurHp = CurHp - los;              
        HurtAnimation();

		this.RefreshHp ();
        //CurUnitHp.RefreshUI(CurHp, Hp);
        //GameControl.SetHpUIHpShow(this);
    }
    #endregion

    #region 方法
    public void ShowArrow(bool show)
    {
        Arrow.gameObject.SetActive(show);
    }

	/// <summary>
	/// 设置回合数
	/// </summary>
	/// <param name="current">Current.</param>
	/// <param name="maxSize">Max size.</param>
	public void SetRoundsData(int current, int maxSize)
	{
		this.CurBoutNu = current;
		if(current >= maxSize)
		{
			BeHurt_MapRoundDotHp((int)(Hp * ConfigManager.ParamConfig.GetParam().MapRoundDotHp));  
		}
		GameControl.SetBoutLabNum(maxSize - current);
	}

    #endregion
    
    public void HpRecoverRender(float hpRecover)
    {
        CurHp = CurHp + hpRecover;
        if (CurHp > Hp)
        {
            CurHp = Hp;
        }
		PvpGameObjectManager.Create("", (GameObject hurtObject)=>
		{
			//GameObject beHurtResource = Resources.Load("PreFabs/Fight/DamageLable") as GameObject;
			//GameObject hurtObject = Instantiate(beHurtResource) as GameObject;
			hurtObject.transform.parent = transform.parent;
			//随机位置
			hurtObject.transform.localScale = new Vector3(1, 1, 1);
			int offsetX = UnityEngine.Random.Range(-30, 31);
			int offsetY = UnityEngine.Random.Range(-30, 31);
			hurtObject.transform.localPosition = new Vector3(transform.localPosition.x + offsetX, transform.localPosition.y + offsetY, transform.localPosition.z);
			FightDamage fd = hurtObject.GetComponent<FightDamage>();
			fd.DamageShow(((int)hpRecover).ToString(), DungeonEnum.ElementAttributes.Earth);
			this.RefreshHp();
			//CurUnitHp.RefreshUI(CurHp, Hp);
			//GameControl.SetHpUIHpShow(this);
		});
    }


    public  void BeHitBack(float backCount, DungeonEnum.FaceDirection backDirection)
    {
        GameControl.CurCharacterEliminate.AttrubuteToRender();
       
        BeHitBackRender(backCount, backDirection);
        PvpEliminate eb = GameControl.FineEliminate(XPosition, YPosition);
        eb.SetToPlayerRender(this.HiddenAlpha);
    }

	
}
