using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;
public class DungeonSelectUI : MonoBehaviour
{

    #region 属性
    public enum ListType
    {
        Normal = 0,
        Event = 1,
        Help = 2
    }

    public enum ViewType
    {
        ChapterView = 0,
        DungeonView = 1,
        HelpView = 2,
        EventView = 3,
        SelectParty = 4
    }

    ListType type;

    ViewType view;

    public GameObject UserBoard;

    public GameObject BackTitle;

    public GameObject DungeonBackTitle;

    public GameObject Table;

    public GameObject DungeonTable;

    public GameObject ChapterInfoCell;

    public GameObject DungeonInfoCell;

    /// <summary>
    /// 助战cell
    /// </summary>
    public GameObject HelpCellPrefab;

    public UIScrollView HelpScroll;

    public GameObject HelpBack;

    public GameObject HelpTable;

    public UIGrid HelpGrid;


    public UIGrid Grid;

    public UIGrid Grid_Hard;

    public UIGrid Grid_hero;

    public UIGrid EventGrid;

    public UIGrid DungeonGrid;

    public UIScrollView Scroll;

    public UIScrollView DungeonScroll;

    public UIScrollView Hard_Scroll;

    public UIScrollView Hero_Scroll;

    /// <summary>
    /// 活动副本scrollview
    /// </summary>
    public UIScrollView EventScroll;

    /// <summary>
    /// 活动副本返回条
    /// </summary>
    public GameObject EventBack;

    /// <summary>
    /// 活动副本List
    /// </summary>
    public GameObject EventTable;

    /// <summary>
    /// 选择队伍
    /// </summary>
    public GameObject SelectPartyObject;

    public PartyInfo PartyControl;

    public GameObject SelectPartyBack;

    public GameObject SelectPartyBoard;

    public GameObject Button_Enter;

    int selected_friend_id;

    //toggles
    public UIToggle chapter_toggle_normal;
    public UIToggle chapter_toggle_hard;

    public GameObject enterLevelLimit_hard;

    public UIToggle chapter_toggle_hero;

    public GameObject enterLevelLimit_hero;

    public UIToggle chapter_toggle_event;

    public const int value_normal_toggle = 0;
    public const int value_hard_toggle = 1;
    public const int value_hero_toggle = 2;
    public const int value_event_toggle = 3;

    //动画
    public DungeonListAnime ChapterList;

    public DungeonListAnime HardChapterList;

    public DungeonListAnime HeroChapterList;

    public DungeonListAnime EventList;

    public DungeonListAnime DungeonList;

    public DungeonListAnime HelpList;
    #endregion

    #region MONO

