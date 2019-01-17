using UnityEngine;
using System.Collections;

public class FightInfo : MonoBehaviour
{
    #region 属性
    public FightLabel FloorLabel;
    public FightLabel GoldLabel;
    #endregion

    #region 重写MONO
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }
    #endregion

    #region 方法
    public void SetFloor(int curFloor,int totalFloor)
    {
        FloorLabel.SetNum(curFloor.ToString() + "/" + totalFloor.ToString());
        FloorLabel.transform.localPosition = new Vector3(40 + FloorLabel.totalWidth/2,0,0);
    }

    public void SetGold(int gold)
    {
        GoldLabel.SetNum(gold.ToString());
        GoldLabel.transform.localPosition = new Vector3( 20 + GoldLabel.totalWidth / 2, 0,0);
    }
    #endregion
}
