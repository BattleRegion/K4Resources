using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DungeonUI : MonoBehaviour
{
    #region 属性
    /// <summary>
    /// 当前副本主题对象
    /// </summary>
    public DungeonTheme CurTheme;

    /// <summary>
    /// 玩家信息UI对象
    /// </summary>
    public PlayerInfo CurPlayerUIInfo;
    #endregion

    #region 重写MONO
    // Use this for initialization
	void Start () {
	
	}
    #endregion

    #region 宠物头像
    /// <summary>
    /// 焦点某个属性的宠物头像
    /// </summary>
    /// <param name="UpAttribute"></param>
    public void FocusPetAvata(DungeonEnum.ElementAttributes attribute,int count)
    {
        List<PetAvata> attributePa = FindSameAttributePetAvata(attribute);
        int totalCount = count;
        if (count > attributePa.Count)
        {
            totalCount = attributePa.Count;
        }
        for (int i = 0; i < totalCount; i++)
        {
            PetAvata pa = attributePa[i];
            pa.AvatarFocus(true);
        }
    }

    /// <summary>
    /// 取消焦点
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="count"></param>
    public void DisFocusPetAvata(DungeonEnum.ElementAttributes attribute, int count)
    {
        List<PetAvata> attributePa = FindSameAttributePetAvata(attribute);
        //取消全部
        if (count == -1)
        {
            foreach (PetAvata pa in attributePa)
            {
                pa.AvatarFocus(false);
            }
        }
        else
        {
            List<PetAvata> curFocusPetAvatas = new List<PetAvata>();
            int focusCount = 0;
            foreach (PetAvata pa in attributePa)
            {
                if (pa.hasFocus == true)
                {
                    curFocusPetAvatas.Add(pa);
                    focusCount++;
                }
            }
            if (count < focusCount)
            {
                curFocusPetAvatas[curFocusPetAvatas.Count - 1].AvatarFocus(false);
            }
        }
    }

    /// <summary>
    /// 查找某个属性的头像
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    List<PetAvata> FindSameAttributePetAvata(DungeonEnum.ElementAttributes attribute)
    {
        List<PetAvata> attributePa = new List<PetAvata>();
        foreach (PetAvata pa in CurPlayerUIInfo.UserPetAvatas)
        {
            if (pa.CurPet != null)
            {
                if (pa.CurPet.CurPetData.PetPro == attribute)
                {
                    attributePa.Add(pa);
                }
            }
        }
        return attributePa;
    }

    /// <summary>
    /// 获取已经焦点的宠物头像
    /// </summary>
    /// <returns></returns>
    public List<PetAvata> GetFocusPetAvata()
    {
        List<PetAvata> focusPa = new List<PetAvata>();
        foreach (PetAvata pa in CurPlayerUIInfo.UserPetAvatas)
        {
            if (pa.hasFocus == true)
            {
                focusPa.Add(pa);
            }
        }
        return focusPa;
    }

    /// <summary>
    /// 查找宠物对应的头像
    /// </summary>
    /// <param name="userPet"></param>
    /// <returns></returns>
    public PetAvata FindPetAvata(UserPet userPet)
    {
        foreach (PetAvata pa in CurPlayerUIInfo.UserPetAvatas)
        {
            if (pa.CurPet == userPet)
            {
                return pa;
            }
        }
        return null;
    }

    public void AvataEffectRender(bool render,UserPet userPet)
    {
        FindPetAvata(userPet).AvataEffectiveRender(render);
    }

    /// <summary>
    /// 播放头像特效渲染
    /// </summary>
    /// <param name="render"></param>
    public void AllAvataEffectiveRender(bool render)
    {
        foreach (PetAvata pa in CurPlayerUIInfo.UserPetAvatas)
        {
            if (pa.CurPet !=null)
            {
                pa.AvataEffectiveRender(render);
            }
        }
    }
    #endregion
}
