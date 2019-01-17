using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
public class PvpEliminate : PvpGameObject
{
	
	#region 属性

	public DungeonEnum.ElementAttributes CurEliminateAttribute;
	
	public PvpEliminate NextLinkEliminate;
	public PvpEliminate LastLinkEliminate;
	
	public float Addition = 1f;

	public List<Sprite> SpriteList;
	
	///连接渲染对象
	public GameObject Line1;
	public GameObject Line2;
	public GameObject Line3;
	public GameObject Line4;
	public GameObject Line5;
	public GameObject Line6;
	public GameObject Line7;
	public GameObject Line8;
	public GameObject Square;

	public int EliColor;
	public int EliIndex;

	/// <summary>
	/// 怪物状态
	/// </summary>
	public bool MonsterStatus;
	
	/// <summary>
	/// 连接LABEL
	/// </summary>
	public LinkAdditionLabel LinkAdditionLabel;
	
	/// <summary>
	/// CHAIN LABEL
	/// </summary>
	public LinkChainLabel LinkChainLabel;
	
	public AudioSource As;
	
	public SpriteRenderer SpeedRange;
	public SpriteRenderer AttackRange;

	private SpriteRenderer objectRender;

	public Material defaultMaterial;
	#endregion
	
	public void ShowSpeed(bool show)
	{
		SpeedRange.gameObject.SetActive(show);
	}
	
	public void ShowAttack(bool show)
	{
		AttackRange.gameObject.SetActive(show);
	}
	
	#region 重写
	public override void SetName()
	{
		name = "Eliminate:" + XPosition + "," + YPosition;
	}
	
	public override void SetOrder()
	{
		if(this.objectRender != null)
		{
			this.objectRender.sortingOrder = 3;
		}
	}
	#endregion
	
	#region 方法
	// oriPos 如果为 -1，
	// oriPos 如果为 正数
	public void AttrubuteToRender(int oriPos = -1, int eliColor = -1)
	{
		// 如果格子颜色为 -1，通过随机算法计算，否则直接设置格子颜色
		if(eliColor == -1)
		{
			this.EliColor = GetEliElement (oriPos);
		}else{
			this.EliColor = eliColor;
		}

		int randomNum = this.EliColor;
		CurEliminateAttribute = (DungeonEnum.ElementAttributes)randomNum;
		//Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
		if(this.objectRender == null)
		{
			this.objectRender = RenderObject.AddComponent<SpriteRenderer>();
			this.SetOrder ();
		}
		RenderObject.layer = GetLayer();
		this.EliIndex = AttributeToSpriteIndex ();
		this.objectRender.sprite = this.SpriteList[this.EliIndex];
	}

	/// <summary>
	/// 重置下落格子的值
	/// </summary>
	public void ResetAttributeToRender(int color, int index)
	{
		this.EliColor = color;
		this.EliIndex = index;
		
		int randomNum = this.EliColor;
		CurEliminateAttribute = (DungeonEnum.ElementAttributes)randomNum;
		
		//Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
		if(this.objectRender != null)
		{
			this.objectRender.sprite = this.SpriteList[index];
		}
		RenderObject.layer = GetLayer();
	}

	/// <summary>
	/// 重置玩家角色的格子
	/// </summary>
	public void ResetRenderPlayerToElement()
	{
		this.ResetAttributeToRender (this.EliColor, this.EliIndex);
	}

	/// <summary>
	/// 设置怪物脚底状态
	/// </summary>
	/// <param name="monsterStatus">If set to <c>true</c> monster status.</param>
	public void ChangeMonsterStatus(bool monsterStatus)
	{
		this.MonsterStatus = monsterStatus;
		if(this.MonsterStatus)
		{
			this.SetEnabelRender(true);
		}else{
			this.SetEnabelRender(false);
		}
	}

	/// <summary>
	/// 1， 随机颜色
	/// 2， 引导  标配颜色 
	/// </summary>
	/// <returns></returns>
	public int GetEliElement(int oriPos = -1) 
	{
		int ty = YPosition;
		if(oriPos == -1)
		{
			if (YPosition > 8) ty = YPosition - 9;
		}else{
			ty = oriPos;
		}
		int color = ResetColor(XPosition,ty);
		return color;
	}

	private JsonObject GetDataByPosition(int x, int y)
	{
		JsonArray ja = GameControl.MapJsonData;           
		for (int i = 0; i < ja.Count; i++) 
		{
			JsonObject jo = (JsonObject) ja[i];
			
			if (jo["x"].ToString() == x.ToString() && jo["y"].ToString() == y.ToString())
			{
				return jo;
			}
		}
		return null;
	}
	
