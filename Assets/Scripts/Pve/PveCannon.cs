using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PveCannon : PveFightUnit
{
    public CannonAnimationController AnimationController;
    public float damage;

    public int Number;
    public SpriteRenderer NumberSprite;

    public override void SetName()
    {
        name = "Cannon:" + XPosition + "," + YPosition;
    }

    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 60, transform.localPosition.z);
        AnimationController.Xposition = xPosition;   
    }

    Color GetColor(int index)
    {
        if (index == 1) return Color.red;
        if (index == 2) return Color.yellow;
        if (index == 3) return Color.white;
        if (index == 4) return Color.green;
        return Color.white;
    }

    /// <summary>
    /// 1 红 2 黄 3 白 4 绿
    /// </summary>
    public void RenderCannon()
    {
        NumberSprite.color = GetColor(Number);

        AnimationController.damage = damage;
        AnimationController.GameControl = GameControl;
        AnimationController.curCannon = this;
    }

    public void Attack()
    {
        AnimationController.Attack();
        GameControl.UserInputLock = true;
        GameControl.CurCharacter.ShowArrow(false);
    }

    public override void AttackEnd()
    {
        GameControl.CurCharacter.ShowArrow(true);

        GameControl.CharacterDead(); //判断主角是否死亡

        List<PveEnemyUnit> allDead = new List<PveEnemyUnit>();
        foreach (PveEnemyUnit pe in GameControl.AllEnemies)
        {
            if (pe.CurHp <= 0)
            {
                allDead.Add(pe);
            }
        }
        if (allDead.Count == 0)
        {
            GameControl.UserInputLock = false;
            GameControl.IsSkilling = false;
        }
        else
        {
            int curHasDeadCount = 0;
            Debug.Log(allDead.Count);
            for(int i = allDead.Count - 1; i >= 0; i--)
            {
                PveEnemyUnit pe = allDead[i];
                if (pe.CurHp <= 0)
                {
                    if (pe.GetType() == typeof(PveCannonTrigger))
                    {
                        pe.UnitDead((p) =>
                        {
                            allDead.Remove(pe);
                            if (allDead.Count == 0)
                            {
                                GameControl.UserInputLock = false;
                                GameControl.IsSkilling = false;
                            }
                        });
                    }
                    else
                    {
                        pe.UnitDead((p) =>
                        {
                            GameObject disObject = Instantiate(Resources.Load("PreFabs/FX/Monster_D")) as GameObject;
                            disObject.transform.position = p.transform.position;

                            GameControl.EnemyUnitEliminateEnable(true);//2015-04-20
                            GameControl.AllEnemies.Remove((PveEnemyUnit)p);
                            Destroy(p.gameObject);



                            curHasDeadCount++;
                            int count = 0;
                            foreach (PveEnemyUnit pu in GameControl.AllEnemies)
                            {
                                if (pu.GetType() != typeof(PveBarrier) && pu.GetType() != typeof(PveCannonTrigger) && pu.CurState != PveFightUnit.UnitState.guard)
                                {
                                    count++;

                                    //Debug.Log(" ----" + pu.name);
                                }

                                if (pu.GetType() == typeof(PveBoss) && pu.CurState == PveFightUnit.UnitState.guard)
                                {
                                    //Debug.Log(" ----" + pu.name);
                                    count++;
                                }
                            }
                            if (count != 0)
                            {
                                if (curHasDeadCount == allDead.Count)
                                {
                                    GameControl.UserInputLock = false;
                                    GameControl.IsSkilling = false;
                                }
                            }
                            else
                            {
                                if (GameControl.CurFloor < ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId).FloorCount)
                                {
                                    GameControl.Door.OpenDoor();
                                    GameObject doorOpen = Instantiate(Resources.Load("PreFabs/FX/KEY_use")) as GameObject;
                                    doorOpen.transform.position = new Vector3(0, 0.75f, 0);
                                    doorOpen.SetActive(true);
                                    PveTile endTile = GameControl.FindPveTile(3, 8);
                                    GameControl.CurCharacter.UnitMove(endTile.XPosition, endTile.YPosition, () =>
                                    {
                                        GameControl.AttackCL.Out();
                                        GameControl.CurCharacter.ShowArrow(false);
                                        GameControl.IsSkilling = false;
                                        GameControl.BeginFloorLoad();
                                    }, DungeonEnum.FaceDirection.Up);
                                }
                                else
                                {

                                    GameControl.RenderSuccess();
                                }
                            }
                        });
                    }
                }
            }
        }
    }

}
