using UnityEngine;
using System.Collections;

/// <summary>
/// 接口
/// </summary>
public interface RemoveBlockInter
{
    void AllLinkRemoveEnd();

    void OneFillEnd();
}

public class RemoveBlock : FightElement
{
    #region 属性
    /// <summary>
    /// 消除块枚举
    /// </summary>
    public enum RemoveType
    {
        Earth = 1,
        Fire = 2,
        Wind = 3,
        Water = 4,
        Player = 5
    }

    public enum LinkDirection
    {
        None = 0,
        Up = 10,
        UpRight = 20,
        Right = 30,
        RightDown = 40,
        Down = 50,
        LeftDown= 60,
        Left = 70,
        LeftUp = 80
    }

    /// <summary>
    /// 当前消除块类型
    /// </summary>
    RemoveType curRemoveType;
    public RemoveType CurRemoveType
    {
        get { return curRemoveType; }
        set 
        {
            curRemoveType = value;
        }
    }

    /// <summary>
    /// 被选择时显示的粒子效果对象
    /// </summary>
    public GameObject BeTouchedFxObject;

    public GameObject Line1;
    public GameObject Line2;
    public GameObject Line3;
    public GameObject Line4;
    public GameObject Line5;
    public GameObject Line6;
    public GameObject Line7;
    public GameObject Line8;
    public GameObject curLine;
    public FightLabel RemoveComboAdditionLabel;
    public FightLabel ComboDetailLabel;
    public GameObject ShowTest;
    /// <summary>
    /// 记录上一个连接点
    /// </summary>
    public RemoveBlock LastRemoveBlock;

    /// <summary>
    /// 下一个连接点
    /// </summary>
    public RemoveBlock NextRemoveBlock;

    /// <summary>
    /// 当前连接方向
    /// </summary>
    public LinkDirection CurLinkDirection;

    /// <summary>
    /// 深度
    /// </summary>
    int basicDepth = 1;

    int middleDepth = 2;

    int highestDepth = 3;

    /// <summary>
    /// 已经呗选定
    /// </summary>
    public bool hasBeSelected = false;

    /// <summary>
    /// 接口指针
    /// </summary>
    public RemoveBlockInter Handler;

    /// <summary>
    /// 消除的粒子资源
    /// </summary>
    public GameObject DisAppearResources;

    #endregion

    #region 重写MONO
    // Use this for initialization

    void Start()
    {
        SetName();
        if (XPosition == 0)
        {
            ComboDetailLabel.transform.localPosition = new Vector3(ComboDetailLabel.transform.localPosition.x + 80, ComboDetailLabel.transform.localPosition.y, ComboDetailLabel.transform.localPosition.z);
        }
        if (XPosition == 6)
        {
            ComboDetailLabel.transform.localPosition = new Vector3(ComboDetailLabel.transform.localPosition.x - 80, ComboDetailLabel.transform.localPosition.y, ComboDetailLabel.transform.localPosition.z);
        }
    }
    #endregion

    #region 事件

    #endregion

    #region 效果
    public void BlockEnabel(bool enabel)
    {
        if (!enabel)
        {
            elementSpriteRender.color = new Color(0.3f, 0.3f, 0.3f, 1);
        }
        else
        {
            elementSpriteRender.color = new Color(1, 1, 1, 1);
        }
    }

    public int GetSpriteIndex()
    {
        if (curRemoveType == RemoveType.Earth)
        {
            return 4;
        }
        else if (curRemoveType == RemoveType.Fire)
        {
            return 3;
        }
        else if (curRemoveType == RemoveType.Water)
        {
            return 1;
        }
        else if (curRemoveType == RemoveType.Wind)
        {
            return 0;
        }
        else if (curRemoveType == RemoveType.Player)
        {
            return 2;
        }

        return -1;
    }

