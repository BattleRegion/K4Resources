using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ViewControl : MonoBehaviour 
{
    public enum Views
    {
        None,
        Monster,
        BlackSmith,
        Dungeon,
        Social,
        Mall,
        Arena,
        Notice
    }

    public List<GameObject> mainViews = new List<GameObject>();
    public GameObject dungeonOver;
	public GameObject pvpOver;

    private static GameObject operationCover = null;
    public static GameObject OperationCover
    {
        get
        {
            if(operationCover == null)
            {
                operationCover = GameObject.Find("TransparentCover").transform.FindChild("Cover").gameObject;
            }
            return operationCover;
        }
        set
        {
            operationCover = value;
        }
    }

    /// <summary>
    /// 按钮的字样
    /// </summary>
    public List<UISprite> ButtonCharas = new List<UISprite>();

    /// <summary>
    /// 按钮的背景
    /// </summary>
    public List<UISprite> ButtonBgs = new List<UISprite>();

    public static void SetCover(bool enable)
    {
        OperationCover.SetActive(enable);
    }

    public void setViews()
    {
        foreach (GameObject g in mainViews)
        {
            if (g.activeSelf == true)
            {
                g.SetActive(false);
            }
        }
    }

    public void SetButtons()
    {
        foreach (UISprite u in ButtonCharas)
        {
            Regex r = new Regex(@"^\w*_");
            Match m = r.Match(u.spriteName);
            if (m.Success)
            {
                u.spriteName = m.Value + "yellow";
            }
        }
        foreach (UISprite u in ButtonBgs)
        {
            u.spriteName = "button_bottom_1";
        }
    }

    /// <summary>
    /// 返回按钮调用
    /// </summary>
    public void ToPartyView()
    {
        switch (UserManager.CurView)
        {
            case Views.BlackSmith:
                {
                    ButtonCharas[1].spriteName = "bottom_chara_blacksmith_yellow";
                    ButtonBgs[1].spriteName = "button_bottom_1";
                    break;
                }
            case Views.Dungeon:
                {
                    ButtonCharas[4].spriteName = "bottom_chara_adventure_yellow";
                    ButtonBgs[4].spriteName = "button_bottom_1";
                    break;
                }
            case Views.Arena:
                {
                    ButtonCharas[2].spriteName = "bottom_chara_arena_yellow";
                    ButtonBgs[2].spriteName = "button_bottom_1";
                    break;
                }
            case Views.Social:
                {
                    ButtonCharas[0].spriteName = "bottom_chara_social_yellow";
                    ButtonBgs[0].spriteName = "button_bottom_1";
                    break;
                }
            default: break;
        }
        ButtonCharas[3].spriteName = "bottom_chara_party_white";
        ButtonBgs[3].spriteName = "button_bottom_2";
        UserManager.CurView = Views.Monster;
    }

    public void ToDiamondConsumeView()
    {
        switch (UserManager.CurView)
        {
            case Views.BlackSmith:
                {
                    ButtonCharas[1].spriteName = "bottom_chara_blacksmith_yellow";
                    ButtonBgs[1].spriteName = "button_bottom_1";
                    break;
                }
            case Views.Dungeon:
                {
                    ButtonCharas[4].spriteName = "bottom_chara_adventure_yellow";
                    ButtonBgs[4].spriteName = "button_bottom_1";
                    break;
                }
            case Views.Monster:
                {
                    ButtonCharas[3].spriteName = "bottom_chara_party_yellow";
                    ButtonBgs[3].spriteName = "button_bottom_1";
                    break;
                }
            case Views.Social:
                {
                    ButtonCharas[0].spriteName = "bottom_chara_social_yellow";
                    ButtonBgs[0].spriteName = "button_bottom_1";
                    break;
                }
            case Views.Arena:
                {
                    ButtonCharas[2].spriteName = "bottom_chara_arena_yellow";
                    ButtonBgs[2].spriteName = "button_bottom_1";
                    break;
                }
            default: break;
        }
        UserManager.CurView = Views.Mall;

        setViews();

        Mall.SetActive(true);
    }

    void OnEnable()
    {
		if(SceneDataManager.fromScene == SceneDataManager.PVP)
		{
			SceneDataManager.fromScene = "";
			if(pvpOver != null)
			{
				pvpOver.SetActive(true);

				PvpOverUI pvpOverUI = pvpOver.GetComponent<PvpOverUI>();
				if(pvpOverUI != null) pvpOverUI.ShowData();
			}
		}
        else
        {
			if(dungeonOver != null)
				dungeonOver.SetActive(OverControl.isOver);
			switch (UserManager.CurView)
			{
			    case Views.Monster:
			    {
				    if (ButtonCharas.Count == 5)
				    {
					    ButtonCharas[3].spriteName = "bottom_chara_party_white";
					    ButtonBgs[3].spriteName = "button_bottom_2";
				    }
				    break;
			    }
			    case Views.Dungeon:
			    {
				    if (ButtonCharas.Count == 5)
				    {
					    ButtonCharas[4].spriteName = "bottom_chara_adventure_white";
					    ButtonBgs[4].spriteName = "button_bottom_2";
				    }
				    break;
			    }
			}
		}
    }

    public GameObject GuiderPanel;

    public GameObject Mall;
    public GameObject Notice;
	public MainButtons ArenaButton;
    /// <summary>
    /// 显示时间
    /// </summary>
    public void OpenMall()
    {
        setViews();
        Mall.SetActive(true);
        Debug.Log(ConfigManager.LocalTime.LocalTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + ConfigManager.LocalTime.LocalDayOfWeek.ToString());
    }

    public void OpenNotice()
    {
        setViews();
        Notice.SetActive(true);
    }
}
