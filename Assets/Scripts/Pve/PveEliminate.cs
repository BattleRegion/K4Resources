using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
public class PveEliminate : PveGameObject
{

    #region 属性

    

    public DungeonEnum.ElementAttributes CurEliminateAttribute;

    public PveEliminate NextLinkEliminate;

    public PveEliminate LastLinkEliminate;
    
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
    #endregion

    public void ShowSpeed(bool show)
    {
        //SpeedRange.gameObject.SetActive(show);
        if (show)
           RenderObject.GetComponent<SpriteRenderer>().sprite = SkillSprites[2];
        else
           ReturnToCommon();
    }

    public void ShowAttack(bool show)
    {
        //AttackRange.gameObject.SetActive(show);
        if (show)
            RenderObject.GetComponent<SpriteRenderer>().sprite = SkillSprites[3];
        else
            ReturnToCommon();
    }
       

    #region 重写
    public override void SetName()
    {
        name = "Eliminate:" + XPosition + "," + YPosition;
    }

    public override void SetOrder()
    {
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 3;
    }
    #endregion

    #region 方法
    public void AttrubuteToRender()
    {
        int randomNum = GetEliElement();
        CurEliminateAttribute = (DungeonEnum.ElementAttributes)randomNum;

        //Debug.Log(CurEliminateAttribute);

        //Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
        RenderObject.layer = GetLayer();

		int index = AttributeToSpriteIndex ();
		
		sr.sprite = this.SpriteList[index];
    }
    /// <summary>
    /// 1， 随机颜色
    /// 2， 引导  标配颜色 
    /// </summary>
    /// <returns></returns>
    public int GetEliElement() {

		int curGuidId = UserManager.CurUserInfo.GuideId;
		if (curGuidId > 41||curGuidId==-1)
		{
			return UnityEngine.Random.Range(1, 5);
		}
		GuiderData gd = ConfigManager.GuiderConfig.GetAllData()[curGuidId - 1];
		if (gd != null && gd.Scene == "Pve"&& PveGameControl. CurDungeonId=="D1_1")
		{
			
			int ty = YPosition;
			if (YPosition > 8) ty = YPosition - 9;
			return ConfigManager.EliminateConfig.GetElementWithXY(gd.Map, XPosition, ty);
		}
		else
		{
			return UnityEngine.Random.Range(1, 5);
		}
    }


    public void ReturnToCommon()
    {
        //Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
		sr.sprite = this.SpriteList[AttributeToSpriteIndex()];
    }
    public void ReturnToCommon_tra() {
        CurEliminateAttribute = DungeonEnum.ElementAttributes.Earth;
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
        sr.sprite = this.SpriteList[AttributeToSpriteIndex()];
    }

    public void SetToPlayerRender()
    {
        CurEliminateAttribute = DungeonEnum.ElementAttributes.Player;
        //Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
		sr.sprite = this.SpriteList[AttributeToSpriteIndex()];
        AnimationHelper.AnimationScaleTo(new Vector3(0.65f, 0.65f, 0.65f), RenderObject.gameObject, iTween.EaseType.easeInExpo, gameObject, "ScaleEnd", 0.1f);
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
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
        if (!enabel)
        {
            sr.color = new Color(0.3f, 0.3f, 0.3f, 1);
        }
        else
        {
            sr.color = new Color(1, 1, 1, 1);
        }
    }

    public bool IsNeighbour(PveEliminate obj)
    {
        if (obj == null)
        {
            Debug.Log("obj=null");
        }
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
    public int CurStep = 0;
    //伤害附加系数
    public void ShowAddition(bool show)
    {
        LinkAdditionLabel.gameObject.SetActive(show);
        if (show)
        {
			int count = 0;

			count = GameControl.CurPathEliminate.Count - 1;
			if(GameControl.CurPathEliminate.Count==0){
				count = GameControl.CurGuideEliId+1 ;
			}
            CurStep = count;
            float addition = 1.0f + count * 0.1f;
            if (addition > ConfigManager.ParamConfig.GetParam().AdditionMax)
            {
                addition = ConfigManager.ParamConfig.GetParam().AdditionMax;
            }

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
			if(GameControl.CurPathEliminate.Count==0){
				chainNumStr = (GameControl.CurGuideEliId+1).ToString();
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
        GameObject UIRoot = GameObject.Find("UI Root");

		PveGameControl gc = UIRoot.GetComponent<PveGameControl>();
		gc.ClearEliminate(this);
        DesCount++;
        if (As != null)
        {
           As.enabled = false;
           As.clip = Resources.Load("Audio/" + GetDesAudioName()) as AudioClip;
           As.enabled = true;
        }
       
        GameObject dis = Instantiate(Resources.Load("PreFabs/FX/Disappear")) as GameObject;
        dis.transform.position = transform.position;
        Line1.SetActive(false);
        Line2.SetActive(false);
        Line3.SetActive(false);
        Line4.SetActive(false);
        Line5.SetActive(false);
        Line6.SetActive(false);
        Line7.SetActive(false);
        Line8.SetActive(false);
        Square.SetActive(false);
        AnimationHelper.AnimationScaleTo(new Vector3(1.7f, 1.7f, 1.7f), RenderObject.gameObject, iTween.EaseType.linear, gameObject, "ScaleRemoveEnd", 0.3f);
        AnimationHelper.AnimationFadeTo(0.2f, RenderObject.gameObject, iTween.EaseType.linear, null, null, 0.3f);
    }

    void ScaleRemoveEnd()
    {
        Destroy(gameObject);
    }
    #endregion

    /// <summary>
    /// 设置技能范围
    /// </summary>
    public Sprite[] SkillSprites;
    public void SetSkillEnabel(bool enabel)
    {
        SetEnabelRender(true);
        if (enabel)
        {
            RenderObject.GetComponent<SpriteRenderer>().sprite = SkillSprites[0];
        }
        else
        {
            RenderObject.GetComponent<SpriteRenderer>().sprite = SkillSprites[1];
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
