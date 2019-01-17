using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PopTips : MonoBehaviour {
    public GameObject Btn_confirm;
    public GameObject Btn_cancel;
    public GameObject TipsContent;
    Action Callback_con;

	public void SetTips(string str,Action callback_confirm){
       TextMesh tm= TipsContent.transform.GetComponent<TextMesh>();
       tm.text = str;
       Callback_con = callback_confirm;

       UIEventListener.Get(Btn_confirm).onClick = (go) =>
        {
            if (Callback_con != null) Callback_con();
            MissGameobject();
        };
       UIEventListener.Get(Btn_cancel).onClick = (go) =>
        {
            MissGameobject();
        };   
    }
    public void MissGameobject()
    {
        gameObject.SetActive(false);
    }
}
