using UnityEngine;
using System.Collections;

public class PveMonster : PveEnemyUnit
{

    public GameObject Element_monster;

	public int InitXPosition;
	public int InitYPosition;

    #region MONO

    #endregion

    #region 重写父类
    public override void SetName()
    {
        name = "Monster:" + XPosition + "," + YPosition;
    }
    #endregion
    public void ShowElement()
    {
        SpriteRenderer sr= Element_monster.transform.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Atlas/Fight/pveNewCell/" + (int)Element);
    }
}
