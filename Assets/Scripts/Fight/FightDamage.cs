using UnityEngine;
using System.Collections;

public class FightDamage : MonoBehaviour {

    public FightLabel DamageValueLabel;

	// Use this for initialization
	void Start () {
        //Invoke("delay", 2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Delay()
    {
        Destroy(gameObject);
    }


    public void DamageShow(string damageValue,DungeonEnum.ElementAttributes attribute,bool crit=false,float pr=1f)
    {
        Color damageColor = new Color(255.0f/255.0f,73.0f/255.0f,80.0f/255.0f);

        if (attribute == DungeonEnum.ElementAttributes.None)
        {
            //白色
            damageColor = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
        }
        else if (attribute == DungeonEnum.ElementAttributes.Earth)
        {
            damageColor = new Color(106 / 255.0f, 243.0f / 255.0f, 132.0f / 255.0f);
        }
        else if (attribute == DungeonEnum.ElementAttributes.Fire)
        {
            damageColor = new Color(255.0f / 255.0f, 73.0f / 255.0f, 80.0f / 255.0f);
        }
        else if (attribute == DungeonEnum.ElementAttributes.Water)
        {
            damageColor = new Color(113 / 255.0f, 166 / 255.0f, 237 / 255.0f);
        }
        else if (attribute == DungeonEnum.ElementAttributes.Wind)
        {
            damageColor = new Color(250 / 255.0f, 252 / 255.0f, 80.0f / 255.0f);
        }


        if (crit)
        {
            //暴击 橙色
            damageColor = new Color(250.0f / 255.0f, 128.0f / 255.0f, 10.0f / 255.0f);            
        }
        else{
            //Debug.Log(" pr = "+pr);
            if (pr < 0.99f)
            {
                //克 灰色
                damageColor = new Color(128.0f / 255.0f, 138.0f / 255.0f, 135.0f / 255.0f);   
            }
            else if (pr > 1.01f)
            {
                //被克                
            }
            else
            {
                //pr=1 非可与不可 白色
                damageColor = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
            }
        }
        

        DamageValueLabel.SetNum(damageValue);
        foreach (Transform t in DamageValueLabel.transform)
        {
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();                 
            sr.color = damageColor;            
        }
        //动画
        iTween.FadeFrom(gameObject, 0.5f, 0.2f);
        if (crit)
        {
            iTween.ScaleTo(gameObject, new Vector3(4.7f, 4f, 4f), 0.5f);
        }
        else
        {
            iTween.ScaleTo(gameObject, new Vector3(1.7f, 2f, 2f), 0.3f);
        }
       
        Hashtable args = new Hashtable();
        args.Add("time", 0.15f);
        args.Add("islocal", true);
        args.Add("position",new Vector3(transform.localPosition.x, transform.localPosition.y+75, transform.localPosition.z));
        args.Add("easetype", iTween.EaseType.linear);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "DownEnd1");
        iTween.MoveTo(gameObject, args);
    }

    void DownEnd1()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.15f);
        args.Add("islocal", true);
        args.Add("position",new Vector3(transform.localPosition.x, transform.localPosition.y-80, transform.localPosition.z));
        args.Add("easetype", iTween.EaseType.linear);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "DownEnd");
        iTween.MoveTo(gameObject, args);
    }

    void DownEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.1f);
        args.Add("scale", new Vector3(3f, 2f, 2f));
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "ScaleBigEnd");

        Hashtable args1 = new Hashtable();
        args1.Add("time", 0.1f);
        args1.Add("position", new Vector3(transform.localPosition.x, transform.localPosition.y - 15, transform.localPosition.z));
        args1.Add("islocal", true);
        iTween.MoveTo(gameObject, args1);
        iTween.ScaleTo(gameObject, args);
    }

    void ScaleBigEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.1f);
        args.Add("scale", new Vector3(2f, 2f, 2f));
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "AllEnd");
        Hashtable args1 = new Hashtable();
        args1.Add("time", 0.1f);
        args1.Add("position", new Vector3(transform.localPosition.x, transform.localPosition.y + 15, transform.localPosition.z));
        args1.Add("islocal", true);
        iTween.MoveTo(gameObject, args1);
        iTween.ScaleTo(gameObject, args);
    }

    void AllEnd()
    {
        iTween.FadeTo(gameObject, 0, 0.2f);
        Invoke("Delay", 0.2f);
    }
}
