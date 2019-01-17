using UnityEngine;
using System.Collections;

public class HpUI : MonoBehaviour {

    public InfoLabel HpLabel;
    public GameObject HpValue;
    public void SetCurHpShow(float curHp,float totalHp)
    {
        if (curHp < 0)
        {
            curHp = 0;
        }
        string hpString = ((int)curHp).ToString() + "/" + ((int)totalHp).ToString();
		if(HpLabel != null)
		{
	        HpLabel.SetNum(hpString);
	        HpLabel.transform.localPosition = new Vector3(150 - HpLabel.TotalWidth/2, 6, 0);
		}
		if(HpValue != null)
		{
        	AnimationHelper.AnimationScaleTo(new Vector3(curHp / totalHp, 1, 1), HpValue.gameObject, iTween.EaseType.easeOutExpo, null, null, 0.3f);
		}
    }
}