	int ResetColor(int x, int y)
	{      
		int length = GameControl.PvpColors.Length;
		int roundId = GameControl.PvpRounds;
        int tableId = GameControl.PvpTableID;

		if (GameControl.EliminateFirstStatus)
		{
			JsonArray ja = GameControl.MapJsonData;           
			for (int i = 0; i < ja.Count; i++) 
			{
				JsonObject jo = (JsonObject) ja[i];
				
				if (jo["x"].ToString() == x.ToString() && jo["y"].ToString() == y.ToString())
				{
					return int.Parse(jo["color"].ToString() == "0" ? jo["original"].ToString() : jo["color"].ToString());
				}
			}
		}

        int r = (int)(Math.Log(roundId + 1) * 100000),
           m = Math.Abs((int)(Math.Sin(x) * 100000)),
           n = Math.Abs((int)(Math.Cos(y) * 100000));

        int index = (m + n + r * tableId) % length;
	
		int color = GameControl.PvpColors[index] - '0';
		
		return color;
	}
	
	public void ReturnToCommon()
	{
		//Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
		if(this.objectRender != null)
		{
			if(this.hiddenAlpha > 0f)
			{
				this.objectRender.sprite = this.SpriteList[AttributeToSpriteIndex()];
			}else{
				this.objectRender.sprite = this.SpriteList[this.EliIndex];
			}
		}
	}

	private float hiddenAlpha;
	public void SetToPlayerRender(float hiddenAlpha = 1.0f)
	{
		this.hiddenAlpha = hiddenAlpha;

		CurEliminateAttribute = DungeonEnum.ElementAttributes.Player;
		// 如果不隐藏
		if(hiddenAlpha > 0f)
		{
			//Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
			if(this.objectRender != null)
			{
				this.objectRender.sprite = this.SpriteList[AttributeToSpriteIndex()];
			}
			AnimationHelper.AnimationScaleTo(new Vector3(0.65f, 0.65f, 0.65f), RenderObject.gameObject, iTween.EaseType.easeInExpo, gameObject, "ScaleEnd", 0.1f);
		}
	}
	
