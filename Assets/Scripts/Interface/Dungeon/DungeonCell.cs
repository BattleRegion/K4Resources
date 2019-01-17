using UnityEngine;
using System.Collections;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;
public class DungeonCell : MonoBehaviour
{
    public UISprite Background;

    public UITexture Icon;

    public UISprite Element;

    public UILabel Name;

    public UILabel Warefare;

    public UILabel CostLabel;

    public UILabel DropRate;

    public GameObject Clear;

    public DungeonData CurDungeonData;

    public UISprite HasAchievedSprite;

    public UISprite NewTag;


    public void SetDungeonInfo(DungeonData dd)
    {
        if(PlayerPrefs.GetInt("entered_dungeon" + dd.Id) == 1)
        {
            NewTag.gameObject.SetActive(false);
        }
        else
        {
            NewTag.gameObject.SetActive(true);
        }

        CurDungeonData = dd;
        ChapterData chapterData = ConfigManager.ChapterConfig.GetChapterData(CurDungeonData.ChapterId);

        Background.spriteName = "menu_listitem_" + (chapterData.Rank * 2 - 1).ToString();
        Background.GetComponent<UIButton>().normalSprite = "menu_listitem_" + (chapterData.Rank * 2 - 1).ToString();
        Background.GetComponent<UIButton>().hoverSprite = "menu_listitem_" + (chapterData.Rank * 2).ToString();
        Background.GetComponent<UIButton>().pressedSprite = "menu_listitem_" + (chapterData.Rank * 2).ToString();

        switch ((int)dd.Element)
        {
            case 0: Element.spriteName = "element_type_none"; break;
            case 1: Element.spriteName = "element_type_earth"; break;
            case 2: Element.spriteName = "element_type_fire"; break;
            case 3: Element.spriteName = "element_type_wind"; break;
            case 4: Element.spriteName = "element_type_water"; break;
            default: Element.spriteName = ""; break;
        }

        Icon.mainTexture = Resources.Load<Texture>(Tools.GetIconTexturePath(dd.DungeonIcons[0]));
        Name.text = dd.Description;
        DropRate.text = dd.ChanceTips;
        if(dd.Warefare > (ConfigManager.ParamConfig.GetParam().WarefareRate * UserManager.CurUserInfo.CurWarfare))
        {
            Warefare.text = "[FF0000]" + dd.Warefare.ToString();
        }
        else if(dd.Warefare > (1.2f * UserManager.CurUserInfo.CurWarfare))
        {
            Warefare.text = "[FFC000]" + dd.Warefare.ToString();
        }
        else
        {
            Warefare.text = "[00FF00]" + dd.Warefare.ToString();
        }
        CostLabel.text = dd.Energy.ToString();

        if (UserManager.CurUserInfo.HasAchievedDungeon(CurDungeonData.Id))
        {
            HasAchievedSprite.spriteName = "level_diamond";
        }
        else
        {
            HasAchievedSprite.spriteName = "level_diamond_back";
        }

    }

    public void ClearRender(bool render)
    {
        Clear.SetActive(render);
        Clear.GetComponent<TweenAlpha>().enabled = render;
    }


    /// <summary>
    /// 点击
    /// </summary>
    public void Click()
    {
        PlayerPrefs.SetInt("entered_dungeon" + CurDungeonData.Id, 1);

        if(CurDungeonData.Warefare > (ConfigManager.ParamConfig.GetParam().WarefareRate * UserManager.CurUserInfo.CurWarfare))
        {
            DungeonWarning dw = GameObject.Find("Dungeon").GetComponent<DungeonWarning>();
            dw.ShowForbidden();
        }
        else if (CurDungeonData.Warefare > 1.2f * UserManager.CurUserInfo.CurWarfare)
        {
            DungeonWarning dw = GameObject.Find("Dungeon").GetComponent<DungeonWarning>();
            dw.ShowWarning(CurDungeonData);
        }
        else
        {
            //DungeonWarning dw = GameObject.Find("Dungeon").GetComponent<DungeonWarning>();
            //if (dw.ShouldShowConfirm(CurDungeonData))
            //{
            //    dw.ShowConfirm(CurDungeonData);
            //}
            //else
            //{
            //    PveGameControl.CurDungeonId = CurDungeonData.Id;
            //    DungeonSelectUI ds = GameObject.Find("Dungeon").GetComponent<DungeonSelectUI>();
            //    ds.ShowHelpList();
            //}
            PveGameControl.CurDungeonId = CurDungeonData.Id;
            DungeonSelectUI ds = GameObject.Find("Dungeon").GetComponent<DungeonSelectUI>();
            ds.ShowHelpList();
        }
    }
}