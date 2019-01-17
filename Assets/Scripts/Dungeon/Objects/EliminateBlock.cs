using UnityEngine;
using System.Collections;

public class EliminateBlock : DungeonObject
{
    #region 属性
    /// <summary>
    /// 当前消除块属性
    /// </summary>
    public DungeonEnum.ElementAttributes CurEliminateAttribute;

    /// <summary>
    /// 当呗组成路径时下一块消除块
    /// </summary>
    public EliminateBlock NextEliminateBlock;

    /// <summary>
    /// 当被组成路劲时上一块消除块
    /// </summary>
    public EliminateBlock LastEliminateBlock;

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
    /// 深度
    /// </summary>
    int basicDepth = 1;

    int highestDepth = 3;

    /// <summary>
    /// 连接LABEL
    /// </summary>
    public LinkAdditionLabel LinkAdditionLabel;

    /// <summary>
    /// CHAIN LABEL
    /// </summary>
    public LinkChainLabel LinkChainLabel;

    public Sprite[] SkillSprites;

    public AudioSource As;
    #endregion

    #region 资源属性
    #endregion

    #region 重写父类
    /// <summary>
    /// 设置精灵
    /// </summary>
    /// <param name="spritePath"></param>
    public override void SetSprite()
    {
        int randomNum = UnityEngine.Random.Range(1, 5);
        CurEliminateAttribute = (DungeonEnum.ElementAttributes)randomNum;
        //CurEliminateAttribute = DungeonEnum.ElementAttributes.Earth;
        AttrubuteToRender();
        base.SetSprite();
    }

    /// <summary>
    /// 设置名字
    /// </summary>
    public override void SetObjectName()
    {
        name = "Eliminate:" + XPosition + "," + YPosition;
    }

    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    public override void SetPosition(int xPosition, int yPosition)
    {
        base.SetPosition(xPosition, yPosition);

        if (xPosition == DungeonScene.InitXPosition && yPosition == DungeonScene.InitYPosition)
        {
            CurEliminateAttribute = DungeonEnum.ElementAttributes.Player;
            AttrubuteToRender();
        }
    }

    /// <summary>
    /// 强射玩家消除快
    /// </summary>
    public void SetToPlayerRender()
    {
        CurEliminateAttribute = DungeonEnum.ElementAttributes.Player;
        SetEnabelRender(true);
        AttrubuteToRender();
    }
    #endregion

    #region 私有方法
    public void AttrubuteToRender()
    {
        Sprite[] s = Resources.LoadAll<Sprite>(DungeonSpritePathManager.EliminatePath);
        ObjectSprite.sprite = s[AttributeToSpriteIndex()];
    }

