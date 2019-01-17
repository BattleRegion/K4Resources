using UnityEngine;
using System.Collections;
using System;

public class PveCharacter : PveOwnUnit
{
    #region 属性
    public PlayerArrow Arrow;
    /// <summary>
    ///当前关卡 剩余回合数
    /// </summary>
    public int CurBoutNu;
    

    #endregion

    #region 重写
    public override void TryAttack(Action attackEnd)
    {
        curEnemies = FindEnemies();
        if (UserManager.CurUserInfo.CurWeapon != null)
        {
            if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far1 ||
    UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Far2)
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

    public override void SetName()
    {
        name = "Character:" + XPosition + "," + YPosition;
    }


    public override void UnitVertigo(DungeonEnum.FaceDirection direction)
    {
        ChangeAnimation(direction, ActionState.Vertigo);
    }

    public override void SetVertigo(int round)
    {
        base.SetVertigo(round);
        UnitVertigo(CurFaceDirection);
    }


    /// <summary>
    /// 反击
    /// </summary>
    /// <param name="sourceItem">Source item.</param>
    public void StrickeAttack(PveFightUnit sourceItem, Action<string> strikeCallback)
    {
        // 反击被击状态
        sourceItem.StrickeHurtStatus = true;
        // 反击为真
        this.StrickeStatus = true;
        //this.StrickeFightUnit = sourceItem;
        //this.StrikeCallback = strikeCallback;

        //this.curFightUnits = new List<PveFightUnit>();
        //this.curFightUnits.Add(sourceItem);

        //DungeonEnum.FaceDirection direction = GetTargetDirection(curFightUnits[0]);

        //UnitAttack(direction);
    }
    public bool BeTrap = false;
    public void BeTrapHurt(PveTrap t)
    {
        ChangeAnimation(CurFaceDirection, ActionState.Hurt);
        BeTrap = true;
        float lastDamage = t.DamagePersent / 10000.0f * Hp * GetRandomHurtPer();
        CurHp = CurHp - lastDamage;
        t.TrapAnimation();
        HurtLabelShow(lastDamage, null);
        //SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(fu.SkinData.Id);
        //GameObject g = Resources.Load("PreFabs/FX/" + skinData.FireFXName) as GameObject;
        //if (g)
        //{
        //    GameObject beHitObject = Instantiate(g) as GameObject;
        //    beHitObject.transform.parent = transform;
        //    beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
        //}
        HurtAnimation();
        CurUnitHp.RefreshUI(CurHp, Hp);
        //GameControl.PvePlayerInfo.HpUI.SetCurHpShow(CurHp, Hp);
        GameControl.SetHpUIHpShow();
    }
	public override float getAtk()
	{
		return UserManager.pveUserInfo.CurAtk;
	}
    public  float getDef()
    {
        return UserManager.pveUserInfo.CurDef;
    }
    //Normal 
    public virtual float BeHurt(PveFightUnit fu)
    {
        
        int fp = ActP(fu);
        int sp = StP();
        float sat = fu.getAtk() ;
        float pr = GetElementParam(fp, sp);
        float lastDamage = (sat - getDef()) * pr;
        if (lastDamage <= 0)
        {
            lastDamage = 1;
        }
        bool missBool = false;

        //闪躲
        float miss = Evade;

        double jg1 = Tools.GetRandom();
        if (jg1 < miss)
        {
            //CameraControl.ShakeCamera();
            lastDamage = 0;
            missBool = true;
            ShowMissEff();
        }
        //Debug.Log(" character    BeHurt = " + lastDamage);
        CurHp = CurHp - lastDamage;
        HurtLabelShow(lastDamage, fu, false, missBool,pr);
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(fu.SkinData.Id);
        string hitSound = skinData.FireFXName;

        if (missBool) {
            hitSound = "FX7";
        }else{
            ChangeAnimation(CurFaceDirection, ActionState.Hurt);
            HurtAnimation();
        };
        GameObject g = Resources.Load("PreFabs/FX/" + hitSound) as GameObject;
        
        if (g)
        {
            GameObject beHitObject = Instantiate(g) as GameObject;
            beHitObject.transform.parent = transform;
            beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
        }
        
        
        //击退
        if (fu.GetType() == typeof(PveBoss))
        {
            PveBoss b = (PveBoss)fu;
            DungeonEnum.FaceDirection direction = b.GetTargetDirection(this);
           // BeHitBack(b.FightOff, direction);
        }


        CurUnitHp.RefreshUI(CurHp, Hp);
        //GameControl.PvePlayerInfo.HpUI.SetCurHpShow(CurHp, Hp);
        GameControl.SetHpUIHpShow();
        return lastDamage;
    }
    //Boss skill use
    public float BeHurt(float damage, PveFightUnit fu)
    {
        ChangeAnimation(CurFaceDirection, ActionState.Hurt);

        int fp = ActP(fu);
        int sp = StP();
        float sat = damage;
        float lastDamage = (sat - getDef()) * GetElementParam(fp, sp);
        if (lastDamage <= 0)
        {
            lastDamage = 1;
        }
        
        CurHp = CurHp - lastDamage;
        HurtLabelShow(lastDamage, fu, false, false);
        if(fu.SkinData != null)
        {
            SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(fu.SkinData.Id);
            string hitSound = skinData.FireFXName;

            GameObject g = Resources.Load("PreFabs/FX/" + hitSound) as GameObject;
            if (g)
            {
                GameObject beHitObject = Instantiate(g) as GameObject;
                beHitObject.transform.parent = transform;
                beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
            }

        }
        else
        {
            string hitSound = "FX8";
            GameObject g = Resources.Load("PreFabs/FX/" + hitSound) as GameObject;

            if (g)
            {
                GameObject beHitObject = Instantiate(g) as GameObject;
                beHitObject.transform.parent = transform;
                beHitObject.transform.localPosition = new Vector3(0, 0.05f, 0);
            }
        }
        
        //击退
        if (fu.GetType() == typeof(PveBoss))
        {
            PveBoss b = (PveBoss)fu;
            DungeonEnum.FaceDirection direction = b.GetTargetDirection(this);
           // BeHitBack(b.FightOff, direction);
        }


        CurUnitHp.RefreshUI(CurHp, Hp);
        //GameControl.PvePlayerInfo.HpUI.SetCurHpShow(CurHp, Hp);
        GameControl.SetHpUIHpShow();
        return lastDamage;
    }


    //回合为0，每次行动扣除体力 
    public void BeHurt_MapRoundDotHp(float los)
    {
        CurHp = CurHp - los;         
        HurtAnimation();
        CurUnitHp.RefreshUI(CurHp, Hp);
        //GameControl.PvePlayerInfo.HpUI.SetCurHpShow(CurHp, Hp);
        GameControl.SetHpUIHpShow();
    }
    #endregion

    #region 方法
    public void ShowArrow(bool show)
    {
        Arrow.gameObject.SetActive(show);
    }
   
    //设置回合数
    public int SetBoutNu(int n)
    { 
        CurBoutNu=CurBoutNu+n;
        if(CurBoutNu<0){
         CurBoutNu=0;
         BeHurt_MapRoundDotHp(Hp * ConfigManager.ParamConfig.GetParam().MapRoundDotHp);  
		}
		// 触发成就
		AchievementManager.Trigger (AchievementTypeEnum.Residue_Round, CurBoutNu);

        GameControl.PvePlayerInfo.setBoutLabNum(CurBoutNu);
        return CurBoutNu;
    }
    //初始回合数
    public void InitBoutNu(int n)
    {
        CurBoutNu = n;
        GameControl.PvePlayerInfo.setBoutLabNum(CurBoutNu);
    }
    

    #endregion

    public void HpRecoverRender(float hpRecover)
    {
        CurHp = CurHp + hpRecover;
        if (CurHp > Hp)
        {
            CurHp = Hp;
        }
        GameObject beHurtResource = Resources.Load("PreFabs/Fight/DamageLable") as GameObject;
        GameObject hurtObject = Instantiate(beHurtResource) as GameObject;
        hurtObject.transform.parent = transform.parent;
        //随机位置
        hurtObject.transform.localScale = new Vector3(1, 1, 1);
        int offsetX = UnityEngine.Random.Range(-30, 31);
        int offsetY = UnityEngine.Random.Range(-30, 31);
        hurtObject.transform.localPosition = new Vector3(transform.localPosition.x + offsetX, transform.localPosition.y + offsetY, transform.localPosition.z);
        FightDamage fd = hurtObject.GetComponent<FightDamage>();
        fd.DamageShow(((int)hpRecover).ToString(), DungeonEnum.ElementAttributes.Earth);
        CurUnitHp.RefreshUI(CurHp, Hp);
        //GameControl.PvePlayerInfo.HpUI.SetCurHpShow(CurHp, Hp);
        GameControl.SetHpUIHpShow();
    }


    public  void BeHitBack(float backCount, DungeonEnum.FaceDirection backDirection)
    {
        GameControl.CurCharacterEliminate.AttrubuteToRender();
        //Debug.Log("backCount=" + backCount);
       // Debug.Log("backDirection=" + backDirection);

        BeHitBackRender(backCount, backDirection);
        PveEliminate eb = GameControl.FineEliminate(XPosition, YPosition);
        eb.SetToPlayerRender();
    }
}
