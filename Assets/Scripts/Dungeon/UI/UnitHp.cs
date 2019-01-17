using UnityEngine;
using System.Collections;

public class UnitHp : MonoBehaviour
{
    #region 枚举
    public enum HpType
    {
        OwnUnit = 1,
        EnemyUnit = 2,
        Boss=3
    }
    #endregion

    #region 属性
    public SpriteRenderer hpSprite;
    public SpriteRenderer frameSprite;
    public SpriteRenderer backSprite;
    public Sprite[] Sprites;
    public HpType curHpType;
    #endregion


    // Use this for initialization
	void Start () 
	{
        transform.localScale = new Vector3(transform.localScale.x / transform.parent.transform.localScale.x,transform.localScale.y / transform.parent.transform.localScale.y, transform.localScale.z / transform.parent.transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (curHpType == HpType.OwnUnit)
        {
            hpSprite.sprite = Sprites[0];
        }
        else if (curHpType == HpType.Boss)
        {
            hpSprite.sprite = Sprites[2];
        }
        else
        {
            hpSprite.sprite = Sprites[1];
        }
	}

    float curPersent = 0.001f;
    public void RefreshUI(float curHp, float Hp) //血条前景
    {
        if (curHp < 0)
        {
            curHp = 0;
        }
        curPersent = (float)curHp / (float)Hp;
        iTween.ScaleTo(hpSprite.gameObject, new Vector3(curPersent, 1, 1), 1f);
    }

    System.Action endCallback;
    public void RealRefreshUI(System.Action callback) //血条背景
    {
        endCallback = callback;
        if (backSprite && curPersent != 0.001f) iTween.ScaleTo(backSprite.gameObject, new Vector3(curPersent, 1, 1), 1f);
        Invoke("End", 1);
    }

    void End()
    {
        endCallback();
    }
}
