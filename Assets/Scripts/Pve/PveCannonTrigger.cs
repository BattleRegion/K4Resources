using UnityEngine;
using System.Collections;

public class PveCannonTrigger : PveEnemyUnit
{
    public Animator AnimationController;

    public int Number;
    public SpriteRenderer upNumberSprite;
    public SpriteRenderer downNumberSprite;

    public override void SetName()
    {
        name = "Trigger:" + XPosition + "," + YPosition;
    }

    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 28, transform.localPosition.z);
        gameObject.layer = LayerHelper.Unit;
        upNumberSprite.sortingOrder++;
        downNumberSprite.sortingOrder++;
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
    public void RenderTrigger()
    {
        upNumberSprite.color = GetColor(Number);
        downNumberSprite.color = GetColor(Number);
    }

    public override void UnitDead(System.Action<PveFightUnit> curDeadAction)
    {
        SetState(true);
        curDeadAction(this);
    }

    //true为触发（弹起可以按），false为未触发（下陷不可按）
    public void SetState(bool value)
    {
        AnimationController.SetBool("Action", value);
        if(value)
        {
            CurState = UnitState.guard;
            GetComponent<CircleCollider2D>().enabled = true;
        }
        else
        {
            CurHp = Hp;
            CurState = UnitState.normal;
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    void OnMouseDown()
    {
        if (GameControl != null && !GameControl.UserInputLock)
        {
            PveBoss curBoss = null;
            BossSkillAI_16 BossSkill = null;
            foreach (PveFightUnit p in GameControl.AllEnemies)
            {
                if (p.GetType() == typeof(PveBoss))
                {
                    if (p.GetComponent<BossSkillAI_16>() != null)
                    {
                        curBoss = (PveBoss)p;
                        BossSkill = curBoss.GetComponent<BossSkillAI_16>();
                        break;
                    }
                }
            }
            if (BossSkill != null)
            {
                PveCannon relateCannon = null;
                foreach (PveCannon p in BossSkill.Cannons)
                {
                    if (p.Number == Number)
                    {
                        relateCannon = p;
                    }
                }
                if (relateCannon != null)
                {
                    relateCannon.Attack();
                    SetState(false);
                }
            }
        }
    }
}
