using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FightLabel : MonoBehaviour {

    public enum LabelType
    {
        ComboLabel = 1,
        ComboAddition = 2,
        Hit =3,
        ActionCombo = 4,
        FightInfo = 5,
        PlayerLvl = 6,
        HpUI= 7,
        Damage = 8
    }

    /// <summary>
    /// 数字文本
    /// </summary>
    string NumText;

    /// <summary>
    /// 字体精灵
    /// </summary>
    public Sprite[] NumSprites;

    /// <summary>
    /// 类型
    /// </summary>
    public LabelType CurLabelType;
	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
      
	}

    /// <summary>
    /// 设置显示数字
    /// </summary>
    /// <param name="Num"></param>
    List<GameObject> allNum = new List<GameObject>();
    public float totalWidth = 0;
    float lastWidth = 0;
    Vector3 lastPosition = new Vector3(0, 0, 0);
    public void SetNum(string Num)
    {
        totalWidth = 0;
        lastWidth = 0;
        lastPosition = new Vector3(0, 0, 0);
        foreach (Transform child in transform)
        {
            if (CurLabelType == LabelType.ActionCombo)
            {
                if (child.name != "Combo")
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
        NumText = Num;
        allNum.Clear();
        for (int i = 0; i < NumText.Length; i++)
        {
            GameObject numObject = new GameObject();
            numObject.transform.parent = transform;
            numObject.transform.localScale = new Vector3(1, 1, 1);
            numObject.name = NumText[i].ToString();
            SpriteRenderer sr = numObject.AddComponent<SpriteRenderer>();
            sr.sprite = NumSprites[GetIndex(NumText[i].ToString())];
            sr.transform.localPosition = new Vector3(lastPosition.x + lastWidth / 2 + sr.sprite.rect.width / 2, lastPosition.y, lastPosition.z);
            lastWidth = sr.sprite.rect.width;
            lastPosition = sr.transform.localPosition;

            if (NumText[i].ToString() == ".")
            {
                sr.transform.localPosition = new Vector3(sr.transform.localPosition.x, sr.transform.localPosition.y - 7, sr.transform.localPosition.z);
            }
            if (NumText[i].ToString() == "x")
            {
                sr.transform.localPosition = new Vector3(sr.transform.localPosition.x, sr.transform.localPosition.y - 3, sr.transform.localPosition.z);
            }
            if (NumText[i].ToString() == "c")
            {
                sr.transform.localPosition = new Vector3(sr.transform.localPosition.x, sr.transform.localPosition.y - 5, sr.transform.localPosition.z);
            }
            if (NumText[i].ToString() == "h")
            {
                sr.transform.localPosition = new Vector3(sr.transform.localPosition.x, sr.transform.localPosition.y - 4, sr.transform.localPosition.z);
            }
            totalWidth = totalWidth + sr.sprite.rect.width;
            allNum.Add(numObject);
            if (CurLabelType == LabelType.Hit)
            {
                sr.sortingOrder = 1;
                numObject.layer = FightLayer.FightNum;
            }
            else if (CurLabelType == LabelType.ComboLabel)
            {
                sr.sortingOrder = 3;
                numObject.layer = FightLayer.FightNum;
            }
            else if (CurLabelType == LabelType.ComboAddition)
            {
                sr.sortingOrder = 1;
                numObject.layer = FightLayer.BasicFX;
            }
            else if (CurLabelType == LabelType.ActionCombo)
            {
                sr.sortingOrder = 2;
                numObject.layer = FightLayer.FightNum;
            }
            else if (CurLabelType == LabelType.FightInfo)
            {
                sr.sortingOrder = 5;
                numObject.layer = FightLayer.FightBasic;
            }
            else if (CurLabelType == LabelType.PlayerLvl)
            {
                sr.sortingOrder = 6;
                numObject.layer = FightLayer.FightBasic;
            }
            else if (CurLabelType == LabelType.HpUI)
            {
                sr.sortingOrder = 7;
                numObject.layer = FightLayer.FightBasic;
            }
            else if (CurLabelType == LabelType.Damage)
            {
                sr.sortingOrder = 99;
                numObject.layer = LayerHelper.UnitFX;
            }
        }

        foreach (GameObject go in allNum)
        {
            go.transform.localPosition = new Vector3(go.transform.localPosition.x - totalWidth / 2, go.transform.localPosition.y, go.transform.localPosition.z);
        }
    }

    int GetIndex(string s)
    {
        if (s == ".")
        {
            return 11;
        }
        else if (s == "c")
        {
            return 10;
        }
        else if (s == "x")
        {
            return 10;
        }
        else if (s == "h")
        {
            return 10;
        }
        else if (s == "/")
        {
            return 10;
        }
        else
        {
            //Debug.Log("FightLabel.cs   s="+ s);
            return int.Parse(s);
        }

        return 0;
    }

    public void ComboSetNum(int num)
    {
        SetNum(num.ToString());
        foreach (Transform child in transform)
        {
            if (child.name != "Combo")
            {
                Hashtable args = new Hashtable();
                args.Add("scale", new Vector3(2.6f, 2.6f, 2.6f));
                args.Add("time", 0.1f);
                args.Add("oncompletetarget", gameObject);
                args.Add("easetype", iTween.EaseType.easeOutExpo);
                args.Add("oncomplete", "ComboScaleEnd");
                iTween.ScaleTo(child.gameObject, args);
                iTween.FadeFrom(child.gameObject, 0.5f, 0.2f);
            }
        }
    }

    void ComboScaleEnd()
    {
        foreach (Transform child in transform)
        {
            if (child.name != "Combo")
            {
                Hashtable args = new Hashtable();
                args.Add("scale", new Vector3(1, 1, 1));
                args.Add("time", 0.08f);
                args.Add("easetype", iTween.EaseType.easeInExpo);
                iTween.ScaleTo(child.gameObject, args);
            }
        }
    }

    public void HitSetNum(int num)
    {
        if (num == 0)
        {
            //消失
            Hashtable args = new Hashtable();
            args.Add("scale", new Vector3(2.2f, 2.2f, 2.2f));
            args.Add("time", 0.25f);
            args.Add("oncompletetarget", gameObject);
            args.Add("oncomplete", "ScaleEnd");
            iTween.ScaleTo(gameObject, args);
            iTween.FadeTo(gameObject, 0, 0.25f);
        }
        else
        {
            string s = num.ToString() + "h";
            SetNum(s);
            foreach (GameObject go in allNum)
            {
                if (go.name != "h")
                {
                    iTween.FadeFrom(go, 0.3f, 0.16f);
                    Hashtable args = new Hashtable();
                    args.Add("amount", new Vector3(0, 0.04f, 0));
                    args.Add("time", 0.08f);
                    args.Add("oncompletetarget", gameObject);
                    args.Add("oncomplete", "MoveEnd");
                    args.Add("oncompleteparams", go);
                    iTween.MoveBy(go, args);
                }
            }
        }
    }

    void ScaleEnd()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        allNum.Clear();
        transform.localScale = new Vector3(0.78f, 0.78f, 0.78f);
    }

    void MoveEnd(GameObject go)
    {
        Hashtable args = new Hashtable();
        args.Add("amount", new Vector3(0, -0.04f, 0));
        args.Add("time", 0.08f);
        iTween.MoveBy(go, args);
    }
}
