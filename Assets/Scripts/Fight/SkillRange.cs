using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SkillRange : MonoBehaviour {
	// Use this for initialization
	void Start () 
    {
        //BalinAnimation();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BalinAnimation()
    {
        //Hashtable args = new Hashtable();
        //args.Add("alpha", 0);
        //args.Add("time", 0.6f);
        //args.Add("looptype", iTween.LoopType.pingPong);
        //args.Add("name", "rangeAnimation" + CurBlock.XPosition.ToString() + CurBlock.YPosition.ToString());
        //iTween.FadeFrom(gameObject, args);
    }


    /// <summary>
    /// 显示范围
    /// </summary>
    /// <param name="show"></param>
    public void ShowRange(bool show)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(show);
        }
    }


    
}
