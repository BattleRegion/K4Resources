using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Pet : OwnUnit
{
    #region 属性
    /// <summary>
    /// 用户宠物指针
    /// </summary>
    public UserPet UserPet;

    /// <summary>
    /// 当前在行动队列中第几位
    /// </summary>
    public int curActionIndex;

    /// <summary>
    /// 最后还需要攻击的目标
    /// </summary>
    List<EnemyUnit> lastNeedAttackNeighbours = new List<EnemyUnit>();

    /// <summary>
    /// 当前最后攻击了几个目标
    /// </summary>
    int curLastAttackCount = 0;

    /// <summary>
    /// 最后攻击的起始位置
    /// </summary>
    public TileBlock lastBeginBlock;
    #endregion

    #region 资源指针
    public GameObject[] PetFlyPar;

    public GameObject[] PetFlyBumpPar;
    #endregion

    #region 重写MONO
    void Awake()
    {
		//this.gameObject.SetActive (false);
        AnimationHelper.AnimationFadeTo(0, gameObject, iTween.EaseType.linear, null, null, 0);
    }
    #endregion

    #region 重写父类
    public override void SetObjectName()
    {
        name = "Pet:" + XPosition + "," + YPosition;
    }

    /// <summary>
    /// 重写开始行动
    /// </summary>
    public override void OwnBeginAction()
    {
        Invoke("DelayOwnBeginAction", curActionIndex * 0.4f);
    }

    /// <summary>
    /// 重写行为结束
    /// </summary>
    public override void AllActionEnd()
    {
        DungeonEnum.FaceDirection direction = curRunDirection;
        if (curRunDirection == DungeonEnum.FaceDirection.None)
        {
            direction = curAttackDirection;
        }
        UnitWaiting(direction);
        //移动回去
        DungeonEnum.FaceDirection d1 = GetTargetDirection(lastBeginBlock);
        UnitMove(d1, lastBeginBlock, "AttackEndMoveEnd",gameObject);

    }
    #endregion 

    #region 工具方法
    DungeonEnum.FaceDirection ReverseDirection(DungeonEnum.FaceDirection direciton)
    {
        if (direciton == DungeonEnum.FaceDirection.Down)
        {
            return DungeonEnum.FaceDirection.Up;
        }
        else if (direciton == DungeonEnum.FaceDirection.Left)
        {
            return DungeonEnum.FaceDirection.Right;
        }
        else if (direciton == DungeonEnum.FaceDirection.LeftDown)
        {
            return DungeonEnum.FaceDirection.UpRight;
        }
        else if (direciton == DungeonEnum.FaceDirection.LeftUp)
        {
            return DungeonEnum.FaceDirection.RightDown;
        }
        else if (direciton == DungeonEnum.FaceDirection.Right)
        {
            return DungeonEnum.FaceDirection.Left;
        }
        else if (direciton == DungeonEnum.FaceDirection.RightDown)
        {
            return DungeonEnum.FaceDirection.LeftUp;
        }
        else if (direciton == DungeonEnum.FaceDirection.Up)
        {
            return DungeonEnum.FaceDirection.Down;
        }
        else if (direciton == DungeonEnum.FaceDirection.UpRight)
        {
            return DungeonEnum.FaceDirection.LeftDown;
        }
        return DungeonEnum.FaceDirection.None;
    }
    #endregion

    #region 自己的行为
    /// <summary>
    /// 延迟开始行为
    /// </summary>
    void DelayOwnBeginAction()
    {
        //从头像UI中飞出来
        PetFlyToScene(() =>
        {
            base.OwnBeginAction();
        });
        
    }


    /// <summary>
    /// 宠物移动结束后剩余不再周围的怪物攻击
    /// </summary>
    void PetLastAttack()
    {
        TileBlock b = DungeonScene.CurPlayer.CurActionPath[DungeonScene.CurPlayer.CurActionPath.Count - 1];
        DungeonEnum.FaceDirection direction = GetTargetDirection(b);
        UnitMove(direction, b, "AttackBeforeMoveEnd",gameObject);
    }

    /// <summary>
    /// 移动回调
    /// </summary>
    void AttackBeforeMoveEnd()
    {
        if (curLastAttackCount < lastNeedAttackNeighbours.Count)
        {
            EnemyUnit e = lastNeedAttackNeighbours[curLastAttackCount];
            curLastAttackCount++;
            OwnUnitAttack(e, () =>
            {
                AttackBeforeMoveEnd();
            });
        }
        else
        {
            //移动回去
            DungeonEnum.FaceDirection direction = GetTargetDirection(lastBeginBlock);
            UnitMove(direction, lastBeginBlock, "AttackEndMoveEnd",gameObject);
        }
    }

    /// <summary>
    /// 移动回调
    /// </summary>
    void AttackEndMoveEnd()
    {
        curRunDirection = ReverseDirection(curRunDirection);
        UnitWaiting(curRunDirection);
        PetBackToAvata(() =>
            {
                base.AllActionEnd();
            });
    }

    /// <summary>
    /// 宠物飞入场景
    /// </summary>
    PetAvata pa;
    Action ToSceneAction;
    Action ToAvataAction;
    void PetFlyToScene(Action flyToSceneEnd)
    {
        ToSceneAction = flyToSceneEnd;
        pa = DungeonScene.CurDungeonUI.FindPetAvata(UserPet);
        pa.hasFocus = false;
        PetParFly(transform.position, pa.transform.position);
    }

    /// <summary>
    /// 粒子飞行
    /// </summary>
    /// <param name="to"></param>
    GameObject curFlyPetPar;
    void PetParFly(Vector3 to,Vector3 from)
    {
        curFlyPetPar = Instantiate(GetPetFlyFX()) as GameObject;
        curFlyPetPar.transform.position = from;

        float rotateAngle = 0;
        float x = Mathf.Abs(to.x - from.x);
        float y = Mathf.Abs(to.y - from.y);
        rotateAngle = 90 - Mathf.Atan(y / x) * 180 / Mathf.PI;
        if (to.x > from.x)
        {
            rotateAngle = rotateAngle * -1;
        }
        curFlyPetPar.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);
        AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), curFlyPetPar, iTween.EaseType.easeOutExpo, gameObject, "ParScaleBigEnd", 0.05f);
    }

    /// <summary>
    /// 粒子放大回调
    /// </summary>
    void ParScaleBigEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(0.4f, 1.8f, 1), curFlyPetPar, iTween.EaseType.easeInExpo, null, null, 0.1f);
        Invoke("ParFly", 0.05f);
    }

    /// <summary>
    /// 粒子飞行动画
    /// </summary>
    void ParFly()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(transform.position.x, transform.position.y, 0), curFlyPetPar, iTween.EaseType.linear, null, null, 0.2f);
        pa.AvataRender.gameObject.SetActive(false);
        FrameAnimation();
        Invoke("ParFlyEnd", 0.16f);
    }

    /// <summary>
    /// 飞行结束
    /// </summary>
    void ParFlyEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(0.5f, 0.5f, 0.5f), curFlyPetPar, iTween.EaseType.easeOutExpo, gameObject, "ParFlyEndScale", 0.05f);
    }

    void ParFlyEndScale()
    {
        Destroy(curFlyPetPar);
        GameObject parBump = Instantiate(GetBumpPar()) as GameObject;
        parBump.transform.position = transform.position;
        AnimationHelper.AnimationFadeTo(1, gameObject, iTween.EaseType.linear, gameObject, "FlyInEnd", 0.2f);
        
    }


    void FlyInEnd()
    {
        ToSceneAction();
    }

    /// <summary>
    /// 获取宠物飞行粒子
    /// </summary>
    /// <returns></returns>
    GameObject GetPetFlyFX()
    {
        if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Earth)
        {
            return PetFlyPar[1];
        }
        else if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Fire)
        {
            return PetFlyPar[2];
        }
        else if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Wind)
        {
            return PetFlyPar[3];
        }
        else if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Water)
        {
            return PetFlyPar[0];
        }
        return null;
    }

    GameObject GetBumpPar()
    {
        if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Earth)
        {
            return PetFlyBumpPar[1];
        }
        else if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Fire)
        {
            return PetFlyBumpPar[2];
        }
        else if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Wind)
        {
            return PetFlyBumpPar[3];
        }
        else if (UserPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.Water)
        {
            return PetFlyBumpPar[0];
        }
        return null;
    }

    /// <summary>
    /// 飞回头像
    /// </summary>
    void PetBackToAvata(Action backToAvata)
    {
        ToAvataAction = backToAvata;
        AnimationHelper.AnimationFadeTo(0, gameObject, iTween.EaseType.linear, null, null, 0.2f);
        PetFlyBack(pa.transform.position, transform.position);
    }

    void PetFlyBack(Vector3 to, Vector3 from)
    {
        curFlyPetPar = Instantiate(GetPetFlyFX()) as GameObject;
        curFlyPetPar.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        curFlyPetPar.transform.position = from;
        float rotateAngle = 0;
        float x = Mathf.Abs(to.x - from.x);
        float y = Mathf.Abs(to.y - from.y);
        rotateAngle = 90 - Mathf.Atan(y / x) * 180 / Mathf.PI;
        if (to.x < from.x)
        {
            rotateAngle = rotateAngle * -1;
        }
        curFlyPetPar.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);
        ParBackScaleBigEnd();
    }

    void ParBackScaleBigEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(0.4f, 1.8f, 1), curFlyPetPar, iTween.EaseType.easeInExpo, null, null, 0.1f);
        Invoke("ParBackFly", 0.05f);
    }

    void ParBackFly()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(pa.transform.position.x, pa.transform.position.y, 0), curFlyPetPar, iTween.EaseType.linear, null, null, 0.2f);
        Invoke("ParFlyBackEnd", 0.16f);
    }

    void ParFlyBackEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), curFlyPetPar, iTween.EaseType.easeOutExpo, gameObject, "ParBackFlyEndScale", 0.05f);
        pa.AvataRender.gameObject.SetActive(true);
        FrameAnimation();
    }

    void ParBackFlyEndScale()
    {
        Destroy(curFlyPetPar);
        GameObject parBump = Instantiate(GetBumpPar()) as GameObject;
        parBump.transform.position = pa.transform.position;
        ToAvataAction();
    }

    void FrameAnimation()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(pa.transform.localPosition.x, -30, pa.transform.localPosition.z), pa.gameObject, iTween.EaseType.linear, null, "", 0.1f);
        AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), pa.gameObject, iTween.EaseType.easeOutExpo, gameObject, "FrameScaleEnd", 0.1f);
    }

    void FrameScaleEnd()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(pa.transform.localPosition.x, -18, pa.transform.localPosition.z), pa.gameObject, iTween.EaseType.linear, null, "", 0.05f);
        AnimationHelper.AnimationScaleTo(new Vector3(1, 1, 1), pa.gameObject, iTween.EaseType.easeInExpo, null, null, 0.05f);
    }

    /// <summary>
    /// 回复能量
    /// </summary>
    public void PetRecoverPower()
    {
        CurPower = CurPower + 1;
        if (CurPower >= UserPet.CurPetData.PetSkillData.SkillPower)
        {
            CurPower = UserPet.CurPetData.PetSkillData.SkillPower;
            DungeonScene.CurDungeonUI.AvataEffectRender(true, this.UserPet);
        }
    }
    #endregion

    #region 重写释放技能
    public override void UseSkill()
    {
        CurPower = 0;
        DungeonScene.IsSkilling = true;
        foreach (EliminateBlock eb in DungeonScene.CurAllEliminateBlock)
        {
            eb.AttrubuteToRender();
        }
        Invoke("PlayerDis", 0.3f);
        PetFlyToScene(() =>
        {
            GameObject shine = Instantiate(Resources.Load("PreFabs/FX/before_skill")) as GameObject;
            shine.transform.position = transform.position;
            shine.SetActive(true);
            Invoke("ShineEnd", 0.6f);
        });
    }

    void ShineEnd()
    {
        float t = 0;
        for (int i = 0; i < FireFrame - 1; i++)
        {
            t = t + AttackFrameRate;
        }
        Invoke("SkillRenderShow", t);
        ChangeState(ActionState.Attack, DungeonEnum.FaceDirection.Down, () =>
        {
            UnitWaiting(DungeonEnum.FaceDirection.Down);
        }, false);
    }

    void SkillRenderShow()
    {
        GameObject resource = Resources.Load(DungeonSpritePathManager.SkillFX(Skill.Id)) as GameObject;
        GameObject skill = null;
        if (resource)
        {
            skill = Instantiate(resource) as GameObject;
        }
        if (Skill.Id == "PSk472")
        {
            skill.transform.position = transform.position;
            float hpRecover = float.Parse(Skill.Yparameter);
            DungeonScene.CurPlayer.HpRecoverRender(hpRecover);
            Invoke("SkillShowEnd", 1.5f);
        }
        else if (Skill.Id == "PSk490")
        {
            skill.transform.position = transform.position;
            Invoke("SkillShowEnd", 0.5f);
        }
        else if (Skill.Id == "PSk439")
        {
            SkillShowEnd();
        }
        else if (Skill.Id == "PSk575")
        {
            Invoke("SkillShowEnd", 1.2f);
        }
        else
        {
            Invoke("SkillShowEnd", 3);
        }
    }

    void SkillShowEnd()
    {
        if (Skill.Id == "PSk490")
        {
            //随机一个位置
            List<EliminateBlock> bs = new List<EliminateBlock>();
            foreach (EliminateBlock b in DungeonScene.CurAllEliminateBlock)
            {
                if (DungeonScene.FindEnemyUnit(b.XPosition, b.YPosition) == null)
                {
                    bs.Add(b);
                }
            }
            int randomIndex = UnityEngine.Random.Range(0, bs.Count);
            EliminateBlock tb = bs[randomIndex];
            GameObject skill = Instantiate(Resources.Load(DungeonSpritePathManager.SkillFX(Skill.Id))) as GameObject;
            skill.transform.position = tb.transform.position;
            DungeonScene.curPlayerEliminateBlock.CurEliminateAttribute = tb.CurEliminateAttribute;
            DungeonScene.curPlayerEliminateBlock.AttrubuteToRender();
            DungeonScene.curPlayerEliminateBlock = tb;
            DungeonScene.curPlayerEliminateBlock.SetToPlayerRender();
            DungeonScene.CurPlayer.gameObject.SetActive(true);
            DungeonScene.CurPlayer.SetPosition(tb.XPosition, tb.YPosition);
            foreach (OwnUnit o in DungeonScene.AllOwnUnits)
            {
                o.SetPosition(DungeonScene.CurPlayer.XPosition, DungeonScene.CurPlayer.YPosition);
            }
        }
        PetBackToAvata(() =>
        {
            DungeonScene.CurPlayer.gameObject.SetActive(true);
            DungeonScene.userInputLock = false;
            DungeonScene.IsSkilling = false ;
            DungeonScene.CurDungeonUI.AvataEffectRender(false, UserPet);
            foreach (TileBlock tb in DungeonScene.AllRangesTile)
            {
                if (Skill.Id == "PSk492" || Skill.Id == "PSk438" || Skill.Id == "PSk575")
                {
                    GameObject bump = Instantiate(Resources.Load(DungeonSpritePathManager.SkillBumpFX(Skill.SkillFX))) as GameObject;
                    bump.transform.position = tb.transform.position;
                    EnemyUnit e = DungeonScene.FindEnemyUnit(tb.XPosition, tb.YPosition);
                    if (e)
                    {
                        e.SkillHurtRender(Skill);
                    }
                }
                else if (Skill.Id == "PSk439")
                {
                    GameObject bump = Instantiate(Resources.Load(DungeonSpritePathManager.SkillBumpFX(Skill.SkillFX))) as GameObject;
                    bump.transform.position = tb.transform.position;
                    StartCoroutine(ChangeBlock(DungeonScene.FindEliminateByPosition(tb.XPosition,tb.YPosition)));
                }
            }
            Invoke("RemoveBack", 0.3f);
        });
    }

    IEnumerator ChangeBlock(EliminateBlock b)
    {
        yield return new WaitForSeconds(0.2f);
        if (b)
        {
            b.CurEliminateAttribute = (DungeonEnum.ElementAttributes)((int)(Skill.Aparameter));
            b.AttrubuteToRender();
        }
    }

    void RemoveBack()
    {
        DungeonScene.ShowSkillBack(false);
    }

    void PlayerDis()
    {
        DungeonScene.CurPlayer.gameObject.SetActive(false);
    }
    #endregion
}
