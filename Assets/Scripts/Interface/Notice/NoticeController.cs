using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJson;

/// <summary>
/// 公告
/// </summary>
public class NoticeController : MonoBehaviour
{
    public GameObject NoticeBoard;
    public UILabel NoticeContext;
    public GameObject CloseButton;
    public UISprite NewTag;
    //public UILabel Publisher;
    //public UILabel Time;

    int NoticeId;

    public void RequestNoticeInfo()
    {
        ApplicationControl.CurApp.BeginLoading();
        HttpHelper.GETRequest("http://" + LoginControl.LoginHost + RequestRoute.NoticeRoute, (r) =>
        {
            Debug.Log(r.Data);
            JsonObject data = (JsonObject)r.Data["data"];
            JsonArray noticeList = (JsonArray)data["list"];
            Loom.QueueOnMainThread(() =>
            {
                ApplicationControl.CurApp.StopLoading();
                if(noticeList.Count > 0)
                {
                    JsonObject n = (JsonObject)noticeList[0];

                    NoticeId = int.Parse(n["notice_id"].ToString());
                    NoticeContext.text = n["context"].ToString();
                    EnableNoticeBoard();
                }
            });
        });
    }

    void EnableNoticeBoard()
    {
        if(PlayerPrefs.GetInt("notice_id") == NoticeId)
        {
            NewTag.gameObject.SetActive(false);
        }
        else
        {
            NewTag.gameObject.SetActive(true);
        }

        NoticeBoard.SetActive(true);
        NoticeBoard.transform.localScale = Vector3.zero;
        AnimationHelper.AnimationScaleTo(new Vector3(1, 1, 1), NoticeBoard, iTween.EaseType.spring, null, null, 0.2f);
    }

    void Start()
    {
        UIEventListener.Get(CloseButton).onClick = (g) => 
        {
            PlayerPrefs.SetInt("notice_id", NoticeId);
            NoticeBoard.SetActive(false);
        };
    }
}
