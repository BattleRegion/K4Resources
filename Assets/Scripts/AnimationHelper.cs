using UnityEngine;
using System.Collections;
using System;

public class AnimationHelper  {
    /// <summary>
    /// 速度
    /// </summary>
    /// <param name="to"></param>
    /// <param name="moveTarget"></param>
    /// <param name="easyType"></param>
    /// <param name="actionEndTarget"></param>
    /// <param name="actionEnd"></param>
    /// <param name="time"></param>
    static public void AnimationMoveToSpeed(Vector3 to, GameObject moveTarget, iTween.EaseType easyType, GameObject actionEndTarget, string actionEnd, float speed)
    {
        Hashtable args = new Hashtable();
        args.Add("speed", speed);
        args.Add("islocal", true);
        args.Add("position", to);
        args.Add("easetype", easyType);
        args.Add("oncompleteparams", moveTarget);
        if (actionEndTarget)
        {
            args.Add("oncompletetarget", actionEndTarget);
            args.Add("oncomplete", actionEnd);
        }
        iTween.MoveTo(moveTarget, args);
    }
    /// <summary>
    /// 移动动画
    /// </summary>
    static public void AnimationMoveTo(Vector3 to,GameObject moveTarget,iTween.EaseType easyType,GameObject actionEndTarget,string actionEnd,float time, object oncompleteparams = null, string name = "")
    {
        Hashtable args = new Hashtable();
        args.Add("time", time);
        args.Add("islocal", true);
        args.Add("position", to);
        args.Add("easetype", easyType);
        if (actionEndTarget)
        {
            args.Add("oncompletetarget", actionEndTarget);
            args.Add("oncomplete", actionEnd);
            if (oncompleteparams != null)
            {
                args.Add("oncompleteparams", oncompleteparams);
            }
        }
		if(!string.IsNullOrEmpty(name))
		{
			args.Add("name", name);
		}
        iTween.MoveTo(moveTarget, args);
    }

    /// <summary>
    /// 缩放动画
    /// </summary>
    /// <param name="to"></param>
    /// <param name="moveTarget"></param>
    /// <param name="easyType"></param>
    /// <param name="actionEndTarget"></param>
    /// <param name="actionEnd"></param>
    /// <param name="time"></param>
    static public void AnimationScaleTo(Vector3 to, GameObject scaleTarget, iTween.EaseType easyType, GameObject actionEndTarget, string actionEnd, float time)
    {
        Hashtable args = new Hashtable();
        args.Add("time", time);
        args.Add("scale", to);
        args.Add("easetype", easyType);
        if (actionEndTarget)
        {
            args.Add("oncompletetarget", actionEndTarget);
            args.Add("oncomplete", actionEnd);
        }
        iTween.ScaleTo(scaleTarget, args);
    }

    /// <summary>
    /// 淡出动画
    /// </summary>
    /// <param name="to"></param>
    /// <param name="fadeTarget"></param>
    /// <param name="easyType"></param>
    /// <param name="actionEndTarget"></param>
    /// <param name="actionEnd"></param>
    /// <param name="time"></param>
    static public void AnimationFadeTo(float to, GameObject fadeTarget, iTween.EaseType easyType, GameObject actionEndTarget, string actionEnd, float time)
    {
        Hashtable args = new Hashtable();
        args.Add("time", time);
        args.Add("alpha", to);
        args.Add("easetype", easyType);
        if (actionEndTarget)
        {
            args.Add("oncompletetarget", actionEndTarget);
            args.Add("oncomplete", actionEnd);
        }
        iTween.FadeTo(fadeTarget, args);
    }

    /// <summary>
    /// 旋转动画
    /// </summary>
    /// <param name="to"></param>
    /// <param name="fadeTarget"></param>
    /// <param name="easyType"></param>
    /// <param name="actionEndTarget"></param>
    /// <param name="actionEnd"></param>
    /// <param name="time"></param>
    static public void AnimationRotateBy(Vector3 amount,iTween.LoopType looptype, GameObject rotateTarget, iTween.EaseType easyType, GameObject actionEndTarget, string actionEnd, float time)
    {
        Hashtable args = new Hashtable();
        args.Add("time", time);
        args.Add("amount", amount);
        args.Add("easetype", easyType);
        args.Add("looptype", looptype);
        if (actionEndTarget)
        {
            args.Add("oncompletetarget", actionEndTarget);
            args.Add("oncomplete", actionEnd);
        }
        iTween.RotateBy(rotateTarget, args);
    }

    /// <summary>
    /// 渐变
    /// </summary>
    public static void AnimationValueTo(GameObject target, float from, float to, float time, string onupdate, GameObject updateTarget, string oncomplete, GameObject oncompletetarget, System.Object oncompleteparams, string name = "")
    {
        Hashtable hash = new Hashtable();
        hash.Add("from", from);
        hash.Add("to", to);
        hash.Add("time", time);
        hash.Add("onupdate", onupdate);
        hash.Add("onupdatetarget", updateTarget);
        if (oncompletetarget)
        {
            hash.Add("oncomplete", oncomplete);
            hash.Add("oncompletetarget", oncompletetarget);
            if(oncompleteparams != null)
            {
                hash.Add("oncompleteparams", oncompleteparams);
            }
		}
		if(!string.IsNullOrEmpty(name))
		{
			hash.Add("name", name);
		}
        iTween.ValueTo(target, hash);
    }

    /// <summary>
    /// label数字增长
    /// </summary>
    public static void LabelUpdate(UILabel label, int from, int to, float time, string onupdate, GameObject updateTarget, string oncomplete, GameObject oncompletetarget)
    {
        AnimationValueTo(label.gameObject, (float)from, (float)to, time, onupdate, updateTarget, oncomplete, oncompletetarget, null);
    }
}
