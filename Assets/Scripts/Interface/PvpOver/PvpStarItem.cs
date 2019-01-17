using UnityEngine;
using System;
using System.Collections;

public class PvpStarItem : MonoBehaviour 
{
	public static readonly int ADD = 1;
	public static readonly int ZERO = 0;
	public static readonly int SUB = -1;

	public int star;
	private UISprite icon;

	void Awake()
	{
		this.icon = this.GetComponentInParent<UISprite> ();
	}

	/// <summary>
	/// 设置星级
	/// </summary>
	/// <param name="star">Star.</param>
	public void InitData(int star)
	{
		if(star >= this.star)
		{
			if(this.icon != null)
			{
				this.icon.alpha = 1.0f;
				this.icon.spriteName = "pvp_star";
			}
		}else{
			if(this.icon != null) this.icon.spriteName = "";
		}
	}

	/// <summary>
	/// 更新动画
	/// </summary>
	/// <param name="star">Star.</param>
	private Action callback;
	public void ChangeData(int effectType, int star, Action callback)
	{
		this.callback = callback;
		// 如果没有激活状态
		if(!this.gameObject.activeSelf)
		{
			if(this.callback != null) this.callback();
			return;
		}

		GameObject effectObject = null;
		// 如果是增加
		if(effectType == PvpStarItem.ADD)
		{
			effectObject = GameObject.Instantiate(Resources.Load("PreFabs/FX/UiFx_1")) as GameObject;
		}
		else if(effectType == PvpStarItem.SUB)
		{
			// 如果是降低
			effectObject = GameObject.Instantiate(Resources.Load("PreFabs/FX/UiFx_2")) as GameObject;
		}

		if(effectType != PvpStarItem.ZERO)
		{
			if(effectObject != null)
			{
				effectObject.transform.parent = this.transform;
				effectObject.transform.localScale = new Vector3(1f, 1f, 1f);
				effectObject.transform.localPosition = Vector3.zero;
			}

			if(effectType == SUB)
			{
				AnimationHelper.AnimationValueTo (this.gameObject, 1f, 0f, 1f, "onValueToUpdate", this.gameObject, "OnValueToCallback", this.gameObject, star);
			}
			else if(effectType == ADD)
			{
				this.InitData (star);
				AnimationHelper.AnimationValueTo (this.gameObject, 0f, 1f, 1f, "onValueToUpdate", this.gameObject, "OnValueFromCallback", this.gameObject, null);
			}
		}else
		{
			if(this.callback != null) this.callback();
		}
	}

	private void onValueToUpdate(float value)
	{
		this.icon.alpha = value;
	}

	private void OnValueToCallback(int star)
	{
		this.InitData (star);
		if(this.callback != null) this.callback();
	}

	private void OnValueFromCallback()
	{
		if(this.callback != null) this.callback();
	}
}