	/// <summary>
	/// 选中时的缩放效果
	/// </summary>
	/// <param name="selected"></param>
	public void SelectScale(bool selected)
	{
		if (CurEliminateAttribute != DungeonEnum.ElementAttributes.Player)
		{
			if (selected)
			{
				AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), RenderObject.gameObject, iTween.EaseType.easeInExpo, null, null, 0.08f);
			}
			else
			{
				AnimationHelper.AnimationScaleTo(new Vector3(0.65f, 0.65f, 0.65f), RenderObject.gameObject, iTween.EaseType.easeInExpo, gameObject, "ScaleEnd", 0.1f);
			}
		}
	}
	
	void ScaleEnd()
	{
		AnimationHelper.AnimationScaleTo(new Vector3(1f, 1f, 1f), RenderObject.gameObject, iTween.EaseType.linear, null, null, 0.15f);
	}
	
	private int AttributeToSpriteIndex()
	{
		if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Earth)
		{
			return 1;
		}
		else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Fire)
		{
			return 6;
		}
		else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Water)
		{
			return 2;
		}
		else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Wind)
		{
			return 0;
		}
		else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Player)
		{
			return 3;
		}
		return -1;
	}
	
	/// <summary>
	/// 重设位置的动画
	/// </summary>
	public IEnumerator ResertPositionAnimation(float speed, float delay, string endActiong)
	{
		yield return new WaitForSeconds(delay);
		NoDelayResertPositionAnimation(speed, endActiong);
	}
	
	public void NoDelayResertPositionAnimation(float speed, string endActiong)
	{
		SetName();
		AnimationHelper.AnimationMoveToSpeed(CaculateRealPosition(XPosition, YPosition), gameObject, iTween.EaseType.easeInCubic, GameControl.gameObject, endActiong, speed);
	}
	
	public void SetEnabelRender(bool enabel)
	{
		if(this.objectRender != null)
		{
			if (!enabel)
			{
				this.objectRender.color = new Color(0.3f, 0.3f, 0.3f, 1);
			}
			else
			{
				this.objectRender.color = new Color(1, 1, 1, 1);
			}
		}
	}
	
	public bool IsNeighbour(PvpEliminate obj)
	{
		if ((obj.XPosition == XPosition - 1 && obj.YPosition == YPosition) ||
		    (obj.XPosition == XPosition - 1 && obj.YPosition == YPosition + 1) ||
		    (obj.XPosition == XPosition - 1 && obj.YPosition == YPosition - 1) ||
		    (obj.XPosition == XPosition + 1 && obj.YPosition == YPosition) ||
		    (obj.XPosition == XPosition + 1 && obj.YPosition == YPosition + 1) ||
		    (obj.XPosition == XPosition + 1 && obj.YPosition == YPosition - 1) ||
		    (obj.XPosition == XPosition && obj.YPosition == YPosition - 1) ||
		    (obj.XPosition == XPosition && obj.YPosition == YPosition + 1))
		{
			return true;
		}
		return false;
	}
	
	public void LinkToNextRender()
	{
		As.enabled = false;
		As.clip = Resources.Load("Audio/" + GetAudioName()) as AudioClip;
		As.enabled = true;
		DungeonEnum.FaceDirection direction = GetTargetDirection(NextLinkEliminate);
		GameObject lineObject = CaculateLine(direction);
		lineObject.SetActive(true);
		NextLinkEliminate.Square.SetActive(true);
		NextLinkEliminate.SelectScale(true);
		NextLinkEliminate.ShowAddition(true);
		NextLinkEliminate.SetChain(true);
		SetChain(false);
		SelectScale(false);
	}
	//伤害附加系数
    public int CurStep = 0;
	public void ShowAddition(bool show)
	{
		LinkAdditionLabel.gameObject.SetActive(show);
		if (show)
		{
			int count = 0;
			
			count = GameControl.CurPathEliminate.Count - 1;

			if(GameControl.CurPathEliminate.Count==0)
			{
				//count = GameControl_pvp.CurGuideEliId+1 ;
			}
			float addition = 1.0f + count * 0.1f;
            if (addition > ConfigManager.ParamConfig.GetParam().AdditionMax)
            {
                addition = ConfigManager.ParamConfig.GetParam().AdditionMax;
            }
            CurStep = count;
			Addition = addition;
			string additionStr = addition.ToString();
			if (addition == 1 || addition == 2 || addition == 3 || addition == 4 || addition == 5 || addition == 6)
			{
				additionStr = addition.ToString() + ".0";
			}
			LinkAdditionLabel.SetNum("x" + additionStr);
		}
	}
	
	public void SetChain(bool show)
	{
		if (XPosition == 0)
		{
			LinkChainLabel.transform.localPosition = new Vector3(75, LinkChainLabel.transform.localPosition.y, LinkChainLabel.transform.localPosition.z);
		}
		else if (XPosition == 6)
		{
			LinkChainLabel.transform.localPosition = new Vector3(-80, LinkChainLabel.transform.localPosition.y, LinkChainLabel.transform.localPosition.z);
		}
		LinkChainLabel.gameObject.SetActive(show);
		if (show)
		{
			LinkChainLabel.gameObject.SetActive(true);
			string chainNumStr = "";
			
			chainNumStr = (GameControl.CurPathEliminate.Count - 1).ToString();
			if(GameControl.CurPathEliminate.Count==0)
			{
				//	chainNumStr = (GameControl_pvp.CurGuideEliId+1).ToString();
			}
			string chainStr = chainNumStr + "c";
			LinkChainLabel.SetNum(chainStr);
		}
	}
	
	
	public void UnLinkFromLastRender(bool NeedAudio)
	{
		if (NeedAudio)
		{
			As.enabled = false;
			As.clip = Resources.Load("Audio/" + GetAudioName()) as AudioClip;
			As.enabled = true;
		}
		if (Line1 == null)
		{
			Debug.Log("pveEliminate  line1="+Line1);
			return;
		}
		Line1.SetActive(false);
		Line2.SetActive(false);
		Line3.SetActive(false);
		Line4.SetActive(false);
		Line5.SetActive(false);
		Line6.SetActive(false);
		Line7.SetActive(false);
		Line8.SetActive(false);
		if (NextLinkEliminate)
		{
			NextLinkEliminate.Square.SetActive(false);
			NextLinkEliminate.SelectScale(false);
			NextLinkEliminate.ShowAddition(false);
			NextLinkEliminate.SetChain(false);
			
			if (this != GameControl.CurCharacterEliminate&&NeedAudio == true)
			{
				SetChain(true);
			}
		}
	}
	
	/// <summary>
	/// 更具方向获取连线
	/// </summary>
	/// <returns></returns>
	GameObject CaculateLine(DungeonEnum.FaceDirection direction)
	{
		if (direction == DungeonEnum.FaceDirection.Down)
		{
			return Line7;
		}
		else if (direction == DungeonEnum.FaceDirection.Left)
		{
			return Line5;
		}
		else if (direction == DungeonEnum.FaceDirection.LeftDown)
		{
			return Line6;
		}
		else if (direction == DungeonEnum.FaceDirection.LeftUp)
		{
			return Line4;
		}
		else if (direction == DungeonEnum.FaceDirection.Right)
		{
			return Line1;
		}
		else if (direction == DungeonEnum.FaceDirection.RightDown)
		{
			return Line8;
		}
		else if (direction == DungeonEnum.FaceDirection.Up)
		{
			return Line3;
		}
		else if (direction == DungeonEnum.FaceDirection.UpRight)
		{
			return Line2;
		}
		return Line1;
	}
	
	public static int DesCount = 0;
	/// <summary>
	/// 块消除
	/// </summary>
	public void BlockEliminateRender()
	{
		if(this == null) return;

		GameObject UIRoot = GameObject.Find("UI Root");
		
		PvpGameControl gc = UIRoot.GetComponent<PvpGameControl>();
		gc.ClearEliminate(this);
		DesCount++;
		if (As != null)
		{
			As.enabled = false;
			As.clip = Resources.Load("Audio/" + GetDesAudioName()) as AudioClip;
			As.enabled = true;
		}

		if(!PvpGameObjectManager.CACHE_STATUS)
		{
			GameObject dis = Instantiate(Resources.Load("PreFabs/FX/Disappear")) as GameObject;
			if(this.transform != null) dis.transform.position = transform.position;
			if(Line1 != null) Line1.SetActive(false);
			if(Line2 != null) Line2.SetActive(false);
			if(Line3 != null) Line3.SetActive(false);
			if(Line4 != null) Line4.SetActive(false);
			if(Line5 != null) Line5.SetActive(false);
			if(Line6 != null) Line6.SetActive(false);
			if(Line7 != null) Line7.SetActive(false);
			if(Line8 != null) Line8.SetActive(false);
			if(Square != null) Square.SetActive(false);
			GameObject.Destroy(this.gameObject);
		}
		else
		{
			PvpGameObjectManager.Create("PreFabs/FX/Disappear", (GameObject dis)=>
			{
				//GameObject dis = Instantiate(Resources.Load("PreFabs/FX/Disappear")) as GameObject;
				if(this.transform != null) dis.transform.position = transform.position;
				if(Line1 != null) Line1.SetActive(false);
				if(Line2 != null) Line2.SetActive(false);
				if(Line3 != null) Line3.SetActive(false);
				if(Line4 != null) Line4.SetActive(false);
				if(Line5 != null) Line5.SetActive(false);
				if(Line6 != null) Line6.SetActive(false);
				if(Line7 != null) Line7.SetActive(false);
				if(Line8 != null) Line8.SetActive(false);
				if(Square != null) Square.SetActive(false);
				
				this.ScaleRemoveEnd();
				//AnimationHelper.AnimationScaleTo(new Vector3(1.7f, 1.7f, 1.7f), RenderObject.gameObject, iTween.EaseType.linear, gameObject, "ScaleRemoveEnd", 0.3f);
				//AnimationHelper.AnimationFadeTo(0.2f, RenderObject.gameObject, iTween.EaseType.linear, null, null, 0.3f);
			});
		}
	}

	public bool destoryStatus;
	void ScaleRemoveEnd()
	{
		// 设置销毁状态，缓存用
		this.destoryStatus = true;
		// 隐藏伤害加成
		this.ShowAddition (false);

		if(this.As != null)
		{
			this.As.enabled = false;
			this.As.clip = null;
		}

		if(this.RenderObject != null) this.RenderObject.transform.localScale = new Vector3 (1f, 1f, 1f);
		if(this.gameObject != null) this.gameObject.SetActive (false);
		GameObject.Destroy (this.objectRender);
		this.GameControl.CacheEliminateList.Add (this);
		//AnimationHelper.AnimationFadeTo(1f, RenderObject.gameObject, iTween.EaseType.linear, this.gameObject, "ScaleRemoveEndCallback", 0.01f);
	}

	/*private void ScaleRemoveEndCallback()
	{
		this.gameObject.SetActive (false);
		GameObject.Destroy (this.objectRender);
		this.GameControl.CacheEliminateList.Add (this);
	}*/
	#endregion
	
	/// <summary>
	/// 设置技能范围
	/// </summary>
	public Sprite[] SkillSprites;
	public void SetSkillEnabel(bool enabel)
	{
		SetEnabelRender(true);
		if(this.objectRender != null)
		{
			if (enabel)
			{
				this.objectRender.sprite = SkillSprites[0];
			}
			else
			{
				this.objectRender.sprite = SkillSprites[1];
			}
		}
	}
	
	#region 声音
	string GetAudioName()
	{
		string countS;
		int count = 0;
		count = GameControl.CurPathEliminate.Count - 1;
		countS = count.ToString();
		if (count < 10)
		{
			countS = "0" + countS;
		}
		if (count > 17)
		{
			countS = "17";
		}
		return "se_step_plan_" + countS;
	}
	
	
	string GetDesAudioName()
	{
		string countS;
		int count = DesCount;
		countS = count.ToString();
		if (count < 10)
		{
			countS = "0" + countS;
		}
		if (count > 17)
		{
			countS = "17";
		}
		return "se_step_" + countS;
	}
	#endregion
	
}
