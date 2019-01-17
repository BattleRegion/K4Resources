using UnityEngine;
using System;
using System.Collections;

public class PvpIconItem : MonoBehaviour 
{
	private UITexture icon;

	void Awake()
	{
		this.icon = this.GetComponentInParent<UITexture> ();
	}

	public void InitData(int level)
	{
		this.icon.mainTexture = Tools.GetPvPIcon (level);
	}

	public void ChangeData(int oriLevel, int newLevel)
	{
		GameObject effectObject = null;
		// 如果不等于零
		if(newLevel < oriLevel)
		{
			effectObject = GameObject.Instantiate(Resources.Load("PreFabs/FX/UiFx_4")) as GameObject;
		}else if(newLevel > oriLevel)
		{
			effectObject = GameObject.Instantiate(Resources.Load("PreFabs/FX/UiFx_3")) as GameObject;
		}
		
		if(effectObject != null)
		{
			effectObject.transform.parent = this.transform;
			effectObject.transform.localScale = new Vector3(1f, 1f, 1f);
			effectObject.transform.localPosition = Vector3.zero;
			
			AnimationHelper.AnimationValueTo (this.gameObject, 1f, 0f, 1f, "onValueToUpdate", this.gameObject, "OnValueToCallback", this.gameObject, newLevel);
		}
	}

	private void onValueToUpdate(float value)
	{
		this.icon.alpha = value;
	}

	private void OnValueToCallback(int newLevel)
	{
		this.InitData (newLevel);
		AnimationHelper.AnimationValueTo (this.gameObject, 0f, 1f, 1f, "onValueToUpdate", this.gameObject, "OnValueFromCallback", this.gameObject, null);
	}

	private void OnValueFromCallback()
	{

	}
}
