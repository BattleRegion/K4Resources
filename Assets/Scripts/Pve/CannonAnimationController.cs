using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonAnimationController : MonoBehaviour
{
    public Animator curAnim;
    public int Xposition;
    public float damage;
    public PveGameControl GameControl;
    public PveCannon curCannon;

    List<PveFightUnit> targets = new List<PveFightUnit>();

    public void Attack()
    {
        curAnim.SetBool("Action", true);
    }

    public void AttackOver()
    {
        curAnim.SetBool("Action", false);
        curCannon.AttackEnd();
        targets.Clear();
    }

    void Hit(int Yposition)
    {
        if (GameControl.CurCharacter.XPosition == Xposition && GameControl.CurCharacter.YPosition == Yposition)
        {
            GameControl.CurCharacter.BeHurt(damage, curCannon);
        }
        else if (GameControl.FindEnemyOn(Xposition, Yposition))
        {
            PveFightUnit pfu = GameControl.FindEnemyOn(Xposition, Yposition);
            if (targets.Contains(pfu)) return;
            pfu.BeSpeciallyHurt(damage);
            targets.Add(pfu);
        }
    }

    void HitPosotion_1()
    {
        int Yposition = 8;

        Hit(Yposition);
    }

    void HitPosotion_2()
    {
        int Yposition = 7;

        Hit(Yposition);
    }

    void HitPosotion_3()
    {
        int Yposition = 6;

        Hit(Yposition);
    }

    void HitPosotion_4()
    {
        int Yposition = 5;

        Hit(Yposition);
    }

    void HitPosotion_5()
    {
        int Yposition = 4;

        Hit(Yposition);
    }

    void HitPosotion_6()
    {
        int Yposition = 3;

        Hit(Yposition);
    }

    void HitPosotion_7()
    {
        int Yposition = 2;

        Hit(Yposition);
    }

    void HitPosotion_8()
    {
        int Yposition = 1;

        Hit(Yposition);
    }

    void HitPosotion_9()
    {
        int Yposition = 0;

        Hit(Yposition);
    }
}
