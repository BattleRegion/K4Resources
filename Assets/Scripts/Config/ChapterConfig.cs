using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;
using System;

public class ChapterConfig : GameConfig
{

    public ChapterConfig()
    {
        this.ConfigName = "Chapter";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            ChapterData m = new ChapterData(data);
            Configs.Add(m);
        }
    }

    /// <summary>
    /// 获取ChapterData
    /// </summary>
    public ChapterData GetChapterData(string ChapterId)
    {
        foreach (ChapterData cd in Configs)
        {
            if (cd.ChapterId == ChapterId) return cd;
        }
        return null;
    }

    public List<ChapterData> GetAllData()
    {
        List<ChapterData> dd = new List<ChapterData>();
        foreach (ChapterData d in Configs)
        {
            if (!d.IsEvent && !d.IsPvp) dd.Add(d);
        }
        return dd;
    }


    public List<ChapterData> GetChaptersByRank(int type)
    {
        switch(type)
        {
            case 0: return GetAllNormalChaps();
            case 1: return GetAllHardChaps();
            case 2: return GetAllHeroChaps();
            case 3: return GetEventChapters();
            default: return null;
        }
    }

    public List<ChapterData> GetAllNormalChaps()
    {
        List<ChapterData> cd = new List<ChapterData>();
        foreach (ChapterData c in Configs)
        {
            if (!c.IsEvent && !c.IsPvp && c.Rank == 1) cd.Add(c);
        }
        return cd;
    }

    public List<ChapterData> GetAllHardChaps()
    {
        List<ChapterData> cd = new List<ChapterData>();
        foreach (ChapterData c in Configs)
        {
            if (!c.IsEvent && !c.IsPvp && c.Rank == 2) cd.Add(c);
        }
        return cd;
    }

    public List<ChapterData> GetAllHeroChaps()
    {
        List<ChapterData> cd = new List<ChapterData>();
        foreach (ChapterData c in Configs)
        {
            if (!c.IsEvent && !c.IsPvp && c.Rank == 3) cd.Add(c);
        }
        return cd;
    }

    /// <summary>
    /// 获取活动副本
    /// </summary>
    /// <returns></returns>
    public List<ChapterData> GetEventChapters()
    {
        List<ChapterData> ed = new List<ChapterData>();
        foreach (ChapterData d in Configs)
        {
            if (d.IsEvent)
            {
                if (TryEnter(d))
                {
                    ed.Add(d);
                }
                else if (TryShow(d))
                {
                    ed.Add(d);
                }
            }
        }
        return ed;
    }

    /// <summary>
    /// 确定活动副本是否可进入
    /// </summary>
    /// <param name="chd"></param>
    /// <returns></returns>
    public bool TryEnter(ChapterData chd)
    {
        if ((ConfigManager.LocalTime.LocalTime >= chd.StartDate) && (ConfigManager.LocalTime.LocalTime <= chd.EndDate))
        {
            if (chd.OpenWeek.Contains(ConfigManager.LocalTime.LocalDayOfWeek))
            {
                if (chd.OpenHour.Contains(ConfigManager.LocalTime.LocalTime.Hour))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 确定活动副本是否显示
    /// </summary>
    /// <param name="chd"></param>
    /// <returns></returns>
    public bool TryShow(ChapterData chd)
    {
        //DateTime dt = ConfigManager.LocalTime.LocalTime + new TimeSpan(TimeSpan.TicksPerDay);
        if ((ConfigManager.LocalTime.LocalTime >= chd.StartDate) && (ConfigManager.LocalTime.LocalTime <= chd.EndDate))
        {
            if (chd.OpenWeek.Contains(ConfigManager.LocalTime.LocalTime.DayOfWeek))
            {
                if (ConfigManager.LocalTime.LocalTime.Hour <= chd.OpenHour[chd.OpenHour.Count - 1])
                {
                    return true;
                }
            }
        }
        return false;
    }
}

public class ChapterData : ConfigData
{
    public string ChapterId;

    public string ChapterName;

    public string ExDungeon;

    public string[] ExDungeons;

    public int SortId;

    public string ChapterIcon;

    public bool IsEvent;

    public bool IsPvp;

    public int Rank;

    public bool show
    {
        get { return ConfigManager.ChapterConfig.TryShow(this); }
        set { show = value; }
    }

    public bool enter
    {
        get { return ConfigManager.ChapterConfig.TryEnter(this); }
        set { enter = value; }
    }

    public int TimeChapter; //0普通副本，1时限副本，2PVP副本

    /// <summary>
    /// 开放日期
    /// </summary>
    public string OpenDateJson;

    private DateTime startDate;
    public DateTime StartDate
    {
        get
        {
            if (IsEvent)
            {
                string[] date = OpenDateJson.Split(',');
                startDate = DateTime.Parse(date[0].ToString());
            }
            return startDate;
        }
        set
        {
            startDate = value;
        }
    }

    private DateTime endDate;
    public DateTime EndDate
    {
        get
        {
            if (IsEvent)
            {
                string[] date = OpenDateJson.Split(',');
                endDate = DateTime.Parse(date[1].ToString());
            }
            return endDate;
        }
        set
        {
            endDate = value;
        }
    }

    /// <summary>
    /// 每周几开放
    /// </summary>
    public string OpenWeekJson;

    public List<DayOfWeek> OpenWeek = new List<DayOfWeek>();

    /// <summary>
    /// 每日开放时间
    /// </summary>
    public string OpenHourJson;

    public List<int> OpenHour = new List<int>();

    public ChapterData(JsonObject data)
    {
        try
        {
            Rank = int.Parse(data["Rank"].ToString());
            ChapterId = data["ChapterId"].ToString();
            ChapterName = data["ChapterName"].ToString();
            ExDungeon = data["ExDungeon"].ToString();
            if(!string.IsNullOrEmpty(ExDungeon))
            {
                ExDungeon = data["ExDungeon"].ToString();
                if (ExDungeon.Contains(","))
                {
                    ExDungeons = ExDungeon.Split(',');
                }
                else
                {
                    ExDungeons = null;
                }
            }
            else
            {
                ExDungeons = null;
            }
            SortId = int.Parse(data["SortId"].ToString());
            ChapterIcon = data["ChapterIcon"].ToString();
            TimeChapter = int.Parse(data["TimeChapter"].ToString());
            switch (TimeChapter)
            {
                case 0:
                    {
                        IsEvent = false;
                        IsPvp = false;
                        break;
                    }
                case 1:
                    {
                        IsEvent = true;
                        IsPvp = false;
                        break;
                    }
                case 2:
                    {
                        IsEvent = false;
                        IsPvp = true;
                        break;
                    }
                default: break;
            }
            OpenDateJson = data["OpenDate"].ToString();
            OpenWeekJson = data["OpenWeek"].ToString();
            OpenHourJson = data["OpenHour"].ToString();

            if (IsEvent)
            {
                string[] weekdays = OpenWeekJson.Split(',');
                foreach (string s in weekdays)
                {
                    if (s.Length > 1)
                    {
                        string[] days = s.Split('-');
                        for (int i = int.Parse(days[0]); i <= int.Parse(days[1]); i++)
                        {
                            OpenWeek.Add((DayOfWeek)i);
                        }
                    }
                    else
                    {
                        OpenWeek.Add((DayOfWeek)int.Parse(s));
                    }
                }

                string[] openhours = OpenHourJson.Split(',');
                foreach (string s in openhours)
                {
                    if (s.Length > 1)
                    {
                        string[] hours = s.Split('-');
                        for (int i = int.Parse(hours[0]); i < int.Parse(hours[1]); i++)
                        {
                            OpenHour.Add(i);
                        }
                    }
                    else
                    {
                        OpenHour.Add(int.Parse(s));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            throw e;
        }
    }
}
