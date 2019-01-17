using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChapterCell : MonoBehaviour {

    public UISprite Background;

    public UISprite Icon;

    public UILabel Name;

    public GameObject Clear;

    public ChapterData CurChapterData;

    public UILabel CountLabel;

    public UILabel CountTime;

    public List<UISprite> GotExpDungeons;

    public UISprite NewTag;

    bool Enter = false;

    bool IsEvent = false;

    public void SetChapterInfo(ChapterData chapterData)
    {
        if(PlayerPrefs.GetInt("chapter_entered" + chapterData.ChapterId) == 1)
        {
            NewTag.gameObject.SetActive(false);
        }
        else
        {
            NewTag.gameObject.SetActive(true);
        }

        IsEvent = chapterData.IsEvent;
        Background.spriteName = "menu_listitem_" + (chapterData.Rank * 2 - 1).ToString();
        Background.GetComponent<UIButton>().normalSprite = "menu_listitem_" + (chapterData.Rank * 2 - 1).ToString();
        Background.GetComponent<UIButton>().hoverSprite = "menu_listitem_" + (chapterData.Rank * 2).ToString();
        Background.GetComponent<UIButton>().pressedSprite = "menu_listitem_" + (chapterData.Rank * 2).ToString();
        Enter = chapterData.enter;
        CurChapterData = chapterData;
        Icon.spriteName = chapterData.ChapterIcon;
        Name.text = chapterData.ChapterName;

        List<DungeonData> curChapDungeos = ConfigManager.DungeonConfig.GetChapterDungeons(chapterData.ChapterId);
        for(int i = 0; i < GotExpDungeons.Count; i++)
        {
            if (i >= curChapDungeos.Count) GotExpDungeons[i].spriteName = "";
            else
            {
                if(UserManager.CurUserInfo.HasAchievedDungeon(curChapDungeos[i].Id))
                {
                    GotExpDungeons[i].spriteName = "level_diamond";
                }
                else
                {
                    GotExpDungeons[i].spriteName = "level_diamond_back";
                }
            }
        }
        //ClearRender(true);
    }

    public void ClearRender(bool render)
    {
        Clear.SetActive(render);
        Clear.GetComponent<TweenAlpha>().enabled = render;
    }

    public void ClickCell()
    {
        PlayerPrefs.SetInt("chapter_entered" + CurChapterData.ChapterId, 1);

        PveGameControl.CurChapterId = CurChapterData.ChapterId;
        if (IsEvent)
        {
            if (!CurChapterData.enter)
            {
                Debug.Log("未到副本开放时间，该副本在" + CurChapterData.OpenDateJson + "每周" + CurChapterData.OpenWeekJson + "每天" + CurChapterData.OpenHourJson + "时开放");
                return;
            }
        }
        DungeonSelectUI ds = GameObject.Find("Dungeon").GetComponent<DungeonSelectUI>();
        ds.ShowDungeons(CurChapterData.ChapterId);
    }


    void UpdateCountDown()
    {
        if (CurChapterData.IsEvent)
        {
            if (!ConfigManager.ChapterConfig.TryShow(CurChapterData))
            {
                Destroy(this.gameObject);
                return;
            }

            //Debug.Log(ConfigManager.ChapterConfig.TryShow(CurChapterData));
            if (CurChapterData.enter)
            {
                Background.color = Color.white;
                CountLabel.text = "开放中";
                CountTime.text = "";
            }
            else
            {
                Background.color = Color.gray;
                CountLabel.text = "开放倒计时";
                int openTime = 0;
                foreach(int i in CurChapterData.OpenHour)
                {
                    if(ConfigManager.LocalTime.LocalTime.Hour < i)
                    {
                        openTime = i;
                        break;
                    }
                }
                DateTime open = new DateTime(ConfigManager.LocalTime.LocalTime.Year, ConfigManager.LocalTime.LocalTime.Month, ConfigManager.LocalTime.LocalTime.Day, openTime, 0, 0);
                CountTime.text = (open - ConfigManager.LocalTime.LocalTime).Hours.ToString() + ":" + (((open - ConfigManager.LocalTime.LocalTime).Minutes < 10) ? ("0" + (open - ConfigManager.LocalTime.LocalTime).Minutes.ToString()) : (open - ConfigManager.LocalTime.LocalTime).Minutes.ToString()) + ":" + (((open - ConfigManager.LocalTime.LocalTime).Seconds < 10) ? ("0" + (open - ConfigManager.LocalTime.LocalTime).Seconds.ToString()) : (open - ConfigManager.LocalTime.LocalTime).Seconds.ToString());
            }
        }
        else
        {
            CountLabel.text = "";
            CountTime.text = "";
        }
    }

    void Update()
    {
        UpdateCountDown();
    }
}   