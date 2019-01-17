using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneControl : MonoBehaviour {

    public SetUser MainS;

	// Use this for initialization
	void Start () {
        RenderPlayerInfo();
	}

	// Update is called once per frame
	void Update () {
                         
    }
  

    #region 主界面渲染
    void RenderPlayerInfo()
    {
        int exp = UserManager.CurUserInfo.Exp;
        int totalExp = UserManager.CurUserInfo.CurLevelExp;
        int energy = UserManager.CurUserInfo.Energy;
        int maxEnergy = UserManager.CurUserInfo.EnergyLimit;
        int gold = UserManager.CurUserInfo.Gold;
        int diamonds = UserManager.CurUserInfo.Diamond;
        int level = UserManager.CurUserInfo.Level;
        string nickName = UserManager.CurUserInfo.NickName;
        MainS.SetUserInfo(exp, totalExp, energy, maxEnergy, nickName, gold, diamonds, level);
       
        GuiderLocal.MainSceneGuid();      
    }
    #endregion
}