    /// <summary>
    /// 选中时效果
    /// </summary>
    /// <param name="enabel"></param>
    public void SelectedFXEffect(bool selected,int curComboNum)
    {
        //光圈
        BeTouchedFxObject.SetActive(selected);
        //缩放
        SelectedScale(selected);
        //数字
        RemoveComboAdditionLabel.gameObject.SetActive(selected);
        //连线
        if (selected == true)
        {
            if (LastRemoveBlock)
            {
                LastRemoveBlock.LinkTo(this);
                LastRemoveBlock.SelectedScale(false);
            }
            //渲染COMBO系数
            //修正位置
            string additionString = (1.0 + (curComboNum * 0.1f)).ToString("0.0");
            RemoveComboAdditionLabel.SetNum("x" + additionString);
            RenderCurComboDetail(curComboNum);
        }
        else
        {
            if (LastRemoveBlock)
            {
                LastRemoveBlock.LinkTo(null);
                LastRemoveBlock.SelectedScale(true);
            }
        }
    }

    /// <summary>
    /// 渲染当前COMBO数
    /// </summary>
    /// <param name="curComboNum"></param>
    public void RenderCurComboDetail(int curComboNum)
    {
        if (LastRemoveBlock != null)
        {
            LastRemoveBlock.ComboDetailLabel.gameObject.SetActive(false);
        }
        ComboDetailLabel.SetNum(curComboNum.ToString()+"c");
        ComboDetailLabel.gameObject.SetActive(true);
        ComboDetailLabel.transform.localScale = new Vector3(1, 1, 1);
        Hashtable args = new Hashtable();
        args.Add("scale", new Vector3(1.3f, 1.3f, 1.3f));
        args.Add("time", 0.3f);
        args.Add("easetype", iTween.EaseType.easeOutExpo);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "CurComboDetailScaleBigEnd");
        iTween.ScaleTo(ComboDetailLabel.gameObject, args);
        iTween.FadeFrom(ComboDetailLabel.gameObject, 0.4f, 0.25f);
        //foreach (Transform t in ComboDetailLabel.transform)
        //{
        //    TweenAlpha.Begin(t.gameObject, 0, 0.2f);
        //    TweenAlpha.Begin(t.gameObject, 0.2f, 1);
        //}
    }

    void CurComboDetailScaleBigEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("scale", new Vector3(0, 0, 0));
        args.Add("time",0.25f);
        args.Add("easetype", iTween.EaseType.easeInExpo);
        iTween.ScaleTo(ComboDetailLabel.gameObject, args);
        iTween.FadeTo(ComboDetailLabel.gameObject, 0, 0.8f);
        //foreach (Transform t in ComboDetailLabel.transform)
        //{
        //    TweenAlpha.Begin(t.gameObject, 0.8f, 0);
        //}
    }

    public void SelectedScale(bool selected)
    {
        if (curRemoveType != RemoveType.Player)
        {
            if (selected)
            {
                elementSpriteRender.sortingOrder = highestDepth;
                iTween.ScaleTo(elementSpriteRender.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
            }
            else
            {
                elementSpriteRender.sortingOrder = middleDepth;
                Hashtable args = new Hashtable();
                args.Add("scale", new Vector3(0.65f, 0.65f, 0.65f));
                args.Add("time", 0.15f);
                args.Add("easetype", iTween.EaseType.easeInExpo);
                args.Add("oncompletetarget",gameObject);
                args.Add("oncomplete", "ScaleEnd");
                iTween.ScaleTo(elementSpriteRender.gameObject, args);
            }
        }
    }

    void ScaleEnd()
    {
        elementSpriteRender.sortingOrder = basicDepth;
        Hashtable args = new Hashtable();
        args.Add("scale", new Vector3(1, 1, 1));
        args.Add("time", 0.4f);
        iTween.ScaleTo(elementSpriteRender.gameObject, args);
    }


    /// <summary>
    /// 连接渲染
    /// </summary>
    public void LinkTo(RemoveBlock block)
    {
        if (block != null)
        {
            curLine = CaculateLinkLine(block);
            curLine.SetActive(true);
        }
        else
        {
            if (curLine != null)
            {
                curLine.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 消除块
    /// </summary>
    public void BlockRemove()
    {
        //Debug.Log("消除"+XPosition.ToString()+","+YPosition.ToString());
        //添加粒子
        elementSpriteRender.sortingOrder = highestDepth;
        GameObject dis = Instantiate(DisAppearResources) as GameObject;
        dis.transform.position = transform.position;
        //缩放消失动画
        LinkTo(null);
        RemoveComboAdditionLabel.gameObject.SetActive(false);
        BeTouchedFxObject.SetActive(false);
        Hashtable args = new Hashtable();
        args.Add("scale", new Vector3(1.6f, 1.6f, 1.6f));
        args.Add("time", 0.25f);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "ScaleRemoveEnd");
        iTween.ScaleTo(elementSpriteRender.gameObject, args);
        iTween.FadeTo(elementSpriteRender.gameObject, 0, 0.25f);
    }

    void ScaleRemoveEnd()
    {
        //NextRemoveBlock = null;
        //if (NextRemoveBlock != null)
        //{
        //    Debug.Log("消除下一个" + NextRemoveBlock.name);
        //    NextRemoveBlock.BlockRemove();
        //    NextRemoveBlock = null;
        //}
        //else
        //{
        //    Handler.AllLinkRemoveEnd();
        //}
    }

    /// <summary>
    /// 计算采用哪个方向的连接线
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    GameObject CaculateLinkLine(RemoveBlock block)
    {
        if (block.XPosition == XPosition + 1 && block.YPosition == YPosition)
        {
            CurLinkDirection = LinkDirection.Right;
            return Line1;
        }
        else if (block.XPosition == XPosition + 1 && block.YPosition == YPosition + 1)
        {
            CurLinkDirection = LinkDirection.UpRight;
            return Line2;
        }
        else if (block.XPosition == XPosition && block.YPosition == YPosition + 1)
        {
            CurLinkDirection = LinkDirection.Up;
            return Line3;
        }
        else if (block.XPosition == XPosition - 1 && block.YPosition == YPosition + 1)
        {
            CurLinkDirection = LinkDirection.LeftUp;
            return Line4;
        }
        else if (block.XPosition == XPosition - 1 && block.YPosition == YPosition)
        {
            CurLinkDirection = LinkDirection.Left;
            return Line5;
        }
        else if (block.XPosition == XPosition - 1 && block.YPosition == YPosition - 1)
        {
            CurLinkDirection = LinkDirection.LeftDown;
            return Line6;
        }
        else if (block.XPosition == XPosition && block.YPosition == YPosition - 1)
        {
            CurLinkDirection = LinkDirection.Down;
            return Line7;
        }
        else if (block.XPosition == XPosition + 1 && block.YPosition == YPosition - 1)
        {
            CurLinkDirection = LinkDirection.RightDown;
            return Line8;
        }
        return null;
    }

    /// <summary>
    /// 填充时重设位置
    /// </summary>
    public void ResertRemoveBlockPosition()
    {
        Vector3 moveTo = GetBasicPosition(XPosition, YPosition);
        Hashtable args = new Hashtable();
        args.Add("position", moveTo);
        args.Add("time",0.5f);
        args.Add("islocal", true);
        args.Add("easetype", iTween.EaseType.easeInExpo);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "MoveEnd");
        iTween.MoveTo(gameObject, args);
    }

    void MoveEnd()
    {
        Handler.OneFillEnd();
    }

    /// <summary>
    /// 设置名字
    /// </summary>
    public override void SetName()
    {
        name = CurRemoveType.ToString() + XPosition.ToString() + "," + YPosition.ToString();
    }

    public void SetTestShow(bool show)
    {
        ShowTest.SetActive(show);
    }
    #endregion
}
