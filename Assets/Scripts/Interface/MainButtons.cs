using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;



public class MainButtons : MonoBehaviour 
{
    /// <summary>
    /// 选中之后切换颜色
    /// </summary>
    public UISprite ButtonChara;

    /// <summary>
    /// 选中之后切换图片
    /// </summary>
    public UISprite ButtonBg;

    /// <summary>
    /// 要打开的场景
    /// </summary>
    public GameObject TarScene;

    public GameObject ArenaTipsLevelObject;
    public UILabel ArenaTipsLevel;

    /// <summary>
    /// MainBoard上挂有关闭全部场景的函数
    /// </summary>
    public GameObject MainBoard;

    /// <summary>
    /// 点击主界面按钮时为true，当前界面检测到之后运行退出动画
    /// </summary>
    public static bool exit = false;


    void OnEnable()
    {
        if(name == "Button_Arena")
        {
            if(UserManager.CurUserInfo.Level >= ConfigManager.ParamConfig.GetParam().ArenaOpenLv)
            {
                ButtonChara.color = Color.white;
                ButtonBg.color = Color.white;
                GetComponent<BoxCollider>().enabled = true;
                ArenaTipsLevelObject.SetActive(false);
            }
            else
            {
                ButtonChara.color = Color.grey;
                ButtonBg.color = Color.grey;
                GetComponent<BoxCollider>().enabled = false;
                ArenaTipsLevelObject.SetActive(true);
                ArenaTipsLevel.text = ConfigManager.ParamConfig.GetParam().ArenaOpenLv.ToString();
            }
        }
    }

    IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(0.2f);
        MainBoard.GetComponent<ViewControl>().setViews();
        exit = false;
        TarScene.SetActive(true);
    }

	public void TriggerOnClick()
	{
		this.OnClick ();
	}

    void OnClick()
    {
        /*if (inter != null)
        {
            inter.Handler();
        }*/

        switch (name)
        {
            case "Button_Bar": if (GameObject.FindGameObjectWithTag("BarMenu") != null) return; break;
            case "Button_BlackJ": if (GameObject.FindGameObjectWithTag("BlackSMenu") != null) return; break;
            case "Button_Mall": if (GameObject.FindGameObjectWithTag("MallMenu") != null) return; break;
            case "Button_Arena": if (GameObject.FindGameObjectWithTag("ArenaMenu") != null) return; break;
            case "Button_monster": if (GameObject.FindGameObjectWithTag("MonsterMenu") != null) return; break;
            case "Button_Quest":
                {
                    if (GameObject.FindGameObjectWithTag("QuestMenu") != null)
                    {
                        if (GameObject.FindGameObjectWithTag("QuestMenu").transform.FindChild("TableBack").localPosition.x == 0) return;
                    }
                    break;
                }
            case "Button_Notice": if (GameObject.FindGameObjectWithTag("NoticeMenu") != null) return; break;
            default: break;
        }
        MainBoard.GetComponent<ViewControl>().SetButtons();
        switch (name)
        {
            case "Button_Bar": UserManager.CurView = ViewControl.Views.Social; break;
            case "Button_BlackJ": UserManager.CurView = ViewControl.Views.BlackSmith; break;
            case "Button_Mall":  UserManager.CurView = ViewControl.Views.Mall; break;
			case "Button_Arena": UserManager.CurView = ViewControl.Views.Arena; break;
            case "Button_monster": UserManager.CurView = ViewControl.Views.Monster; break;
            case "Button_Quest": UserManager.CurView = ViewControl.Views.Dungeon; break;
            case "Button_Notice": UserManager.CurView = ViewControl.Views.Notice; break;
            default: break;
        }
        if (name != "Button_Mall" && name != "Button_Notice")
        {
            Regex r1 = new Regex("yellow$");
            Match m1 = r1.Match(ButtonChara.spriteName);
            if (m1.Success)
            {
                Regex r2 = new Regex(@"^\w*_");
                Match m2 = r2.Match(ButtonChara.spriteName);
                if (m2.Success)
                {
                    ButtonChara.spriteName = m2.Value + "white";
                }
            }

            if (ButtonBg.spriteName == "button_bottom_1")
            {
                ButtonBg.spriteName = "button_bottom_2";
            }
        }
        exit = true;

        StartCoroutine(DelayExit());
    }
}
