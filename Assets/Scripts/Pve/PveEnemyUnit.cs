using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJson;
public class PveEnemyUnit : PveFightUnit {

    /// <summary>
    /// 打击次数LABEL
    /// </summary>
    public EnemyBeHitLabel HitLabel;

    /// <summary>
    /// 当前打击次数
    /// </summary>
    public int CurHitNum;

    public int Range;

    public int RunPower;

    public int curRunPower;

    public JsonArray DropList;

    public MonsterData.MonsterType CurMonsterType = MonsterData.MonsterType.Comm;

    public int RunAwayNum;

	public bool RunAwayStatus;

    public InfoLabel CurInfoLabel;

	public GameObject runAwayIcon;

    /// <summary>
    /// 被打击次数渲染
    /// </summary>
    public void WillBeHitRender(int hitNum)
    {
        if (HitLabel && hitNum != CurHitNum)
        {
            CurHitNum = hitNum;
            HitLabel.SetNum(hitNum.ToString() + "h");
        }
    }

    public List<PveTile> ActionPath = new List<PveTile>();
    int curFindCount = 0;
    int ox = 0;
    int oy = 0;
    public void FindActionPath()
    {
        ActionPath.Clear();
        curFindCount = 0;
        ox = XPosition;
        oy = YPosition;
        curRunPower = RunPower;

        if (!PlayerInRange())
        {
            FindPathTile(GameControl.FindPveTile(XPosition, YPosition)); 
        }
        else  //原地可攻击 不需寻路 
        {
        }
        XPosition = ox;
        YPosition = oy;
        if(ActionPath.Count>0)
        {
            PveTile lastTile = ActionPath[ActionPath.Count - 1];
            List<PveTile> toPtRanges = FindRangeTile(lastTile.XPosition, lastTile.YPosition);
            foreach (PveTile tcp in toPtRanges)
            {
                tcp.CanMoveOn = false;
            }
        }
    }

    void FindPathTile(PveTile curPveTile)
    {
        if (curRunPower < 1)
        {
            ActionPath.Clear();
            return;
        }

        List<PveTile> templist = new List<PveTile>();
        templist.Add(curPveTile);
       
        //(1)查找周围可能移动的格子 如范围1，则neighbours.count<=8
        //List<PveTile> neighbours = GameControl.FindNeigherTileByRange(FindRangeTile(curPveTile.XPosition, curPveTile.YPosition),this);
        List<PveTile> neighbours = GameControl.FindNeigherTileByRange(templist, this);
        //(2)按距离排序
         //Debug.Log(neighbours.Count+"   --- "+name);
        for (int i = 0; i < neighbours.Count; i++)
        {
            
            for (int j = 0; j < neighbours.Count - i - 1; j++)
            {
                PveTile tb = neighbours[j];
                PveTile tb1 = neighbours[j + 1];
                if (CurMonsterType == MonsterData.MonsterType.Comm)
                {
                    if (tb.Distance(tb, GameControl.CurCharacter) >= tb1.Distance(tb1, GameControl.CurCharacter))
                    {
                        PveTile temp = neighbours[j];
                        neighbours[j] = neighbours[j + 1];
                        neighbours[j + 1] = temp;
                    }
                }
                else
                {
                    if (tb.Distance(tb, GameControl.CurCharacter) < tb1.Distance(tb1, GameControl.CurCharacter))
                    {
                        PveTile temp = neighbours[j];
                        neighbours[j] = neighbours[j + 1];
                        neighbours[j + 1] = temp;
                    }
                }
            }
        }

       

        PveTile nextPathTile = null;
        foreach (PveTile pt in neighbours)
        {
            List<PveTile> toPtRanges = FindRangeTile(pt.XPosition, pt.YPosition);
            List<PveTile> curPtRanges = FindRangeTile(XPosition, YPosition);
            if (toPtRanges.Count == XRange * YRange)
            {
                int count = toPtRanges.Count;
                foreach (PveTile rpt in toPtRanges)
                {
                    if (rpt.CanMoveOn)
                    {
                        count--;
                    }
                    else
                    {
                        if (curPtRanges.Contains(rpt))
                        {
                            count--;
                        }
                    }
                }
                if (count == 0)
                {
                    //下个方砖 nextPathTile
                    nextPathTile = pt;
                    ActionPath.Add(nextPathTile);

                    int tre = PointAngle(XPosition, YPosition, pt.XPosition, pt.YPosition);

                    //Debug.Log("===  " + nextPathTile.name);
                    if (tre == 0) curRunPower--;
                    if (tre == 1) curRunPower = curRunPower - 2;
                    
                    XPosition = pt.XPosition;
                    YPosition = pt.YPosition;
                   
                    CaculateRangeTiles();//--新生成 PlayerRange 

                    if (!PlayerInRange())
                    {
                        curFindCount++;
                    }
                    else
                    {
                        //可以攻击到主角  不再找下个方砖
                        curFindCount = RunPower;
                    }
                    break;
                }
            }
        }

        if (nextPathTile)
        {
            if (curFindCount < RunPower&& curRunPower>0)
            {
                FindPathTile(nextPathTile);
            }
        }
    }
    //判断俩点相对角度  0正面 1斜角
    public int PointAngle(int x0, int y0, int x1, int y1)
    {
        int r = 0;

        //正
        if ((x0 == x1 - 1 && y0 == y1) ||
            (x0 == x1 + 1 && y0 == y1) ||
            (x0 == x1 && y0 == y1 - 1) ||
             (x0 == x1 && y0 == y1 + 1))
        {
            r = 0;               
        }
        //角
        if ((x0 == x1 - 1 && y0 == y1 + 1) ||
            (x0 == x1 - 1 && y0 == y1 - 1) ||
            (x0 == x1 + 1 && y0 == y1 + 1) ||
            (x0 == x1 + 1 && y0 == y1 - 1))
        {
            r = 1;
        }       
        return r;
    }
    public List<PveTile> FindRangeTile(int xPosition,int yPosition)
    {
        List<PveTile> ranges = new List<PveTile>();
        for (int i = xPosition; i < xPosition + XRange; i++)
        {
            for (int j = yPosition; j < yPosition + YRange; j++)
            {
                PveTile pt = GameControl.FindPveTile(i, j);
                if (pt)
                {
                    ranges.Add(pt);
                }
            }
        }
        return ranges;
    }

