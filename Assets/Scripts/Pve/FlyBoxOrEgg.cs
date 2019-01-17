using UnityEngine;
using System.Collections;

public class FlyBoxOrEgg : MonoBehaviour {

	// Use this for initialization
    public string ResName;
    GameObject EndObj;
    PveGameControl GameControl;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void StartFly(PveGameControl pgc)
    {
        GameControl = pgc;
        Invoke("SFlyBoxOrEgg", 1);  
    }
    void SFlyBoxOrEgg()
    {
        if (ResName == "egg")
        {
            EndObj=PveGameControl.CurShakObj_egg;
        }
        else if (ResName == "box")
        {
            EndObj = PveGameControl.CurShakObj_box;
        }
        AnimationHelper.AnimationMoveTo(EndObj.transform.position, gameObject, iTween.EaseType.linear, gameObject, "DesCurBoxOrEgg", 0.4f);
    }
    void DesCurBoxOrEgg()
    {
        
        if (ResName == "egg")
        {
            GameControl.PvePlayerInfo.setEggInfoNum(PveGameControl.CurEggNum);
        }
        else if (ResName == "box")
        {
            GameControl.PvePlayerInfo.setBoxInfoNum(PveGameControl.CurBoxNum);
        }
        iTween.ShakeScale(EndObj, new Vector3(1.5f, 1.5f, 1.5f), 0.5f);
        Destroy(gameObject);
    }
}