    private int AttributeToSpriteIndex()
    {
        if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Earth)
        {
            return 0;
        }
        else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Fire)
        {
            return 3;
        }
        else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Water)
        {
            return 2;
        }
        else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Wind)
        {
            return 4;
        }
        else if (CurEliminateAttribute == DungeonEnum.ElementAttributes.Player)
        {
            return 1;
        }
        return -1;
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
    #endregion

    #region 公有方法
    public void SetChain(bool show)
    {
        if (DungeonScene.userInputLock == false)
        {
            if (XPosition == 0)
            {
                LinkChainLabel.transform.localPosition = new Vector3(75, LinkChainLabel.transform.localPosition.y, LinkChainLabel.transform.localPosition.z);
            }
            else if (XPosition == 6)
            {
                LinkChainLabel.transform.localPosition = new Vector3(-80, LinkChainLabel.transform.localPosition.y, LinkChainLabel.transform.localPosition.z);
            }
            if (show)
            {
                LinkChainLabel.gameObject.SetActive(true);
                int basic = 0;
                foreach (TileBlock b in DungeonScene.curLinkPath)
                {
                    EliminateBlock e = DungeonScene.FindEliminateByPosition(b.XPosition, b.YPosition);
                    if (e.CurEliminateAttribute != DungeonEnum.ElementAttributes.Player)
                    {
                        basic++;
                    }
                }
                string chainStr = basic.ToString() + "c";
                LinkChainLabel.SetNum(chainStr);
            }
            else
            {
                LinkChainLabel.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 设置连接加成
    /// </summary>
    public void SetAddition(float addition,bool show)
    {
        if (CurEliminateAttribute != DungeonEnum.ElementAttributes.Player)
        {
            LinkAdditionLabel.gameObject.SetActive(show);
            if (show)
            {
                DungeonScene.FindTile(XPosition, YPosition).CurAddition = addition;
            }
            string additionStr = addition.ToString();
            if (addition == 1 || addition == 2 || addition == 3 || addition == 4 || addition == 5 || addition == 6)
            {
                additionStr = addition.ToString() + ".0";
            }
            LinkAdditionLabel.SetNum("x" + additionStr);
        }
    }

    /// <summary>
    /// 清理消除块缓存数据
    /// </summary>
    public void ClearEliminateBlock()
    {
        LastEliminateBlock = null;
        NextEliminateBlock = null;
    }

    string GetAudioName()
    {
        string countS;
        int count = DungeonScene.curLinkPath.Count - 1;
        countS = count.ToString();
        if (count < 10)
        {
            countS = "0" + countS;
        }
        if (count > 17)
        {
            countS = "17";
        }
        return "se_step_plan_"+countS;
    }

    public static int DesCount = 0;

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

    /// <summary>
    /// 连接渲染
    /// </summary>
    /// <param name="linkToBlock"></param>
    public void LinkToRender()
    {
        As.enabled = false;
        As.clip = Resources.Load("Audio/" + GetAudioName()) as AudioClip;
        As.enabled = true;
        int basic = 0;
        foreach (TileBlock b in DungeonScene.curLinkPath)
        {
            EliminateBlock e = DungeonScene.FindEliminateByPosition(b.XPosition, b.YPosition);
            if (e.CurEliminateAttribute != DungeonEnum.ElementAttributes.Player)
            {
                basic++;
            }
        }
        float addition = 1 + basic * 0.1f;
        NextEliminateBlock.SetAddition(addition, true);
        SetChain(false);
        NextEliminateBlock.SetChain(true);
        NextEliminateBlock.Square.SetActive(true);
        DungeonEnum.FaceDirection direction = GetTargetDirection(NextEliminateBlock);
        CaculateLine(direction).SetActive(true);
        NextEliminateBlock.SelectScale(true);
        SelectScale(false);
    }

    /// <summary>
    /// 取消连接渲染
    /// </summary>
    public void UnLinkRender(bool NeedAudio)
    {
        if (NeedAudio)
        {
            As.enabled = false;
            As.clip = Resources.Load("Audio/" + GetAudioName()) as AudioClip;
            As.enabled = true;
        }
        Line1.SetActive(false);
        Line2.SetActive(false);
        Line3.SetActive(false);
        Line4.SetActive(false);
        Line5.SetActive(false);
        Line6.SetActive(false);
        Line7.SetActive(false);
        Line8.SetActive(false);
        if (CurEliminateAttribute != DungeonEnum.ElementAttributes.Player)
        {
            SetChain(true);
        }
        if (NextEliminateBlock)
        {
            NextEliminateBlock.SetChain(false);
            NextEliminateBlock.SetAddition(0, false);
            NextEliminateBlock.Square.SetActive(false);
            NextEliminateBlock.SelectScale(false);
        }
    }

    /// <summary>
    /// 设置明暗渲染
    /// </summary>
    /// <param name="enabel"></param>
    public void SetEnabelRender(bool enabel)
    {
        if (!enabel)
        {
            ObjectSprite.color = new Color(0.3f, 0.3f, 0.3f, 1);
        }
        else
        {
            ObjectSprite.color = new Color(1, 1, 1, 1);
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
                ObjectSprite.sortingOrder = highestDepth;
                AnimationHelper.AnimationScaleTo(new Vector3(1.2f, 1.2f, 1.2f), ObjectSprite.gameObject, iTween.EaseType.easeInExpo, null, null, 0.1f);
            }
            else
            {
                AnimationHelper.AnimationScaleTo(new Vector3(0.65f, 0.65f, 0.65f), ObjectSprite.gameObject, iTween.EaseType.easeInExpo, gameObject, "ScaleEnd", 0.1f);
            }
        }
    }
    /// <summary>
    /// 缩放结束的回调
    /// </summary>
    void ScaleEnd()
    {
        ObjectSprite.sortingOrder = basicDepth;
        AnimationHelper.AnimationScaleTo(new Vector3(1f, 1f,1f), ObjectSprite.gameObject, iTween.EaseType.linear, null, null, 0.15f);
    }

    /// <summary>
    /// 块消除
    /// </summary>
    public void BlockEliminateRender()
    {
        DesCount++;
        As.enabled = false;
        As.clip = Resources.Load("Audio/" + GetDesAudioName()) as AudioClip;
        As.enabled = true;
        ObjectSprite.sortingOrder = highestDepth;
        GameObject dis = Instantiate(DisAppearFX) as GameObject;
        dis.transform.position = transform.position;
        UnLinkRender(false);
        AnimationHelper.AnimationScaleTo(new Vector3(1.7f, 1.7f, 1.7f), ObjectSprite.gameObject, iTween.EaseType.linear, gameObject, "ScaleRemoveEnd", 0.3f);
        AnimationHelper.AnimationFadeTo(0.2f, ObjectSprite.gameObject, iTween.EaseType.linear, null, null, 0.3f);
    }

    void ScaleRemoveEnd()
    {
        ObjectHandler.ObjectDestoryFromDungeon(this);
    }

    /// <summary>
    /// 重设位置的动画
    /// </summary>
    public IEnumerator ResertPositionAnimation(float speed ,float delay,string endActiong)
    {
        yield return new WaitForSeconds(delay);
        NoDelayResertPositionAnimation(speed, endActiong);
    }

    public void NoDelayResertPositionAnimation(float speed, string endActiong)
    {
        SetObjectName();
        AnimationHelper.AnimationMoveToSpeed(CaculateRealPosition(XPosition, YPosition), gameObject, iTween.EaseType.easeInCubic, DungeonScene.gameObject, endActiong, speed);
    }

    /// <summary>
    /// 设置技能范围
    /// </summary>
    public void SetSkillEnabel(bool enabel)
    {
        SetEnabelRender(true);
        if (enabel)
        {
            ObjectSprite.sprite = SkillSprites[1];
        }
        else
        {
            ObjectSprite.sprite = SkillSprites[0];
        }
    }
    #endregion
}