    public bool PlayerInRange()
    {
        if (GetType() == typeof(PveMonster))
        {
            PveMonster pm = (PveMonster)this;
            if(pm.CurMonsterType == MonsterData.MonsterType.RunAway)
            {
                return false;
            }
        }
        foreach (PveTile tb in RangeTiles)
        {
            //纵向
            int tbX = tb.XPosition;
            int minY = tb.YPosition - Range;
            int maxY = tb.YPosition + Range;
            for (int i = minY; i <= maxY; i++)
            {
                if (GameControl.CurCharacter.XPosition == tbX && GameControl.CurCharacter.YPosition ==i)
                {
                    return true;
                }
            }
            //横向
            int tbY = tb.YPosition;
            int minX = tb.XPosition - Range;
            int maxX = tb.XPosition + Range;
            for (int i = minX; i <= maxX; i++)
            {
                if (GameControl.CurCharacter.XPosition == i && GameControl.CurCharacter.YPosition == tbY)
                {
                    return true;
                }
            }

        }
        return false;
    }
    //怪物开始行动
    int curMoveStep = 0;
    Action ActionEnd;

    public virtual void BeginAction(Action actionEnd)
    {
        //Debug.Log(name + "attack");
        AttackTag = false;  
        //skill  PSk9_1
        if (MissActionReturn() == false)
        {
            ActionEnd();
            return;
        }  

        if (GetType() == typeof(PveMonster))
        {
            PveMonster pm = (PveMonster)this;
            //CurInfoLabel.SetNum("9");
            if(pm.CurMonsterType == MonsterData.MonsterType.RunAway)
            {
                RunAwayNum--;
                if (RunAwayNum < 0)
                {
					this.RunAwayStatus = true;
					//触发成就
					AchievementManager.Trigger(AchievementTypeEnum.No_Monster_Escape, 1);
                    actionEnd();
                }
                else
                {
					CurInfoLabel.SetNum(RunAwayNum.ToString(), LayerHelper.UnitFX);
					CurInfoLabel.transform.localPosition = new Vector3(0.08f, -0.032f, 0);
                    ActionEnd = actionEnd;
                    curMoveStep = 0;
                    MoveOnce();
                }
            }
            else
            {
				CurInfoLabel.transform.localPosition = new Vector3(0.08f, -0.032f, 0);
                ActionEnd = actionEnd;
                curMoveStep = 0;
                MoveOnce();
            }
        }
        else if(GetType() == typeof(PveBoss)) //判断回合前技能并使用
        {
            ActionEnd = actionEnd;
            curMoveStep = 0;
            MoveOnce();
            //CurInfoLabel.transform.localPosition = new Vector3(0.05f, 0, 0);
        }
    }

    DungeonEnum.FaceDirection GetCharacterDirection()
    {
        if (GameControl.CurCharacter.XPosition < XPosition )
        {
            return DungeonEnum.FaceDirection.Left;
        }
        else if (GameControl.CurCharacter.XPosition > (XPosition + XRange - 1))
        {
            return DungeonEnum.FaceDirection.Right;
        }
        else if (GameControl.CurCharacter.YPosition < YPosition)
        {
            return DungeonEnum.FaceDirection.Down;
        }
        else if (GameControl.CurCharacter.YPosition > (YPosition + YRange - 1))
        {
            return DungeonEnum.FaceDirection.Up;
        }
        return DungeonEnum.FaceDirection.None;
    }

