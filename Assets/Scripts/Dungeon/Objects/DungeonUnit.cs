using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DungeonUnit :DungeonObject
{
    #region 枚举
    public enum ActionState
    {
        None = 0,
        Waiting = 10,
        Run = 40,
        Attack = 30,
        Skill = 50,
        Hurt = 20,
        Dead = 70
    }
    #endregion

    #region 属性
    /// <summary>
    /// 当前行为
    /// </summary>
    public ActionState CurActionState;

    /// <summary>
    /// 默认起始等待方向
    /// </summary>
    public DungeonEnum.FaceDirection WaitingDefaultDirection = DungeonEnum.FaceDirection.Down;

    /// <summary>
    /// 帧率
    /// </summary>
    public float WaitingFrameRate;

    public float RunningFrameRate;

    public float AttackFrameRate;

    public float HurtFrameRate;

    public float DeadFrameRate;

    public float SkillFrameRate;
    /// <summary>
    /// 攻击碰撞帧
    /// </summary>
    public int FireFrame;

    /// <summary>
    /// 皮肤Id
    /// </summary>
    public string SkinId;

    //2d动画
    /// <summary>
    /// 当前所有的动画帧
    /// </summary>
    Sprite[] curAnimationSprites;

    /// <summary>
    /// 当前动画路劲
    /// </summary>
    string spriteAnimationPath;
    /// <summary>
    /// 当前帧数
    /// </summary>
    int curFrame = -1;

    /// <summary>
    /// 动画是否为循环播放
    /// </summary>
    bool animationForever = false;

    /// <summary>
    /// 动画结束的回调
    /// </summary>
    Action animationEndAction;

    /// <summary>
    /// 生命
    /// </summary>
    public UnitHp CurUnitHp;

    public float Hp;

    public float CurHp;

    /// <summary>
    /// 攻击力
    /// </summary>
    float atk;

    public float Atk
    {
        get 
        {
            if (GetType() == typeof(Player))
            {
                TileBlock t = DungeonScene.FindTile(XPosition, YPosition);
                return atk * t.CurAddition;
            }
            return atk;
        }
        set { atk = value; }
    }

    // 偏移属性
    public float WaitingOffsetX;
    public float WaitingOffsetY;

    public float AttackOffsetX;
    public float AttackOffsetY;

    public float RunOffsetX;
    public float RunOffsetY;

    public float DeadOffsetX;
    public float DeadOffsetY;

    #endregion

    #region 资源粒子
    public GameObject HurtFX;
    #endregion

    #region 重写MONO
    void Awake()
    {
        CurHp = 1;
    }
    #endregion

    #region 重写父类
    public override void SetZorder()
    {
        base.SetZorder();
        if (CurUnitHp)
        {
            CurUnitHp.frameSprite.sortingOrder = ObjectSprite.sortingOrder + 3;
            CurUnitHp.hpSprite.sortingOrder = ObjectSprite.sortingOrder + 2;
            CurUnitHp.backSprite.sortingOrder = ObjectSprite.sortingOrder + 1;
        }
    }
    /// <summary>
    /// 设置精灵
    /// </summary>
    public override void SetSprite()
    {
        UnitWaiting(WaitingDefaultDirection);
        if (CurUnitHp != null)
        {
            CurUnitHp.transform.localPosition = new Vector3(0, -34, 0);
        }
        base.SetSprite();
    }
    #endregion

    #region 单位行为
    public DungeonEnum.FaceDirection curWaitingDirection;
    public void UnitWaiting(DungeonEnum.FaceDirection direction)
    {
        curWaitingDirection = direction;
        ChangeState(ActionState.Waiting, direction, null, true);
    }

    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="direction"></param>
    /// <param name="stateAnimationEnd"></param>
    /// <param name="forever"></param>
    public void ChangeState(ActionState state, DungeonEnum.FaceDirection direction,Action stateAnimationEnd,bool forever)
    {
        CurActionState = state;
        spriteAnimationPath = DungeonSpritePathManager.UnitAnmationPath(SkinId, direction, CurActionState);
        PlayAnimation(spriteAnimationPath, forever, stateAnimationEnd);
        CorrectUnitPosition();
    }
    #endregion

    #region 自身的SET方法
    public virtual void SetSkin(SkinConfigData skinData)
    {
        SkinId = skinData.Id;
        WaitingFrameRate = skinData.WaitingFrameRate;
        AttackFrameRate = skinData.AttackFrameRate;
        RunningFrameRate = skinData.RunFrameRate;
        DeadFrameRate = skinData.DeadFrameRate;
        HurtFrameRate = skinData.HurtFrameRate;
        FireFrame = skinData.FireFrame;

        WaitingOffsetX = skinData.WaitingRate[0];
        WaitingOffsetY = skinData.WaitingRate[1];
        AttackOffsetX = skinData.AttackRate[0];
        AttackOffsetY = skinData.AttackRate[1];
        RunOffsetX = skinData.RunRate[0];
        RunOffsetY = skinData.RunRate[1];
        DeadOffsetX = skinData.DeadRate[0];
        DeadOffsetY = skinData.DeadRate[1];
    }

    /// <summary>
    /// 更具状态重设位置
    /// </summary>
    public void CorrectUnitPosition()
    {
        float offsetX = 0;
        float offsetY = 0;
        if (CurActionState == ActionState.Waiting)
        {
            offsetX = WaitingOffsetX;
            offsetY = WaitingOffsetY;
        }
        else if (CurActionState == ActionState.Attack)
        {
            offsetX = AttackOffsetX;
            offsetY = AttackOffsetY;
        }
        else if (CurActionState == ActionState.Dead)
        {
            offsetX = DeadOffsetX;
            offsetY = DeadOffsetY;
        }
        else if (CurActionState == ActionState.Run)
        {
            offsetX = RunOffsetX;
            offsetY = RunOffsetY;
        }

        //Debug.Log(this +":"+(ObjectSprite.sprite.rect.height - 82) / 2);

        ObjectSprite.transform.localPosition = new Vector3(offsetX,(ObjectSprite.sprite.rect.height - 82) / 2+offsetY,0);
    }
    #endregion

    #region 单位2D动画
    /// <summary>
    /// 播放动画
    /// </summary>
    void PlayAnimation(string spritePath, bool forever, Action animationEndAction)
    {
        StopAnimation();
        curFrame = -1;
        animationForever = forever;
        this.animationEndAction = animationEndAction;
        curAnimationSprites = Resources.LoadAll<Sprite>(spritePath);
        if (ObjectSprite != null)
        {
            ChangeCurAnimationFrame();
        }
        else
        {
            Debug.LogError("请绑定对象精灵属性");
        }
    }

    void ChangeCurAnimationFrame()
    {
        if (curAnimationSprites.Length == 0)
        {
            Debug.LogError(spriteAnimationPath + "帧数长度为0"); return;
        }
        curFrame++;
        ObjectSprite.sprite = curAnimationSprites[curFrame];
        if (curFrame == curAnimationSprites.Length - 1)
        {
            if (animationForever)
            {
                curFrame = -1;
            }
            else
            {
               Invoke("AnimationEnd", GetCurFrameInterval()); return;
            }
        }
        Invoke("ChangeCurAnimationFrame", GetCurFrameInterval());
    }

    /// <summary>
    /// 停止动画
    /// </summary>
    public void StopAnimation()
    {
        CancelInvoke("ChangeCurAnimationFrame");
        curFrame = -1;
        animationForever = false;
        animationEndAction = null;
    }

    /// <summary>
    ///  动画结束
    /// </summary>
    void AnimationEnd()
    {
        if (animationEndAction!=null)
        {
            animationEndAction();
        }
    }

    /// <summary>
    /// 获取帧率
    /// </summary>
    /// <returns></returns>
    float GetCurFrameInterval()
    {
        try
        {
            int index = curFrame;
            if (curFrame == -1)
            {
                index = curAnimationSprites.Length - 1;
            }
            if (CurActionState == ActionState.Attack)
            {
                return AttackFrameRate;
            }
            else if (CurActionState == ActionState.Run)
            {
                return RunningFrameRate;
            }
            else if (CurActionState == ActionState.Waiting)
            {
                return WaitingFrameRate;
            }
            else if (CurActionState == ActionState.Hurt)
            {
                return HurtFrameRate;
            }
            else if (CurActionState == ActionState.Dead)
            {
                return DeadFrameRate;
            }
        }
        catch
        {
            return 1;
        }
        return 1;
    }
    #endregion

    #region 虚方法
    /// <summary>
    /// 被打击
    /// </summary>
    public virtual void BeHurt(DungeonUnit hurtFrom,bool needDelay)
    {
        if (needDelay)
        {
            StartCoroutine(BeHurtAttackBump(hurtFrom));
        }
        else
        {
            HurtRender(hurtFrom);
        }
    }

    /// <summary>
    /// 攻击碰撞
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    IEnumerator BeHurtAttackBump(DungeonUnit hurtFrom)
    {
        float t = 0;
        for (int i = 0; i < hurtFrom.FireFrame - 1; i++)
        {
            t = t + hurtFrom.AttackFrameRate;
        }
        yield return new WaitForSeconds(t);
        HurtRender(hurtFrom);
    }

    /// <summary>
    /// 技能伤害渲染
    /// </summary>
    public void SkillHurtRender(SkillData skill)
    {
        if (CurHp > 0)
        {
            if (skill.Id == "Sk1" &&( GetType() == typeof(Monster) || GetType() == typeof(Boss)))
            {
                DungeonEnum.FaceDirection direction = DungeonScene.CurPlayer.GetTargetDirection(this);
                BeHitBackRender(skill.Cparameter, direction);
            }
            CurHp = CurHp - skill.Aparameter;
            HurtLabelShow(skill.Aparameter, null);
            HurtAnimation();
            CurUnitHp.RefreshUI(CurHp, Hp);
            if (CurHp <= 0)
            {
                ObjectHandler.ObjectDestoryFromDungeon(this);
            }
        }
    }

    /// <summary>
    /// 伤害渲染
    /// </summary>
    /// <param name="hurtFrom"></param>
     void HurtRender(DungeonUnit hurtFrom)
    {
        if (CurHp > 0)
        {
            //if (hurtFrom.GetType() == typeof(Boss))
            //{
            //    Boss b = (Boss)hurtFrom;
            //    DungeonEnum.FaceDirection direction = hurtFrom.GetTargetDirection(this);
            //    BeHitBackRender(b.FightOff, direction);
            //}
            CurHp = CurHp - hurtFrom.Atk;
            HurtLabelShow(hurtFrom.Atk, hurtFrom);
            SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(hurtFrom.SkinId);
            GameObject g = Resources.Load("PreFabs/FX/" + skinData.FireFXName) as GameObject;
            if (g)
            {
                GameObject beHitObject = Instantiate(g) as GameObject;
                beHitObject.transform.parent = transform;
                beHitObject.transform.localPosition = new Vector3(0, 0, 0);
            }
            HurtAnimation();
            CurUnitHp.RefreshUI(CurHp, Hp);
            if (CurHp <= 0)
            {
                ObjectHandler.ObjectDestoryFromDungeon(this);
            }
        }
    }

    /// <summary>
    /// 受伤动画
    /// </summary>
    public virtual void HurtAnimation()
    {
        iTween.PunchPosition(ObjectSprite.gameObject, new Vector3(0.1f, 0, 0), 0.5f);
    }

    /// <summary>
    /// 受伤跳血数字
    /// </summary>
    public void HurtLabelShow(float damage,DungeonUnit from)
    {
        GameObject beHurtResource = Resources.Load("PreFabs/Fight/DamageLable") as GameObject;
        GameObject hurtObject = Instantiate(beHurtResource) as GameObject;
        hurtObject.transform.parent = transform.parent;
        //随机位置
        hurtObject.transform.localScale = new Vector3(1, 1, 1);
        int offsetX = UnityEngine.Random.Range(-30, 31);
        int offsetY = UnityEngine.Random.Range(-30, 31);
        hurtObject.transform.localPosition = new Vector3(transform.localPosition.x + offsetX, transform.localPosition.y + offsetY, transform.localPosition.z);
        FightDamage fd = hurtObject.GetComponent<FightDamage>();
        DungeonEnum.ElementAttributes defaultAttibute = DungeonEnum.ElementAttributes.Fire;
        if (from)
        {
            if (from.GetType() == typeof(Pet))
            {
                Pet p = (Pet)from;
                defaultAttibute = p.UserPet.CurPetData.PetPro;
            }
        }
        fd.DamageShow(((int)damage).ToString(), defaultAttibute);
    }

    public DungeonEnum.FaceDirection curRunDirection = DungeonEnum.FaceDirection.None;
    public DungeonEnum.FaceDirection curAttackDirection = DungeonEnum.FaceDirection.None;
    /// <summary>
    /// 移动
    /// </summary>
    public virtual void UnitMove(DungeonEnum.FaceDirection direction, TileBlock moveTo, string actionEnd, GameObject endGameObject)
    {
        XPosition = moveTo.XPosition;
        YPosition = moveTo.YPosition;
        SetObjectName();
        if (curRunDirection != direction)
        {
            curRunDirection = direction;
            curAttackDirection = DungeonEnum.FaceDirection.None;
            ChangeState(ActionState.Run, curRunDirection, null, true);
        }
        AnimationHelper.AnimationMoveTo(moveTo.transform.localPosition, gameObject, iTween.EaseType.linear, endGameObject, actionEnd, 0.32f);
    }

    

    /// <summary>
    /// 被击退
    /// </summary>
    public virtual void BeHitBackRender(float backCount,DungeonEnum.FaceDirection backDirection)
    {
        Debug.Log("击退" + backCount + backDirection);
        int xP = XPosition;
        int yP = YPosition;
        TileBlock tb = null;
        if (backDirection == DungeonEnum.FaceDirection.Up)
        {
            int tempY = YPosition + (int)backCount;
            if (tempY > 8)
            {
                tempY = 8;
            }
            for (int i = YPosition + 1; i <= tempY; i++)
            {
                if (DungeonScene.FindEnemyUnit(xP, i) == null)
                {
                    yP++;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Down)
        {
            int tempY = YPosition - (int)backCount;
            if (tempY < 0)
            {
                tempY = 0;
            }
            for (int i = YPosition - 1; i >=tempY; i--)
            {
                if (DungeonScene.FindEnemyUnit(xP, i) == null)
                {
                    yP--;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Left)
        {
            int tempX = YPosition - (int)backCount;
            if (tempX < 0)
            {
                tempX = 0;
            }
            for (int i = XPosition - 1; i >= tempX; i--)
            {
                if (DungeonScene.FindEnemyUnit(i, yP) == null)
                {
                    xP--;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
        }
        else if (backDirection == DungeonEnum.FaceDirection.Right)
        {
            int tempX = XPosition + (int)backCount;
            if (tempX > 6)
            {
                tempX = 6;
            }
            for (int i = XPosition + 1; i <= tempX; i++)
            {
                if (DungeonScene.FindEnemyUnit(i, yP) == null)
                {
                    xP++;
                }
                else
                {
                    break;
                }
            }
            tb = DungeonScene.FindTile(xP, yP);
        }
    
        XPosition = tb.XPosition;
        YPosition = tb.YPosition;
        SetObjectName();
        AnimationHelper.AnimationMoveTo(tb.transform.localPosition, gameObject, iTween.EaseType.linear, null, null, 0.25f);
    }
    #endregion

}
