using UnityEngine;
using System.Collections;

public class FightHp : MonoBehaviour {
    public SpriteRenderer hpSprite;
    public SpriteRenderer frameSprite;
    public SpriteRenderer backSprite;
    public enum HpType
    {
        Player = 1,
        Enemy = 2
    }

    public Sprite[] Sprites;

    public HpType curHpType;
	// Use this for initialization
	void Start () 
    {
        if (curHpType == HpType.Enemy)
        {
            hpSprite.sprite = Sprites[1];
        }
        else
        {
            hpSprite.sprite = Sprites[0];
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 刷新血条
    /// </summary>
    /// <param name="curHp"></param>
    /// <param name="hpLimit"></param>
    public void RefreshHp(float curHp,float hpLimit)
    {
        if (curHp < 0)
        {
            curHp = 0;
        }
        iTween.ScaleTo(hpSprite.gameObject, new Vector3((float)curHp / (float)hpLimit, 1, 1), 1.5f);
        if (curHpType == HpType.Player)
        {
            FightPlayerUI playerUI = GameObject.Find("PlayerUI").GetComponent<FightPlayerUI>();
            playerUI.PlayerHpUI.SetCurHpShow(curHp, hpLimit);
        }
    }

    public void RealRefresh()
    {
    }

    public void SetZorder(int basicZ)
    {
        frameSprite.sortingOrder = basicZ + 3;
        hpSprite.sortingOrder = basicZ + 2;
        backSprite.sortingOrder = basicZ + 1;
    }
}
