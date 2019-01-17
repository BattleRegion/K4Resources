using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJson;

public class TimeConfig
{
    /// <summary>
    /// 服务器传来的时间
    /// </summary>
    public DateTime ConnectDate;

    /// <summary>
    /// 客户端与服务器同步之后的日期时间
    /// </summary>
    public DateTime LocalTime
    {
        get
        {
            return ConnectDate + LocalTimeAddition;
        }
        set { LocalTime = value; }
    }

    /// <summary>
    /// 客户端与服务器同步之后的星期
    /// </summary>
    public DayOfWeek LocalDayOfWeek
    {
        get
        {
            return LocalTime.DayOfWeek;
        }
        set { LocalDayOfWeek = value; }
    }

    /// <summary>
    /// 客户端增加的时间，用于计算当前时间
    /// </summary>
    public TimeSpan LocalTimeAddition
    {
        get
        {
            return new TimeSpan((long)Time.realtimeSinceStartup * TimeSpan.TicksPerSecond);
        }
        set { LocalTimeAddition = value; }
    }

    public TimeConfig(string date)
    {
        ConnectDate = DateTime.Parse(date);
    }
}
