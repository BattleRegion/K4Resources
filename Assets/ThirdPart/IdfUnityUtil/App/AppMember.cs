using UnityEngine;
using System.Collections;

public class AppMember  
{
    /// <summary>
    /// 帐号类型
    /// </summary>
    public enum AccountType
    {
        Normal = 1,
        Anonymous = 2,
        Machine = 3
    }

    /// <summary>
    /// member 凭据
    /// </summary>
    static public string Cre;

    /// <summary>
    /// id
    /// </summary>
    static public int MemberId;

    /// <summary>
    /// 帐号
    /// </summary>
    static public string Account;

    /// <summary>
    /// 帐号类型
    /// </summary>
    static public AccountType Type;
}