    void OnEnable()
    {
        type = ListType.Normal;
        view = ViewType.ChapterView;

        //困难副本进入限制 
        if(UserManager.CurUserInfo.Level < 12)
        {
            enterLevelLimit_hard.SetActive(true);
            chapter_toggle_hard.transform.FindChild("Label").GetComponent<UIWidget>().color = Color.grey;
            chapter_toggle_hard.GetComponent<BoxCollider>().enabled = false;
            chapter_toggle_hard.enabled = false;
            chapter_toggle_hard.transform.FindChild("Sprite2").gameObject.SetActive(false);
            chapter_toggle_hard.GetComponent<UIToggledObjects>().activate[0].SetActive(false);
        }
        else
        {
            enterLevelLimit_hard.SetActive(false);
            chapter_toggle_hard.transform.FindChild("Label").GetComponent<UIWidget>().color = Color.white;
            chapter_toggle_hard.GetComponent<BoxCollider>().enabled = true;
            chapter_toggle_hard.enabled = true;
        }

        //英雄副本进入等级限制
        if (UserManager.CurUserInfo.Level < 20)
        {
            enterLevelLimit_hero.SetActive(true);
            chapter_toggle_hero.transform.FindChild("Label").GetComponent<UIWidget>().color = Color.grey;
            chapter_toggle_hero.GetComponent<BoxCollider>().enabled = false;
            chapter_toggle_hero.enabled = false;
            chapter_toggle_hero.transform.FindChild("Sprite3").gameObject.SetActive(false);
            chapter_toggle_hero.GetComponent<UIToggledObjects>().activate[0].SetActive(false);
        }
        else
        {
            enterLevelLimit_hero.SetActive(false);
            chapter_toggle_hero.transform.FindChild("Label").GetComponent<UIWidget>().color = Color.white;
            chapter_toggle_hero.GetComponent<BoxCollider>().enabled = true;
            chapter_toggle_hero.enabled = true;
        }


        EventDelegate event_normal_toggle = new EventDelegate(this, "SwitchToggle");
        event_normal_toggle.parameters[0] = new EventDelegate.Parameter(this, "value_normal_toggle");
        chapter_toggle_normal.onChange.Add(event_normal_toggle);

        EventDelegate event_hard_toggle = new EventDelegate(this, "SwitchToggle");
        event_hard_toggle.parameters[0] = new EventDelegate.Parameter(this, "value_hard_toggle");
        chapter_toggle_hard.onChange.Add(event_hard_toggle);

        EventDelegate event_hero_toggle = new EventDelegate(this, "SwitchToggle");
        event_hero_toggle.parameters[0] = new EventDelegate.Parameter(this, "value_hero_toggle");
        chapter_toggle_hero.onChange.Add(event_hero_toggle);

        EventDelegate event_event_toggle = new EventDelegate(this, "SwitchToggle");
        event_event_toggle.parameters[0] = new EventDelegate.Parameter(this, "value_event_toggle");
        chapter_toggle_event.onChange.Add(event_event_toggle);

        UIEventListener.Get(Button_Enter).onClick = (g) => 
        {
            JsonObject args = new JsonObject();
            args.Add("dungeon_id", PveGameControl.CurDungeonId);
            args.Add("queue_id", PartyControl.curPartyIndex);
            args.Add("friend_id", selected_friend_id);
            //UserManager.CurUserInfo.UserPets.Add(HelpPet);
            SocketCenter.Request(GameRouteConfig.EnterScene, args, (r) =>
            {
                if (r.Code == SocketResult.ResultCode.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        UserManager.CurUserInfo.CurPartyIndex = PartyControl.curPartyIndex;

                        JsonObject sceneContext = (JsonObject)r.Data["scene_context"];
                        PveGameControl.CurPveData = (JsonObject)sceneContext["drop_list"];

                        if (GuiderLocal.TriggerPve())
                        {
                            Debug.Log("triggerpve guid   true");
                            PveGameControl.CurFriend = FriendInfo.GuiderFriend;
                        }
                        else
                        {
                            PveGameControl.CurFriend = new FriendInfo((JsonObject)sceneContext["friend"]);
                        }

                        UserManager.CurUserInfo.AddElements((JsonArray)r.Data["consume"]);
                        UserPet fp = PveGameControl.CurFriend.FriendLeader;

                        Application.LoadLevel("Pve");
                    });
                }
            }, null, true, false);
        };

        ShowChapterInfo();
        //UIEventListener.Get(chapter_toggle_normal).onClick = (g) =>
        //{
        //    Debug.Log(chapter_toggle_normal.GetComponent<UIToggle>().value);
        //    if (chapter_toggle_normal.GetComponent<UIToggle>().value == false)
        //    {
        //        SwitchToggle(0);
        //    }
        //};

        //UIEventListener.Get(chapter_toggle_hard).onClick = (g) =>
        //{
        //    SwitchToggle(1);
        //};

        //UIEventListener.Get(chapter_toggle_hero).onClick = (g) =>
        //{
        //    SwitchToggle(2);
        //};

        //UIEventListener.Get(chapter_toggle_event).onClick = (g) =>
        //{
        //    SwitchToggle(3);
        //};

    }

    void Awake()
    {
        DelaySwitchToggle();
    }
    
    void DelaySwitchToggle()
    {
        if (PveGameControl.CurDungeonId != null)  //根据上一个副本确定初始化显示界面
        {
            chapter_toggle_normal.startsActive = false;
            chapter_toggle_hard.startsActive = false;
            chapter_toggle_hero.startsActive = false;
            chapter_toggle_event.startsActive = false;

            DungeonData d = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);
            ChapterData c = ConfigManager.ChapterConfig.GetChapterData(d.ChapterId);
            if (c.IsEvent)
            {
                chapter_toggle_event.startsActive = true;
            }
            else switch (c.Rank)
                {
                    case 1:
                        {
                            chapter_toggle_normal.startsActive = true;
                            break;
                        }
                    case 2:
                        {
                            chapter_toggle_hard.startsActive = true;
                            break;
                        }
                    case 3:
                        {
                            chapter_toggle_hero.startsActive = true;
                            break;
                        }
                }
        }
    }
    void OnDisable()
    {
        Clear();
        Table.transform.localPosition = new Vector3(800, Table.transform.localPosition.y, 0);
        BackTitle.transform.localPosition = new Vector3(-800, BackTitle.transform.localPosition.y, 0);
        DungeonBackTitle.transform.localPosition = new Vector3(-800, DungeonBackTitle.transform.localPosition.y, 0);
        DungeonTable.transform.localPosition = new Vector3(800, DungeonTable.transform.localPosition.y, 0);
        EventTable.transform.localPosition = new Vector3(800, EventTable.transform.localPosition.y, 0);
        EventBack.transform.localPosition = new Vector3(-800, EventBack.transform.localPosition.y, 0);
        HelpTable.transform.localPosition = new Vector3(800, HelpTable.transform.localPosition.y, 0);
        HelpBack.transform.localPosition = new Vector3(-800, HelpBack.transform.localPosition.y, 0);
        SelectPartyBoard.transform.localPosition = new Vector3(800, SelectPartyBoard.transform.localPosition.y, 0);
        SelectPartyBack.transform.localPosition = new Vector3(-800, SelectPartyBack.transform.localPosition.y, 0);

        SelectPartyObject.SetActive(false);
        UserBoard.SetActive(true);
    }

    void Update()
    {
        if (view == ViewType.SelectParty)
        {
            DungeonData d = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);
            if (UserManager.CurUserInfo.UserPartys[PartyControl.curPartyIndex].Elementconfirm(d.Element))
            {
                TipsLabel.SetActive(true);
            }
            else
            {
                TipsLabel.SetActive(false);
            }
        }

        if(MainButtons.exit)
        {

            switch(view)
            {
                case ViewType.ChapterView:
                    {
                        BackTitle.SetActive(false);
                        AnimationHelper.AnimationMoveTo(new Vector3(800, Table.transform.localPosition.y, 0), Table, iTween.EaseType.linear, null, null, 0.2f);
                        break;
                    }
                case ViewType.DungeonView:
                    {
                        AnimationHelper.AnimationMoveTo(new Vector3(800, DungeonTable.transform.localPosition.y, 0), DungeonTable, iTween.EaseType.linear, null, null, 0.2f);
                        AnimationHelper.AnimationMoveTo(new Vector3(-800, DungeonBackTitle.transform.localPosition.y, 0), DungeonBackTitle, iTween.EaseType.linear, null, null, 0.2f);
                        break;
                    }
                case ViewType.EventView:
                    {
                        AnimationHelper.AnimationMoveTo(new Vector3(800, EventTable.transform.localPosition.y, 0), EventTable, iTween.EaseType.linear, null, null, 0.2f);
                        AnimationHelper.AnimationMoveTo(new Vector3(-800, EventBack.transform.localPosition.y, 0), EventBack, iTween.EaseType.linear, null, null, 0.2f);
                        break;
                    }
                case ViewType.HelpView:
                    {
                        AnimationHelper.AnimationMoveTo(new Vector3(800, HelpTable.transform.localPosition.y, 0), HelpTable, iTween.EaseType.linear, null, null, 0.2f);
                        AnimationHelper.AnimationMoveTo(new Vector3(-800, HelpBack.transform.localPosition.y, 0), HelpBack, iTween.EaseType.linear, null, null, 0.2f);
                        break;
                    }
                case ViewType.SelectParty:
                    {
                        K4MoveHelper.MoveTo(SelectPartyBoard, new Vector3(800, SelectPartyBoard.transform.localPosition.y, 0), 0.2f);
                        K4MoveHelper.MoveTo(SelectPartyBack, new Vector3(-800, SelectPartyBack.transform.localPosition.y, 0), 0.2f);
                        break;
                    }
                default: break;
            }
        }
    }

    void ActionEnd()
    {
        if(PartyAnime.moveEnd != null)
        {
            PartyAnime.moveEnd();
            PartyAnime.moveEnd = null;
        }
    }

    #endregion

    #region chapterlist
    public void ShowChapterInfo()
    {
        view = ViewType.ChapterView;

        Clear();

        //动画
        //AnimationHelper.AnimationMoveTo(new Vector3(-60, BackTitle.transform.localPosition.y, 0), BackTitle, iTween.EaseType.linear, null, null, 0.2f);
        BackTitle.SetActive(true);
        AnimationHelper.AnimationMoveTo(new Vector3(0, Table.transform.localPosition.y, 0), Table, iTween.EaseType.linear, gameObject, "ActionEnd", 0.2f);

        RenderList(Scroll, Grid, ChapterList, 0);
        Invoke("DelayShowChapter", 0.01f);

        RenderList(Hard_Scroll, Grid_Hard, HardChapterList, 1);
        Invoke("DelayShowHardChapter", 0.01f);

        RenderList(Hero_Scroll, Grid_hero, HeroChapterList, 2);
        Invoke("DelayShowHeroChapter", 0.01f);

        RenderList(EventScroll, EventGrid, EventList, 3);
        Invoke("DelayShowEventChapter", 0.01f);

        //ResetToggles();
    }

    void DelayShowChapter()
    {
        if(chapter_toggle_normal.value)
        {
            Scroll.SetDragAmount(0, 0, false);
            Grid.Reposition();
            float f = ChapterList.Show();
            StartCoroutine(GridReposition(Grid, f));
        }
    }

    void DelayShowHardChapter()
    {
        if(chapter_toggle_hard.value)
        {
            Hard_Scroll.SetDragAmount(0, 0, false);
            Grid_Hard.Reposition();
            float f = HardChapterList.Show();
            StartCoroutine(GridReposition(Grid_Hard, f));
        }
    }

    void DelayShowHeroChapter()
    {
        if(chapter_toggle_hero.value)
        {
            Hero_Scroll.SetDragAmount(0, 0, false);
            Grid_hero.Reposition();
            float f = HeroChapterList.Show();
            StartCoroutine(GridReposition(Grid_hero, f));
        }
    }

    void DelayShowEventChapter()
    {
        if(chapter_toggle_event.value)
        {
            EventScroll.SetDragAmount(0, 0, false);
            EventGrid.Reposition();
            float f = EventList.Show();
            StartCoroutine(GridReposition(EventGrid, f));
        }
    }
    #endregion

    #region EventList
    /// <summary>
    /// 活动关卡按钮事件
    /// </summary>
    public void ShowEventChapter()
    {
        view = ViewType.EventView;
        type = ListType.Event;
        BackTitle.SetActive(false);
        AnimationHelper.AnimationMoveTo(new Vector3(800, Table.transform.localPosition.y, 0), Table, iTween.EaseType.linear, gameObject, "ShowEvent", 0.2f);
        //AnimationHelper.AnimationMoveTo(new Vector3(-800, BackTitle.transform.localPosition.y, 0), BackTitle, iTween.EaseType.linear, gameObject, "ShowEvent", 0.2f);
    }

    /// <summary>
    /// 显示活动关卡
    /// </summary>
    public void ShowEvent()
    {
        Clear();
        AnimationHelper.AnimationMoveTo(new Vector3(-60, EventBack.transform.localPosition.y, 0), EventBack, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(0, EventTable.transform.localPosition.y, 0), EventTable, iTween.EaseType.linear, gameObject, "ActionEnd", 0.2f);
        if (ConfigManager.ChapterConfig.GetEventChapters().Count < 5)
        {
            EventScroll.enabled = false;
        }
        else
        {
            EventScroll.enabled = true;
        }
        EventScroll.SetDragAmount(0, 0, false);
        List<ChapterData> Chapters = ConfigManager.ChapterConfig.GetEventChapters();
        for (int i = Chapters.Count - 1; i >= 0; i--)
        {
            GameObject chapterCell = NGUITools.AddChild(EventGrid.gameObject, ChapterInfoCell);
            ChapterCell cc = chapterCell.GetComponent<ChapterCell>();
            chapterCell.name = Chapters[i].ChapterId;
            cc.SetChapterInfo(Chapters[i]);
            cc.ClearRender(UserManager.CurUserInfo.HasClearChapter(Chapters[i].ChapterId));
            EventList.cells.Add(chapterCell);
        }
        EventGrid.Reposition();
        float f = EventList.Show();
        StartCoroutine(GridReposition(EventGrid, f));
    }
    #endregion

    #region dungeonlist
    List<DungeonData> ChapterDungeons;
    public void ShowDungeons(string chapterId)
    {
        view = ViewType.DungeonView;
        ChapterDungeons = ConfigManager.DungeonConfig.GetChapterDungeons(chapterId);
        if (type == ListType.Normal)
        {
            BackTitle.SetActive(false);
            AnimationHelper.AnimationMoveTo(new Vector3(800, Table.transform.localPosition.y, 0), Table, iTween.EaseType.linear, gameObject, "ChapterMoveOutEnd", 0.2f);
            //AnimationHelper.AnimationMoveTo(new Vector3(-800, BackTitle.transform.localPosition.y, 0), BackTitle, iTween.EaseType.linear, gameObject, "ChapterMoveOutEnd", 0.2f);
        }
        //else if (type == ListType.Event)
        //{
        //    AnimationHelper.AnimationMoveTo(new Vector3(800, EventTable.transform.localPosition.y, 0), EventTable, iTween.EaseType.linear, null, null, 0.2f);
        //    AnimationHelper.AnimationMoveTo(new Vector3(-800, EventBack.transform.localPosition.y, 0), EventBack, iTween.EaseType.linear, gameObject, "ChapterMoveOutEnd", 0.2f);
        //}
        else
        {
            ChapterMoveOutEnd();
        }
    }

    public void ChapterMoveOutEnd()
    {
        DungeonList.cells.Clear();
        //动画
        AnimationHelper.AnimationMoveTo(new Vector3(-40, DungeonBackTitle.transform.localPosition.y, 0), DungeonBackTitle, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(0, DungeonTable.transform.localPosition.y, 0), DungeonTable, iTween.EaseType.linear, gameObject, "ActionEnd", 0.2f);
        if (ChapterDungeons.Count < 5)
        {
            DungeonScroll.enabled = false;
        }
        else
        {
            DungeonScroll.enabled = true;
        }
        DungeonScroll.SetDragAmount(0, 0, false);
        for (int i = ChapterDungeons.Count - 1; i >= 0; i--)
        {
            if (!string.IsNullOrEmpty(ChapterDungeons[i].ExDungeon))
            {
                if(ChapterDungeons[i].ExDungeons != null)
                {
                    int count = 0;
                    foreach(string s in ChapterDungeons[i].ExDungeons)
                    {
                        if(UserManager.CurUserInfo.HasClearDungeon(s))
                        {
                            count++;
                        }
                    }
                    if(count == ChapterDungeons[i].ExDungeons.Length)
                    {
                        GameObject dungeonInfoCell = NGUITools.AddChild(DungeonGrid.gameObject, DungeonInfoCell);
                        DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                        dungeonInfoCell.name = ChapterDungeons[i].Id;
                        dc.SetDungeonInfo(ChapterDungeons[i]);
                        dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                        DungeonList.cells.Add(dungeonInfoCell);
                    }
                }
                else if (UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].ExDungeon))
                {
                    GameObject dungeonInfoCell = NGUITools.AddChild(DungeonGrid.gameObject, DungeonInfoCell);
                    DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                    dungeonInfoCell.name = ChapterDungeons[i].Id;
                    dc.SetDungeonInfo(ChapterDungeons[i]);
                    dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                    DungeonList.cells.Add(dungeonInfoCell);
                }
            }
            else
            {
                GameObject dungeonInfoCell = NGUITools.AddChild(DungeonGrid.gameObject, DungeonInfoCell);
                DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                dungeonInfoCell.name = ChapterDungeons[i].Id;
                dc.SetDungeonInfo(ChapterDungeons[i]);
                dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                DungeonList.cells.Add(dungeonInfoCell);
            }
        }
        DungeonGrid.Reposition();
        float f = DungeonList.Show();
        StartCoroutine(GridReposition(DungeonGrid, f));
    }
    #endregion

    #region helplist
    public void ShowHelpList()
    {
        type = ListType.Help;
        //AnimationHelper.AnimationMoveTo(new Vector3(800, DungeonTable.transform.localPosition.y, 0), DungeonTable, iTween.EaseType.linear, null, null, 0.2f);
        //AnimationHelper.AnimationMoveTo(new Vector3(-800, DungeonBackTitle.transform.localPosition.y, 0), DungeonBackTitle, iTween.EaseType.linear, gameObject, "ShowHelp", 0.2f);

        BackTitle.SetActive(false);
        AnimationHelper.AnimationMoveTo(new Vector3(800, Table.transform.localPosition.y, 0), Table, iTween.EaseType.linear, gameObject, "ShowHelp", 0.2f);
    }

    void ShowHelp()
    {
        view = ViewType.HelpView;
        Clear();

        if (GuiderLocal.TriggerPve())
        {
            int Num = 0;
            AnimationHelper.AnimationMoveTo(new Vector3(-60, HelpBack.transform.localPosition.y, 0), HelpBack, iTween.EaseType.linear, null, null, 0.2f);
            AnimationHelper.AnimationMoveTo(new Vector3(0, HelpTable.transform.localPosition.y, 0), HelpTable, iTween.EaseType.linear, gameObject, "ActionEnd", 0.2f);
            HelpScroll.enabled = false;

            GameObject helpInfoCell = NGUITools.AddChild(HelpGrid.gameObject, HelpCellPrefab);
            HelpCell h = helpInfoCell.GetComponent<HelpCell>();
            helpInfoCell.name = Num.ToString();
            h.SetHelpInfo(HelpData.tempHelper);
            HelpList.cells.Add(helpInfoCell);
        }
        else
        {
            UserManager.CurUserInfo.RefreshHelpList(() =>
            {
                Loom.QueueOnMainThread(() =>
                {
                    int Num = 0;
                    AnimationHelper.AnimationMoveTo(new Vector3(-60, HelpBack.transform.localPosition.y, 0), HelpBack, iTween.EaseType.linear, null, null, 0.2f);
                    AnimationHelper.AnimationMoveTo(new Vector3(0, HelpTable.transform.localPosition.y, 0), HelpTable, iTween.EaseType.linear, gameObject, "ActionEnd", 0.2f);
                    if (UserManager.CurUserInfo.UserHelpers.Count < 5)
                    {
                        HelpScroll.enabled = false;
                    }
                    else
                    {
                        HelpScroll.enabled = true;
                    }
                    foreach (HelpData hc in UserManager.CurUserInfo.UserHelpers)
                    {
                        GameObject helpInfoCell = NGUITools.AddChild(HelpGrid.gameObject, HelpCellPrefab);
                        HelpCell h = helpInfoCell.GetComponent<HelpCell>();
                        helpInfoCell.name = Num.ToString();
                        Num++;
                        h.SetHelpInfo(hc);
                        HelpList.cells.Add(helpInfoCell);
                    }
                    HelpGrid.Reposition();
                    float f = HelpList.Show();
                    StartCoroutine(GridReposition(HelpGrid, f));
                });
            });
        }
    }
    #endregion

    #region SelectParty
    public void ExShowSelectBoard(int selectedFriend)
    {
        selected_friend_id = selectedFriend;
        AnimationHelper.AnimationMoveTo(new Vector3(800, HelpTable.transform.localPosition.y, 0), HelpTable, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(-800, HelpBack.transform.localPosition.y, 0), HelpBack, iTween.EaseType.linear, gameObject, "ShowSelectBoard", 0.2f);
        for(int i = 0; i < 5; i++)
        {
            StartCoroutine(PartyControl.refreshParty(i, true));
        }
    }

    public void ShowSelectBoard()
    {
        UserBoard.SetActive(false);

        view = ViewType.SelectParty;
        SelectPartyObject.SetActive(true);
        SetAchievement();

        AnimationHelper.AnimationMoveTo(new Vector3(-60, SelectPartyBack.transform.localPosition.y, 0), SelectPartyBack, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(0, SelectPartyBoard.transform.localPosition.y, 0), SelectPartyBoard, iTween.EaseType.linear, gameObject, "ActionEnd", 0.2f);
    }


    public UISprite AchievementDiamond;
    public UILabel AchievementTips;
    public UISprite DungeonElementSprite;
    public GameObject TipsLabel;


    void SetAchievement()
    {
        DungeonData CurDungeonData = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);
        DungeonElementSprite.spriteName = Tools.GetHardwareElement(CurDungeonData.Element);
        if (UserManager.CurUserInfo.HasAchievedDungeon(CurDungeonData.Id))
        {
            AchievementDiamond.spriteName = "level_diamond";
        }
        else
        {
            AchievementDiamond.spriteName = "level_diamond_back";
        }
        AchievementTips.text = CurDungeonData.QuestTips;
    }
    #endregion

    #region 返回主场景
    public void BackToMain()
    {
        Clear();
        BackTitle.SetActive(false);
        AnimationHelper.AnimationMoveTo(new Vector3(800, Table.transform.localPosition.y, 0), Table, iTween.EaseType.linear, null, null, 0.2f);
        //AnimationHelper.AnimationMoveTo(new Vector3(-800, BackTitle.transform.localPosition.y, 0), BackTitle, iTween.EaseType.linear, gameObject, "BackToMainEnd", 0.2f);
    }

    void BackToMainEnd()
    {
        Invoke("InvokeMainEnd", 0.1f);
    }

    void InvokeMainEnd()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region helplist返回dungeonlist
    public void BackToDungeon()
    {
        foreach (GameObject go in HelpList.cells)
        {
            DestroyImmediate(go);
        }
        HelpList.cells.Clear();

        //if (ConfigManager.ChapterConfig.GetChapterData(PveGameControl.CurChapterId).IsEvent)
        //{
        //    type = ListType.Event;
        //}
        //else
        //{
        //    type = ListType.Normal;
        //}

        AnimationHelper.AnimationMoveTo(new Vector3(-800, HelpBack.transform.localPosition.y, 0), HelpBack, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, HelpTable.transform.localPosition.y, 0), HelpTable, iTween.EaseType.linear, gameObject, "BackToDungeonEnd", 0.2f);
    }

    void BackToDungeonEnd()
    {
        //ShowDungeons(PveGameControl.CurChapterId);
        ShowChapterInfo();
    }
    #endregion
        
    #region dungeonlist返回chapterlist
    public void BackToChapter()
    {
        foreach (GameObject go in DungeonList.cells)
        {
            DestroyImmediate(go);
        }
        DungeonList.cells.Clear();
        AnimationHelper.AnimationMoveTo(new Vector3(-800, DungeonBackTitle.transform.localPosition.y, 0), DungeonBackTitle, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, DungeonTable.transform.localPosition.y, 0), DungeonTable, iTween.EaseType.linear, gameObject, "BackToChapterEnd", 0.2f);
    }


    void BackToChapterEnd()
    {
        ////动画
        //if (type == ListType.Normal)
        //{
        //    //AnimationHelper.AnimationMoveTo(new Vector3(-60, 400, 0), BackTitle, iTween.EaseType.linear, null, null, 0.2f);
        //    //AnimationHelper.AnimationMoveTo(new Vector3(0, -80, 0), Table, iTween.EaseType.linear, null, null, 0.2f);
        //    ShowChapterInfo();
        //}
        //else
        //{
        //    //AnimationHelper.AnimationMoveTo(new Vector3(-60, 400, 0), EventBack, iTween.EaseType.linear, null, null, 0.2f);
        //    //AnimationHelper.AnimationMoveTo(new Vector3(0, -80, 0), EventTable, iTween.EaseType.linear, null, null, 0.2f);
        //    ShowEventChapter();
        //}
        ShowChapterInfo();
    }
    #endregion

    #region eventlist返回chapterlist
    public void ExitEvent()
    {
        foreach (GameObject go in DungeonList.cells)
        {
            DestroyImmediate(go);
        }
        DungeonList.cells.Clear();
        AnimationHelper.AnimationMoveTo(new Vector3(-800, EventBack.transform.localPosition.y, 0), EventBack, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, EventTable.transform.localPosition.y, 0), EventTable, iTween.EaseType.linear, gameObject, "ExitEventEnd", 0.2f);
    }

    void ExitEventEnd()
    {
        OnEnable();
        //AnimationHelper.AnimationMoveTo(new Vector3(-60, BackTitle.transform.localPosition.y, 0), BackTitle, iTween.EaseType.linear, null, null, 0.2f);
        BackTitle.SetActive(true);
        AnimationHelper.AnimationMoveTo(new Vector3(0, Table.transform.localPosition.y, 0), Table, iTween.EaseType.linear, null, null, 0.2f);
    }
    #endregion

    #region SelectBoard返回HelpList
    public void BackToHelp()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-800, SelectPartyBack.transform.localPosition.y, 0), SelectPartyBack, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, SelectPartyBoard.transform.localPosition.y, 0), SelectPartyBoard, iTween.EaseType.linear, gameObject, "BackToHelpEnd", 0.2f);
    }

    void BackToHelpEnd()
    {
        UserBoard.SetActive(true);

        SelectPartyObject.SetActive(false);
        ShowHelp();
    }
    #endregion

    #region 方法
    void Clear()
    {
        Scroll.enabled = true;
        DungeonScroll.enabled = true;
        EventScroll.enabled = true;
        HelpScroll.enabled = true;
        Hard_Scroll.enabled = true;
        Hero_Scroll.enabled = true;

        foreach (GameObject go in ChapterList.cells)
        {
            Destroy(go);
        }
        ChapterList.cells.Clear();
        foreach (GameObject go in DungeonList.cells)
        {
            Destroy(go);
        }
        DungeonList.cells.Clear();
        foreach (GameObject go in EventList.cells)
        {
            Destroy(go);
        }
        EventList.cells.Clear();
        foreach (GameObject go in HelpList.cells)
        {
            Destroy(go);
        }
        HelpList.cells.Clear();
        foreach(GameObject go in HardChapterList.cells)
        {
            Destroy(go);
        }
        HardChapterList.cells.Clear();
        foreach(GameObject go in HeroChapterList.cells)
        {
            Destroy(go);
        }
        HeroChapterList.cells.Clear();
    }

    /// <summary>
    /// switch chapter toggle
    /// 0 normal
    /// 1 hard
    /// 2 hero
    /// 3 event
    /// </summary>
    public void SwitchToggle(int toggleIndex)
    {
        TempDisableToggles();
        switch (toggleIndex)
        {
            case 0:
                {
                    if(chapter_toggle_normal.value)
                    {
                        DelayShowChapter();
                    }
                    break;
                }
            case 1:
                {
                    if (chapter_toggle_hard.value)
                    {
                        DelayShowHardChapter();
                    }
                    break;
                }
            case 2:
                {
                    if (chapter_toggle_hero.value)
                    {
                        DelayShowHeroChapter();
                    }
                    break;
                }
            case 3:
                {
                    if(chapter_toggle_event.value)
                    {
                        DelayShowEventChapter();
                    }
                    break;
                }
            default: break;
        }
    }

    void TempDisableToggles()
    {
        chapter_toggle_normal.GetComponent<BoxCollider>().enabled = false;
        chapter_toggle_hard.GetComponent<BoxCollider>().enabled = false;
        chapter_toggle_hero.GetComponent<BoxCollider>().enabled = false;
        chapter_toggle_event.GetComponent<BoxCollider>().enabled = false;
        Invoke("EnableToggles", 0.5f);
    }

    void EnableToggles()
    {
        chapter_toggle_normal.GetComponent<BoxCollider>().enabled = true;
        chapter_toggle_hard.GetComponent<BoxCollider>().enabled = true;
        chapter_toggle_hero.GetComponent<BoxCollider>().enabled = true;
        chapter_toggle_event.GetComponent<BoxCollider>().enabled = true;
    }

    /// <summary>
    /// 各chapterList渲染
    /// </summary>
    void RenderList(UIScrollView sv, UIGrid gr, DungeonListAnime list, int type)
    {
        //if (type == 3)
        //{
        //    EventScroll.SetDragAmount(0, 0, false);
        //    List<ChapterData> Chapters = ConfigManager.ChapterConfig.GetEventChapters();
        //    for (int i = Chapters.Count - 1; i >= 0; i--)
        //    {
        //        GameObject chapterCell = NGUITools.AddChild(EventGrid.gameObject, ChapterInfoCell);
        //        ChapterCell cc = chapterCell.GetComponent<ChapterCell>();
        //        chapterCell.name = Chapters[i].ChapterId;
        //        cc.SetChapterInfo(Chapters[i]);
        //        cc.ClearRender(UserManager.CurUserInfo.HasClearChapter(Chapters[i].ChapterId));
        //        EventList.cells.Add(chapterCell);
        //    }
        //    EventGrid.Reposition();
        //}
        //else
        //{
        //    //sv.SetDragAmount(0, 0, false);
        //    List<ChapterData> Chapters = ConfigManager.ChapterConfig.GetChaptersByRank(type);
        //    for (int i = Chapters.Count - 1; i >= 0; i--)
        //    {
        //        if (!string.IsNullOrEmpty(Chapters[i].ExDungeon))
        //        {
        //            if (Chapters[i].ExDungeons != null)
        //            {
        //                int count = 0;
        //                foreach (string s in Chapters[i].ExDungeons)
        //                {
        //                    if (UserManager.CurUserInfo.HasClearDungeon(s))
        //                    {
        //                        count++;
        //                    }
        //                }
        //                if (count == Chapters[i].ExDungeons.Length)
        //                {
        //                    GameObject chapterCell = NGUITools.AddChild(gr.gameObject, ChapterInfoCell);
        //                    ChapterCell cc = chapterCell.GetComponent<ChapterCell>();
        //                    chapterCell.name = Chapters[i].ChapterId;
        //                    cc.SetChapterInfo(Chapters[i]);
        //                    cc.ClearRender(UserManager.CurUserInfo.HasClearChapter(Chapters[i].ChapterId));
        //                    list.cells.Add(chapterCell);
        //                }
        //            }
        //            else if (UserManager.CurUserInfo.HasClearDungeon(Chapters[i].ExDungeon))
        //            {
        //                GameObject chapterCell = NGUITools.AddChild(gr.gameObject, ChapterInfoCell);
        //                ChapterCell cc = chapterCell.GetComponent<ChapterCell>();
        //                chapterCell.name = Chapters[i].ChapterId;
        //                cc.SetChapterInfo(Chapters[i]);
        //                cc.ClearRender(UserManager.CurUserInfo.HasClearChapter(Chapters[i].ChapterId));
        //                list.cells.Add(chapterCell);
        //            }
        //        }
        //        else
        //        {
        //            GameObject chapterCell = NGUITools.AddChild(gr.gameObject, ChapterInfoCell);
        //            ChapterCell cc = chapterCell.GetComponent<ChapterCell>();
        //            chapterCell.name = Chapters[i].ChapterId;
        //            cc.SetChapterInfo(Chapters[i]);
        //            cc.ClearRender(UserManager.CurUserInfo.HasClearChapter(Chapters[i].ChapterId));
        //            list.cells.Add(chapterCell);
        //        }
        //    }
        //    gr.Reposition();
        //}

        if (type == 3)
        {
            EventScroll.SetDragAmount(0, 0, false);
            List<ChapterData> Chapters = ConfigManager.ChapterConfig.GetEventChapters();
            List<DungeonData> ChapterDungeons = new List<DungeonData>();
            foreach(ChapterData cd in Chapters)
            {
                List<DungeonData> dungeons = ConfigManager.DungeonConfig.GetChapterDungeons(cd.ChapterId);
                foreach(DungeonData dd in dungeons)
                {
                    ChapterDungeons.Add(dd);
                }
            }

            for (int i = ChapterDungeons.Count - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(ChapterDungeons[i].ExDungeon))
                {
                    if (ChapterDungeons[i].ExDungeons != null)
                    {
                        int count = 0;
                        foreach (string s in ChapterDungeons[i].ExDungeons)
                        {
                            if (UserManager.CurUserInfo.HasClearDungeon(s))
                            {
                                count++;
                            }
                        }
                        if (count == ChapterDungeons[i].ExDungeons.Length)
                        {
                            GameObject dungeonInfoCell = NGUITools.AddChild(EventGrid.gameObject, DungeonInfoCell);
                            DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                            dungeonInfoCell.name = ChapterDungeons[i].Id;
                            dc.SetDungeonInfo(ChapterDungeons[i]);
                            dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                            EventList.cells.Add(dungeonInfoCell);
                        }
                    }
                    else if (UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].ExDungeon))
                    {
                        GameObject dungeonInfoCell = NGUITools.AddChild(EventGrid.gameObject, DungeonInfoCell);
                        DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                        dungeonInfoCell.name = ChapterDungeons[i].Id;
                        dc.SetDungeonInfo(ChapterDungeons[i]);
                        dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                        EventList.cells.Add(dungeonInfoCell);
                    }
                }
                else
                {
                    GameObject dungeonInfoCell = NGUITools.AddChild(EventGrid.gameObject, DungeonInfoCell);
                    DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                    dungeonInfoCell.name = ChapterDungeons[i].Id;
                    dc.SetDungeonInfo(ChapterDungeons[i]);
                    dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                    EventList.cells.Add(dungeonInfoCell);
                }
            }


            EventGrid.Reposition();
        }
        else
        {
            //sv.SetDragAmount(0, 0, false);
            List<ChapterData> Chapters = ConfigManager.ChapterConfig.GetChaptersByRank(type);
            List<DungeonData> ChapterDungeons = new List<DungeonData>();
            foreach(ChapterData cd in Chapters)
            {
                if (!string.IsNullOrEmpty(cd.ExDungeon))
                {
                    if (cd.ExDungeons != null)
                    {
                        int count = 0;
                        foreach (string s in cd.ExDungeons)
                        {
                            if (UserManager.CurUserInfo.HasClearDungeon(s))
                            {
                                count++;
                            }
                        }
                        if (count == cd.ExDungeons.Length)
                        {
                            List<DungeonData> dungeons = ConfigManager.DungeonConfig.GetChapterDungeons(cd.ChapterId);
                            foreach(DungeonData dd in dungeons)
                            {
                                ChapterDungeons.Add(dd);
                            }
                        }
                    }
                    else if (UserManager.CurUserInfo.HasClearDungeon(cd.ExDungeon))
                    {
                        List<DungeonData> dungeons = ConfigManager.DungeonConfig.GetChapterDungeons(cd.ChapterId);
                        foreach (DungeonData dd in dungeons)
                        {
                            ChapterDungeons.Add(dd);
                        }
                    }
                }
                else
                {
                    List<DungeonData> dungeons = ConfigManager.DungeonConfig.GetChapterDungeons(cd.ChapterId);
                    foreach (DungeonData dd in dungeons)
                    {
                        ChapterDungeons.Add(dd);
                    }
                }
            }

            for (int i = ChapterDungeons.Count - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(ChapterDungeons[i].ExDungeon))
                {
                    if (ChapterDungeons[i].ExDungeons != null)
                    {
                        int count = 0;
                        foreach (string s in ChapterDungeons[i].ExDungeons)
                        {
                            if (UserManager.CurUserInfo.HasClearDungeon(s))
                            {
                                count++;
                            }
                        }
                        if (count == ChapterDungeons[i].ExDungeons.Length)
                        {
                            GameObject dungeonInfoCell = NGUITools.AddChild(gr.gameObject, DungeonInfoCell);
                            DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                            dungeonInfoCell.name = ChapterDungeons[i].Id;
                            dc.SetDungeonInfo(ChapterDungeons[i]);
                            dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                            list.cells.Add(dungeonInfoCell);
                        }
                    }
                    else if (UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].ExDungeon))
                    {
                        GameObject dungeonInfoCell = NGUITools.AddChild(gr.gameObject, DungeonInfoCell);
                        DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                        dungeonInfoCell.name = ChapterDungeons[i].Id;
                        dc.SetDungeonInfo(ChapterDungeons[i]);
                        dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                        list.cells.Add(dungeonInfoCell);
                    }
                }
                else
                {
                    GameObject dungeonInfoCell = NGUITools.AddChild(gr.gameObject, DungeonInfoCell);
                    DungeonCell dc = dungeonInfoCell.GetComponent<DungeonCell>();
                    dungeonInfoCell.name = ChapterDungeons[i].Id;
                    dc.SetDungeonInfo(ChapterDungeons[i]);
                    dc.ClearRender(UserManager.CurUserInfo.HasClearDungeon(ChapterDungeons[i].Id));
                    list.cells.Add(dungeonInfoCell);
                }
            }

            gr.Reposition();
        }

    }

    public void ResetToggles()
    {
        if (!chapter_toggle_normal.value)
        {
            Debug.Log("open normal toggle");
            chapter_toggle_normal.value = true;
            Scroll.gameObject.SetActive(true);
        }

        if (chapter_toggle_hard.value)
        {
            Debug.Log("close hard toggle");
            chapter_toggle_hard.value = false;
            Hard_Scroll.gameObject.SetActive(false);
        }

        if (chapter_toggle_hero.value)
        {
            Debug.Log("close hero toggle");
            chapter_toggle_hero.value = false;
            Hero_Scroll.gameObject.SetActive(false);
        }

        if (chapter_toggle_event.value)
        {

            chapter_toggle_event.value = false;
            EventScroll.gameObject.SetActive(false);
        }
    }

    IEnumerator GridReposition(UIGrid ug, float f)
    {
        yield return new WaitForSeconds(f);
        ug.Reposition();
    }
    #endregion
}