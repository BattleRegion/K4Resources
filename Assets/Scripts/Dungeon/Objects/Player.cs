using UnityEngine;
using System.Collections;

public class Player : OwnUnit
{
    #region 属性

    public bool HasFloorKey = false;

    public GameObject PlayerArrow;

    public bool GodMode = false;
    #endregion

    #region 资源指针
    /// <summary>
    /// 蓄力资源
    /// </summary>
    public GameObject SkillCareFX;
    #endregion

    #region 重写父类
    /// <summary>
    /// 设置名字
    /// </summary>
    public override void SetObjectName()
    {
        name = "Player:" + XPosition + "," + YPosition;
    }

    /// <summary>
    /// 单位一次行动
    /// </summary>
    public override void OwnUnitOnceAction()
    {
        DungeonScene.FindEliminateByPosition(XPosition, YPosition).BlockEliminateRender();
        base.OwnUnitOnceAction();
    }

    /// <summary>
    /// 所有行为结束
    /// </summary>
    public override void AllActionEnd()
    {
        if (Skill.Bparameter < DungeonScene.curLinkPath.Count)
        {
            //奋力一击
            PlayerSkillSlashingStrike();
        }
        else
        {
            base.AllActionEnd();
        }
    }

    /// <summary>
    /// 消失
    /// </summary>
    public override void ObjectDisAppear()
    {
        ChangeState(ActionState.Dead, DungeonEnum.FaceDirection.LeftDown, () =>
        {
            //base.ObjectDisAppear();

        }, false);
    }

    public override void BeHurt(DungeonUnit hurtFrom, bool needDelay)
    {
        if (GodMode)
        {
            hurtFrom.Atk = 0;
        }
        base.BeHurt(hurtFrom, needDelay);
        DungeonScene.CurDungeonUI.CurPlayerUIInfo.HpUI.SetCurHpShow(CurHp, Hp);
    }
    #endregion 

    #region 奋力一击
    GameObject skillCareObject;
    void PlayerSkillSlashingStrike()
    {
        if (Skill != null)
        {
            skillCareObject = Instantiate(SkillCareFX) as GameObject;
            skillCareObject.transform.position = transform.position;
            StopAnimation();
            string stateAnimationPath = "Atlas/Character/" + SkinId.ToString() + "/" + SkinId.ToString() + ((int)ActionState.Attack).ToString() + ((int)RemoveBlock.LinkDirection.Down).ToString();
            Sprite[] attackSprites = Resources.LoadAll<Sprite>(stateAnimationPath);
            ObjectSprite.sprite = attackSprites[2];
            //ObjectSprite.transform.localPosition = new Vector3(OriginSpritePosition.x + AttackOffsetX, OriginSpritePosition.y + AttackOffsetY, OriginSpritePosition.z);
            Invoke("PowerGetEnd", 1.2f);
        }
        else
        {
            base.AllActionEnd();
        }
    }

    /// <summary>
    /// 蓄力结束
    /// </summary>
    void PowerGetEnd()
    {
        Destroy(skillCareObject);
        float t = 0;
        for (int i = 0; i < FireFrame - 1; i++)
        {
            t = t + AttackFrameRate;
        }
        Invoke("RangeBumpRender", t);
        ChangeState(ActionState.Attack,DungeonEnum.FaceDirection.Down, () =>
        {
            base.AllActionEnd();
        },false);
    }

    /// <summary>
    /// 范围技能渲染
    /// </summary>
    void RangeBumpRender()
    {
        if (UserManager.CurUserInfo.CurWeapon != null)
        {
            if (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Light || UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style == HardWareData.HardWareType.Heavy)
            {
                foreach (TileBlock t in DungeonScene.AllRangesTile)
                {
                    if (t.XPosition == XPosition && t.YPosition == YPosition)
                    {
                    }
                    else
                    {
                        StartCoroutine(PowerSkillRenderBump(t));
                    }
                }
            }
            else
            {
                Debug.Log("远程");
            }
        }
    }

    /// <summary>
    /// 逐个延迟渲染
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator PowerSkillRenderBump(TileBlock t)
    {
        yield return new WaitForSeconds(Distance(this, t) * 0.15f);
        GameObject skillResource = Resources.Load(DungeonSpritePathManager.SkillBumpFX(Skill.SkillFX)) as GameObject;
        GameObject skillBump = Instantiate(skillResource) as GameObject;
        skillBump.transform.position = t.transform.position;
        skillBump.SetActive(true);
        CameraControl.ShakeCamera();
        EnemyUnit e =  DungeonScene.FindEnemyUnit(t.XPosition, t.YPosition);
        if (e)
        {
            if (EnemyUnit.CanAttack(e, this))
            {
                e.SkillHurtRender(Skill);
            }
        }
    }
    #endregion

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    public void ShowArrow(bool show)
    {
        PlayerArrow.gameObject.SetActive(show);
    }

    /// <summary>
    /// HP回复渲染
    /// </summary>
    public void HpRecoverRender(float hpRecover)
    {
        Debug.Log("回复！");
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
        DungeonScene.CurDungeonUI.CurPlayerUIInfo.HpUI.SetCurHpShow(CurHp, Hp);
    }

    public override void BeHitBackRender(float backCount, DungeonEnum.FaceDirection backDirection)
    {
        DungeonScene.curPlayerEliminateBlock.SetSprite();
        base.BeHitBackRender(backCount, backDirection);
        EliminateBlock eb = DungeonScene.FindEliminateByPosition(XPosition, YPosition);
        eb.SetToPlayerRender();
    }
}