    Action AttackEndAction = null;
    public void Attack(Action callback = null)
    {
        if (callback != null) AttackEndAction = callback;
        //DungeonEnum.FaceDirection direction = GetTargetDirection(GameControl.CurCharacter);
        DungeonEnum.FaceDirection direction = GetCharacterDirection();
        if (GetType() == typeof(PveBoss) || GetType() == typeof(PveMonster))
        {
			// 显示怪物表情 攻击
			PveFaceManager.Trigger(this, PveFaceTypeEnum.ATTACK);

            if (Range > 1)
            {
                //远程攻击 固定方向
                direction = DungeonEnum.FaceDirection.LeftDown;
            }
        }
        UnitAttack(direction);
    }
    GameObject farAttackBall;

    float lastTimeDamage;
    public override void AttackBump()
    {
        if (Range == 1)
        {
            lastTimeDamage = GameControl.CurCharacter.BeHurt(this);

            if (lastTimeDamage > 0) AttackTag = true; //判断有效攻击
            else AttackTag = false;
        }
        else
        {
            //远程 

            Debug.Log(SkinData.FlyFXName);
            farAttackBall = Instantiate(Resources.Load("PreFabs/FX/" + SkinData.FlyFXName) as GameObject) as GameObject;
            farAttackBall.transform.position = transform.position;
            farAttackBall.SetActive(true);
            AnimationHelper.AnimationMoveToSpeed(GameControl.CurCharacter.transform.position, farAttackBall, iTween.EaseType.easeInExpo, gameObject, "FarParFlyEnd", 1.3f);
        }
    }


    public void BeTrapHurt(PveTrap t)
    {
        float lastDamage = t.DamagePersent / 10000.0f * Hp;
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
    }

    void FarParFlyEnd()
    {
        lastTimeDamage = GameControl.CurCharacter.BeHurt(this);

        if (lastTimeDamage > 0) AttackTag = true; //判断有效攻击
        else AttackTag = false;

        Vector3 v = farAttackBall.transform.position;
        Destroy(farAttackBall);
        GameObject bump = Instantiate(Resources.Load("PreFabs/FX/" + SkinData.FireFXName)) as GameObject;
        bump.transform.position = v;
        bump.SetActive(true);
        if(GetType() == typeof(PveBoss))
        {
            BossAttackSkill(() => 
            {
                ActionEnd();
            });
        }
        else
        {
            ActionEnd();
        }
    }



    public override void AttackEnd()
    {
        base.AttackEnd();

        if(AttackEndAction != null) //反击回调
        {
            AttackEndAction();
            AttackEndAction = null;
            return;
        }

        if (Range == 1)
        {
            if (GetType() == typeof(PveBoss))
            {
                BossAttackSkill(() =>
                {
                    ActionEnd();
                });
            }
            else
            {
                ActionEnd();
            }
        }
    }

    /// <summary>
    /// BOSS攻击时触发的BOSS技能
    /// </summary>
    void BossAttackSkill(Action callback)
    {
        if (GetType() == typeof(PveBoss))
        {
            PveGameControl.CurSkillTime = BossSkillController.TriggerType.Attack;
            PveBoss boss = (PveBoss)this;
            if (boss.ReadyForBossSkill())
            {
                Hashtable args = new Hashtable();
                args.Add("damage", lastTimeDamage);
                boss.UseSkill(args, () =>
                {
                    callback();
                });
            }
            else callback();
        }
        else callback();
    }

    public void MoveOnce()
    {
        MoveSpeed = 260;
        PveTrap tr = GameControl.TryTrap(XPosition, YPosition);
        if (tr && curMoveStep>0)
        {
            UnitWaiting(CurFaceDirection);
            BeTrapHurt(tr);
            ActionEnd();
        }
        else
        {
            
            if (curMoveStep >= ActionPath.Count)
            {
                if (PlayerInRange())
                {
                    if(GetType()==typeof(PveMonster))
                    {
                        PveMonster pm = (PveMonster)this;
                        if (pm.CurMonsterType == MonsterData.MonsterType.RunAway)
                        {
                            UnitWaiting(CurFaceDirection);
                            ActionEnd();
                        }
                        else
                        {
                            Attack();
                        }
                    }
                    else
                    {
                        Attack();
                    }
                }
                else
                {
                    UnitWaiting(CurFaceDirection);
                    ActionEnd();
                }
            }
            else if (curMoveStep < ActionPath.Count)
            {
                PveTile t = ActionPath[curMoveStep];
                curMoveStep++;
                DungeonEnum.FaceDirection direction = GetTargetDirection(t);
                UnitMove(t.XPosition, t.YPosition, () =>
                {
                    CaculateRangeTiles();
                    MoveOnce();
                }, direction);
            }
        }
    }
    
    public void RealHpReduce(Action callback)
    {
        if (CurUnitHp != null)
        {
            CurUnitHp.RealRefreshUI(() =>
            {
                callback();
            });
        }
        else
        {
            callback();
        }
    }
}
