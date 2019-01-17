using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

public class PveCrystal : PveEnemyUnit
{
    public override void SetName()
    {
        name = "Crystal:" + XPosition + "," + YPosition;
    }

    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 60, transform.localPosition.z);
    }

    List<SpriteRenderer> bossSprites = new List<SpriteRenderer>();
    string attackFxPath = "PreFabs/FX/FX54";
    string reduceDefFXPath = "PreFabs/FX/skill_9004";

    string removeColorMaterialPath = "K4Shaders/RemoveColor";
    Action attackActionEnd;

    public int shutDownCount; //自动关闭的回合数，boss技能Y参数

    int activeCount = 0;
    public bool activeState = false;

    void ChangeSprite(Transform t, bool rmColor)
    {
        if(t.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            sr.material.shader = Resources.Load<Shader>(removeColorMaterialPath);
            if(rmColor == false)
            {
                sr.color = Color.black;
            }
            else
            {
                sr.color = Color.white;
            }
        }
        foreach(Transform child in t)
        {
            ChangeSprite(child, rmColor);
        }
    }

    //设置激活
    public void SetCrystalState(bool state)
    {
        if(state == false)
        {
            activeCount = 0;
            ChangeSprite(RenderObject.transform, false);
        }
        else
        {
            ChangeSprite(RenderObject.transform, true);
        }
        RenderObject.GetComponent<Animator>().SetBool("Action", state);
        activeState = state;
        
    }

    //攻击boss
    public void skillAttack(PveBoss boss, Action callback)
    {
        attackActionEnd = callback;

        GameObject prefab = Resources.Load<GameObject>(attackFxPath);
        Component AnimationJscript = prefab.GetComponent("DeadTime");  //C#访问JS
        float duringTime = 0f;
        if (AnimationJscript)
        {
            FieldInfo parameter = AnimationJscript.GetType().GetField("deadTime");
            duringTime = float.Parse(parameter.GetValue(AnimationJscript).ToString());
        }

        GameObject SkillFX = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        Tools.SetLayer(SkillFX.transform, LayerHelper.UnitFX);

        GameObject prefab_2 = Resources.Load<GameObject>(reduceDefFXPath);
        GameObject SkillFX_2 = Instantiate(prefab_2, boss.transform.position, Quaternion.identity) as GameObject;
        Tools.SetLayer(SkillFX_2.transform, LayerHelper.UnitFX);

        //ChangeBossSprite(boss.RenderObject.transform);

        Debug.Log(boss.Def);
        Hashtable args = new Hashtable();
        args.Add("def", -1f);

        PveBuffData buff = new PveBuffData(args, int.Parse(boss.GetComponent<BossSkillAI>().CurBossData.BossSkill.Aparameter));
        boss.AddBuff(buff);
        Debug.Log(boss.Def);

        Invoke("SkillRenderEnd", duringTime);


    }

    void SkillRenderEnd()
    {

        attackActionEnd();
    }

    public override void BeginAction(Action actionEnd)
    {
        PveTile pt = GameControl.FindPveTile(XPosition, YPosition);
        List<PveTile> RangeTiles = GameControl.FindNeighbourTileIn(pt, GameControl.AllPveTiles);
        PveBoss bossInRange = null;
        foreach(PveTile p in RangeTiles)
        {
            if(GameControl.HasEnemyWithTile(p))
            {
                if(GameControl.FindEnemyOn(p.XPosition, p.YPosition).GetType() == typeof(PveBoss))
                {
                    bossInRange = (PveBoss)GameControl.FindEnemyOn(p.XPosition, p.YPosition);
                }
            }
        }

        if(bossInRange != null)
        {
            skillAttack(bossInRange, () => 
            {
                SetCrystalState(false);
                actionEnd();
            });
        }
        else
        {
            activeCount++;
            if(activeCount > shutDownCount)
            {
                SetCrystalState(false);
                actionEnd();
            }
            else
            {
                actionEnd();
            }
        }
    }
}
