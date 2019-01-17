using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;
using System;

public class GuiderLocal : MonoBehaviour
{
    List<GuiderData> GuiderList;
    List<GameObject> CurList;
    public GuiderData CurGuiddata;
    //主场景内引导
    GameObject GuiderNGUI; //(透明黑底 文本 箭头)
    GameObject AlphaBG;//透明遮罩层
    UILabel MessageLabel;  //文本   
    GameObject MessageLabelBg;
    GameObject DirOb;      //箭头
    GameObject CharGirl;

    float SpaceTime=2f;//间隔时间按 秒

    public int curId;      //引导id
    //pve
    public PveGameControl gamecontrol;
    public GameObject HandOb;  //手
    GameObject PropertyOb;

    Vector3 MessageHoldPostion;
    Vector3 CharGirlHoldPostion;

    Action tempAction;

    iTween Girl_x_itween;
    iTween Girl_y_itween;

    
    void OnEnable()
    {
        GuiderLocal.GuiderLocalInstance = this;
        CurList = new List<GameObject>();
        GuiderList = ConfigManager.GuiderConfig.GetAllData();

        GuiderNGUI = transform.FindChild("GuidBgNGUI").gameObject;
        MessageLabel = GuiderNGUI.transform.FindChild("GMessageLabel").GetComponent<UILabel>();
        AlphaBG = GuiderNGUI.transform.FindChild("AlphaBG").gameObject;
        MessageLabelBg = GameObject.Find("talkBG");
        DirOb = GuiderNGUI.transform.FindChild("GuiderDir").gameObject;
        CharGirl = GuiderNGUI.transform.FindChild("charaGirl").gameObject;
        HandOb = GuiderNGUI.transform.FindChild("hand").gameObject;
        PropertyOb = GuiderNGUI.transform.FindChild("guiderproperty").gameObject;

        DirOb.SetActive(false);
        HandOb.SetActive(false);
        MessageLabelBg.SetActive(false);
        MessageLabel.text = "";

        MessageHoldPostion = MessageLabel.transform.position;
        CharGirlHoldPostion = CharGirl.transform.position;

        
        girlv1 = CharGirl.transform.position;
        //ITween_chargirl();

        ITween_chargirl_x();

        Debug.Log(girlv1.x);
        Debug.Log(girlv1.y);

        Invoke("ShowCurGuider", 0.2f);
        
        UIWidget cuw = CharGirl.transform.GetComponent<UIWidget>();
        cuw.depth = 35;
        //temp--      
        BoxCollider bc = CharGirl.transform.GetComponent<BoxCollider>();
        bc.center = new Vector3(0,110,0);
        bc.size = new Vector3(200, 170, 0);
        UIEventListener.Get(CharGirl).onClick = (g) =>
        {
            Debug.Log("...");
            UserManager.CurUserInfo.GuideId = -1;
            PlayerPrefs.SetInt("GuideID", UserManager.CurUserInfo.GuideId);
            if (gamecontrol != null) gamecontrol.GuiderLocalMa = null;
            Destroy(gameObject);
        };
    }
    //结合其他模块
    public static GuiderLocal GuiderLocalInstance;
    public static void GuiderNext()
    {
        GuiderLocalInstance.GuiderAddAction();
    }

    void ITween_chargirl()
    {      
        //if (Girl_y_itween)
        //{
        //    Girl_y_itween.enabled = true;
        //}
        //else
        //{
        if (Girl_y_itween) Girl_y_itween.enabled = false;
            Hashtable a1 = new Hashtable();
            a1.Add("loopType", "pingPong");
            a1.Add("easeType", iTween.EaseType.linear);
            a1.Add("time", 1.3f);
            a1.Add("delay", 0.1f);
            Vector3 v1 = new Vector3(0f, 0.03f, 0);
            a1.Add("position", CharGirl.transform.position + v1);
            iTween.MoveTo(CharGirl, a1);
            Girl_y_itween = CharGirl.GetComponent<iTween>();
        //}

       
    }
    Vector3 girlv1 ;
    Vector3 girlv2 = new Vector3(-0.7f, 0, 0);
    public bool girlBool = true;
    void ITween_chargirl_x()
    {
        Vector3 tempv;
      
        if(girlBool){
            tempv = girlv1+girlv2;
        }else{
            tempv=girlv1;
        }
        girlBool = !girlBool;

        if (Girl_y_itween) Girl_y_itween.enabled = false;

        Hashtable a1 = new Hashtable();
        a1.Add("easeType", iTween.EaseType.linear);
        a1.Add("time", 0.3f);
        a1.Add("delay", 0.03f);
        a1.Add("position", tempv);
        a1.Add("oncompletetarget", gameObject);
        a1.Add("oncomplete", "Girl_X_callback");
        iTween.MoveTo(CharGirl, a1);
        Girl_x_itween = CharGirl.GetComponent<iTween>();
    }
    void Girl_X_callback()
    {
        //ITween_chargirl();
    }
    void ITween_dirob()
    {
        iTween.Stop(DirOb);
        Hashtable a1 = new Hashtable();
        a1.Add("loopType", "pingPong");
        a1.Add("easeType", iTween.EaseType.easeOutQuad);
        a1.Add("time", 0.2f);
        a1.Add("delay", 0.1f);
        Vector3 v1 = new Vector3(0f, 0.05f, 0);
        a1.Add("position", DirOb.transform.position + v1);
        iTween.MoveTo(DirOb, a1);
    }
    void ITween_messBG()
    {
        //iTween.Stop(MessageLabelBg);
        MessageLabelBg.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        Hashtable ht = new Hashtable();
        ht.Add("scale", new Vector3(1f, 1f, 1f));
        ht.Add("time", 0.1f);
        ht.Add("oncompletetarget", gameObject);
        ht.Add("oncomplete", "SetMessage");
        iTween.ScaleTo(MessageLabelBg, ht);
    }
    void ITween_LightObject(GameObject gb)
    {
        gb.transform.localScale = new Vector3(1.1f, 1.1f, 1);
        Hashtable ht = new Hashtable();
        ht.Add("loopType", "pingPong");
        ht.Add("scale", new Vector3(1f, 1f, 1f));
        ht.Add("time", 0.8f);
        ht.Add("easeType", iTween.EaseType.linear);
        
        iTween.ScaleTo(gb, ht);
    }
    

    //主场景
    public static void MainSceneGuid()
    {
        int guidid = UserManager.CurUserInfo.GuideId;
        bool b = false;
        if (guidid > 0 && guidid < 16)
        {
            //首次副本前
            b= true;
        }
        else if (guidid >= 43 && guidid < 61)
        {
            //首次副本后 
            b= true;
        }

        if (b)
        {
            GameObject gp = GameObject.Find("Camera");
            gp.transform.Find("GuiderPanel").gameObject.SetActive(true);
        }

    }

    //结算面板和竞技场 
    public static void TriggerGuideWithOut(string s)
    {
        GameObject gp;
        if (s == "overWar")
        {
            //结算面板关闭
            if (UserManager.CurUserInfo.GuideId == 42)
            {
                gp = GameObject.Find("Camera");
				gp.transform.Find("GuiderPanel").gameObject.SetActive(true);               
            }
        }
        else if (s == "openArena")
        {
            //打开竞技场
            if (UserManager.CurUserInfo.Level >= ConfigManager.ParamConfig.GetParam().ArenaOpenLv && UserManager.CurUserInfo.GuideId == 67)
            {
				UserManager.CurUserInfo.GuideId++;
                gp = GameObject.Find("Camera");
				gp.transform.Find("GuiderPanel").gameObject.SetActive(true);              
            }
        }
    }
    public static bool TriggerPveSencond()
    {
        return true;
    }
    //首次pve
    public static bool TriggerPve()
    {
        if (UserManager.CurUserInfo.GuideId == 66 && PveGameControl.CurDungeonId == "D1_2")
        {
           
            return true;
        }


        if (UserManager.CurUserInfo.GuideId > 12 && UserManager.CurUserInfo.GuideId <40 && PveGameControl.CurDungeonId == "D1_1")
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    void SetMessage()
    {
        MessageLabel.text = CurGuiddata.Description;
    }
    public void BackPveAction()
    {
        GuiderNGUI.SetActive(true);
        ShowCurGuider();
    }
    void SendStepToServer(int n)
    {
        JsonObject args = new JsonObject();
        args.Add("step", n);
        SocketCenter.Request(GameRouteConfig.guideStep, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Debug.Log(r);
            }
        }, null, true, true);
    }
    public void ShowCurGuider()
    {
        

        Debug.Log("GuideId " + UserManager.CurUserInfo.GuideId);

        //send x to server ,keep step. 后端存储 GuidId 点
        if (UserManager.CurUserInfo.GuideId == 9 ||
            UserManager.CurUserInfo.GuideId == 16 ||
            UserManager.CurUserInfo.GuideId == 42 ||
            UserManager.CurUserInfo.GuideId == 43 ||
            UserManager.CurUserInfo.GuideId == 50 ||
            UserManager.CurUserInfo.GuideId == 57 ||
            UserManager.CurUserInfo.GuideId == 60 ||
            UserManager.CurUserInfo.GuideId == 64 ||
            UserManager.CurUserInfo.GuideId == 67 ||
            UserManager.CurUserInfo.GuideId == 70
            )
        {
            SendStepToServer(UserManager.CurUserInfo.GuideId);
        }
        
        if (UserManager.CurUserInfo.GuideId > GuiderList.Count || UserManager.CurUserInfo.GuideId == -1)
        {
            //send  101 to server ,end guide step;
            SendStepToServer(101);
            Debug.Log("Guide End");
            UserManager.CurUserInfo.GuideId = -1;
            PlayerPrefs.SetInt("GuideID", UserManager.CurUserInfo.GuideId);
            foreach (GameObject g in CurList)
            {
                Destroy(g);
            }
            Destroy(gameObject);
            return;
        }
        curId = UserManager.CurUserInfo.GuideId;
        PlayerPrefs.SetInt("GuideID", curId);

        GuiderData gd = GuiderList[curId - 1];
        CurGuiddata = gd;

        if (gd.Scene == "")
        {
            //空行 跳到下条
            //GuiderAddAction();
            UserManager.CurUserInfo.GuideId++;
            ShowCurGuider();
            return;
        }

        if (gd.Scene != Application.loadedLevelName ) return;
        SpaceTime = gd.DelayTime;


        //对话
        if (!string.IsNullOrEmpty(gd.Description))
        {
            MessageLabel.text = "";
            MessageLabelBg.SetActive(true);
            //if (MessageLabelBg.active == false)
            //{
            //    ITween_messBG();
            //}
            //else
            //{
            MessageLabel.text = CurGuiddata.Description;
            //}           
            SetBgTouchEnable(true);
            HideBGAndCharGirl(true);
            CharGirl.SetActive(true);

            if (girlBool) ITween_chargirl_x();
            //ITween_chargirl();
        }
        else
        {
            MessageLabel.text = "";
            MessageLabelBg.SetActive(false);
            if (!girlBool) ITween_chargirl_x();
        }


        if (gd.Scene == "MainScene" && Application.loadedLevelName == "MainScene" && curId < GuiderList.Count && GuiderList[curId].Scene == "Pve")
        {
            Debug.Log("go to pve");
            //UserManager.CurUserInfo.GuideId++;
        }


        if (gd.Scene == "Pve")
        {
            clearCurList();
            gamecontrol.SetEliminatesCollider(false);
            PveGuide();
        }
        else if (gd.Scene == "MainScene")
        {
            MainSceneGuide();
        }
    }

    //PVE

    List<Vector2> Routs = new List<Vector2>();
    void PveGuide()
    {       
        //连线
        GuiderData gd = CurGuiddata;          
        if (gd.Tag == "b1")
        {
            PropertyOb.SetActive(true);
            //HideBGAndCharGirl(false);
            CharGirl.SetActive(false);
            GameObject confirm = PropertyOb.transform.FindChild("guiderconfirm").gameObject;
            UIEventListener.Get(confirm).onClick = (g) =>
            {
                UserManager.CurUserInfo.GuideId++;
                girlBool = true;
                ShowCurGuider();
            };
            SetBgTouchEnable(true);
        }
        else
        {
            PropertyOb.SetActive(false);
        }

        if (gd.Condition == "hand")
        {
            Routs.Clear();
            string[] routes = gd.Route.Split('|');
            foreach (string s in routes)
            {
                int tx = int.Parse(s.Substring(1, 1));
                int ty = int.Parse(s.Substring(3, 1));
                // Debug.Log(tx+"    "+ty);
                Vector2 v2 = new Vector2(tx, ty);
                Routs.Add(v2);
            }
            //HandOb.SetActive(true);
            DirOb.SetActive(false);
            HideBGAndCharGirl(false);
            UITexture ut = HandOb.transform.GetComponent<UITexture>();
            gamecontrol.GuideLineEliminates(Routs);
        }
        else if (gd.Condition == "arrow")
        {            
            HideBGAndCharGirl(true);
            LightClicOper();
        }
    }
    void HideBGAndCharGirl(bool b)
    {
       //if(CharGirl) CharGirl.SetActive(b);       
        AlphaBG.transform.GetComponent<UISprite>().enabled = b;
    }

    //主场景
    void MainSceneGuide()
    {
        LightClicOper();
    }
    void LightClicOper()
    {
        GuiderData gd = CurGuiddata;
        GameObject c0 = GameObject.Find(gd.LightObjectName);

        if(gd.Tag=="d3") Debug.Log(GameObject.Find("PartyBoard_2"));
        if (c0 == null && gd.LightObjectName!="") Debug.Log(gd.LightObjectName + " 找不到 ");

        if (c0 != null)
        {
            Debug.Log(gd.LightObjectName+" ... ");
            if (c0.GetComponent<iTween>() != null)
            {
                DestroyImmediate(c0.GetComponent<iTween>());
            }

            GameObject cl = GameObject.Instantiate(c0) as GameObject;
            cl.transform.parent = transform;
            cl.transform.position = c0.transform.position;
            cl.transform.localScale = new Vector3(1, 1, 1);
            
            CurList.Add(cl);
            
            UIWidget ud = cl.GetComponent<UIWidget>();
            if (ud != null)
            {
                ud.depth = 3;
            }
            else
            {
                ud = cl.AddComponent<UIWidget>();
                ud.depth = 3;
                ud.autoResizeBoxCollider = false;
            }
            UIButton ub = cl.GetComponent<UIButton>();

            //数据传递
            if (gd.Tag == "c1" || gd.Tag == "d7")
            {
                //选择宠物编组
                ItemInterface cc = c0.GetComponent<ItemInterface>();
                ItemInterface c1c = cl.GetComponent<ItemInterface>();
                c1c.itemInter = cc.itemInter;
                cl.transform.localPosition = new Vector3(-213f, 103f, 0f);

            }
            else if (gd.Tag == "c2" || gd.Tag == "d10")
            {
                //---废除 2015-05-08
                //选关卡（新手之路）

                //ChapterCell cc = c0.GetComponent<ChapterCell>();
                //ChapterCell c1c = cl.GetComponent<ChapterCell>();
                //c1c.CurChapterData = cc.CurChapterData;
                //cl.transform.localPosition = new Vector3(-0f, 92f, 0f);
            }
            else if (gd.Tag == "c3" || gd.Tag == "d11")
            {
                //子关卡
                DungeonCell cc = c0.GetComponent<DungeonCell>();
                DungeonCell c1c = cl.GetComponent<DungeonCell>();
                c1c.CurDungeonData = cc.CurDungeonData;
                //cl.transform.localPosition = new Vector3(-0f, 141f, 0f);
                //gd.DelayTime = 0f;
                cl.transform.localPosition = new Vector3(-0f, 94f, 0f);
            }
            else if (gd.Tag == "c4" || gd.Tag == "d12")
            {
                //好友怪              
                cl.transform.localPosition = new Vector3(-0f, 141f, 0f);
            }
            else if (gd.Tag == "t7" || gd.Tag == "d6")
            {
                cl.GetComponent<ButtonAddMonster>().position = c0.GetComponent<ButtonAddMonster>().position;
                cl.GetComponent<ButtonAddMonster>().blankInter = c0.GetComponent<ButtonAddMonster>().blankInter;
            }
            else if (gd.Tag == "t6" || gd.Tag == "t12" || gd.Tag == "m2" || gd.Tag == "d2")
            {
                //ub 在子物体上 
                GameObject gob = cl.transform.FindChild("Sprite").gameObject;
                gob.GetComponent<UIWidget>().depth = 3;
                ub = gob.GetComponent<UIButton>();         

                 if (gd.Tag == "d2")
                 {
                     cl.transform.localPosition = new Vector3(-12f, 136f, 0f);
                 }            
            }
            else if (gd.Tag == "p26")
            {
                //无 UIButton
                PetAvata cc = c0.GetComponent<PetAvata>();
                PetAvata c1c = cl.GetComponent<PetAvata>();
                c1c.CurPet = cc.CurPet;
                Tools.SetLayer(cl.transform, gameObject.layer);
                ub = null;

                UIEventListener uis0 = c0.GetComponent<UIEventListener>();
                UIEventListener uis = cl.GetComponent<UIEventListener>();
                uis.onClick = (go) =>
                {
                    uis0.onClick(c0);
                    GuiderAddAction();
                };
            }
            else if (gd.Tag == "p7")
            {
                Tools.SetLayer(cl.transform, gameObject.layer);
            }           
            else if (gd.Tag == "p27")
            {
                Tools.SetLayer(cl.transform, gameObject.layer);

                UIEventListener uis0 = c0.GetComponent<UIEventListener>();
                UIEventListener uis = cl.GetComponent<UIEventListener>();
                ub = null;
                uis.onClick = (go) =>
                {
                    HideBGAndCharGirl(false);
                    uis0.onClick(c0);
                    cl.SetActive(false);
                    GuiderAddAction();
                };
            }
            else if ( gd.Tag == "m8")
            {
                //锻造后面板
                ud.alpha = 0.01f;                
                UIEventListener uis0 = c0.GetComponent<UIEventListener>();
                UIEventListener uis = cl.GetComponent<UIEventListener>();
                ub = null;
                SetBgTouchEnable(false);
                uis.onClick = (go) =>
                {
                    uis0.onClick(c0);                  
                };
            }else if(gd.Tag=="a3"){
                // a3,a5,a6 miss ?
                UIEventListener uis = cl.GetComponent<UIEventListener>();
                ub = null;
                uis.onClick = (go) =>
                {
                    //uis0.onClick(c0);
                    OverControl oc= GameObject.Find("DungeonOver").transform.GetComponent<OverControl>();
                    if (oc != null) oc.CloseThisUI();
                    GuiderAddAction();
                };

            }else if(gd.Tag=="m4"){
                cl.GetComponent<EquipmentMenuItemInterface>().equipInter = c0.GetComponent<EquipmentMenuItemInterface>().equipInter;
                ud.depth = 2;
            }
            else if (gd.Tag == "m5")
            {
				//锻造（武器）
				cl.GetComponent<ButtonMakeWeapon>().makeWeaponInter = c0.GetComponent<ButtonMakeWeapon>().makeWeaponInter;
            }
            else if (gd.Tag == "m3")
            {
                //选择“远程武器”按钮
                cl.transform.localPosition= new Vector3(140f,185f,0f);
            } 
            else if (gd.Tag == "d3")
            {
                //穿武器
                PlayerWeaponController cc = c0.GetComponent<PlayerWeaponController>();
                PlayerWeaponController c1c = cl.GetComponent<PlayerWeaponController>();
                c1c.weaponInter = cc.weaponInter;
            }
            else if (gd.Tag == "d4")
            {
                //选择武器
                equipmentItemInterface cc = c0.GetComponent<equipmentItemInterface>();
                equipmentItemInterface c1c = cl.GetComponent<equipmentItemInterface>();
                c1c.equipmentItemInter = cc.equipmentItemInter;
            }
            else if (gd.Tag == "f1" || gd.Tag == "f4")
            {
                //pve挑战按钮
                cl.GetComponent<UISprite>().enabled = true;
                ub = null;

                UIEventListener uis0 = c0.GetComponent<UIEventListener>();
                UIEventListener uis = cl.GetComponent<UIEventListener>();
                K4Input.inputLock = true;
                uis.onClick = (go) =>
                {
                    uis0.onClick(c0);
                    GuiderAddAction();
                    K4Input.inputLock = false;
                };
            }else if(gd.Tag == "f3" ){
                //选择编组 右箭头
                ub = null;
                UIEventListener uis0 = c0.GetComponent<UIEventListener>();
                UIEventListener uis = cl.GetComponent<UIEventListener>();
                K4Input.inputLock = true;
                uis.onClick = (go) =>
                {
                    uis0.onClick(c0);
                    cl.GetComponent<BoxCollider>().enabled = false;
                    //GuiderAddAction();
                    //K4Input.inputLock = false;
                };
            }
            else if (gd.Tag == "f2") {
                K4Input.inputLock = true;
            }

            ITween_LightObject(cl);

            if (gd.Condition == "arrow" && c0 != null)
            {
                //显示箭头
                SetBgTouchEnable(false);
                DirOb.SetActive(true);
                //iTween.PunchScale(DirOb, new Vector3(1f, 0.6f, 1f), 3f);
                DirOb.transform.position = cl.transform.position + new Vector3(0, 0.18f, 0);
                ITween_dirob();
            }
            else
            {
                DirOb.SetActive(false);
                SetBgTouchEnable(true);
            };
           
            //--------------------------------------------------------------------------

            if ( gd.Tag == "p7" || gd.Tag == "a2")
            {
                SetBgTouchEnable(true);
            }
            else
            {
                if(gd.LightObjectName!="" && gd.DelayTime ==0f && gd.Scene=="MainScene")
                {
                    //外部触发下一步
                    Debug.Log("外部触发  "+gd.LightObjectName);
                                    
                    PartyAnime.moveEnd = GuiderAddAction;
                    
                    if (ub != null)
                    {
                        //去除当前点击按钮
                        List<EventDelegate> list = ub.onClick;
                        EventDelegate ed = new EventDelegate(this, "ReSetUi");
                        list.Add(ed);
                    }
                }
                else 
                {
                    if (ub != null)
                    {
                        //绑定延时，内部触发
                        List<EventDelegate> list = ub.onClick;                       
                        EventDelegate ed = new EventDelegate(this, "GuiderAddAction");
                        list.Add(ed);
                    }
                }
            }
        }
    }
    IEnumerator DelayExit_show()
    {
        yield return new WaitForSeconds(SpaceTime);
        ShowCurGuider();
    }
    public void DelayShowNextStep()
    {
       if(gameObject!=null) StartCoroutine(DelayExit_show());
    }
    public void GuiderAddAction()
    {
        ReSetUi();
        Debug.Log("GuideId++");
        UserManager.CurUserInfo.GuideId++;
        DelayShowNextStep();
    }
    void ReSetUi()
    {
        clearCurList();
        SetBgTouchEnable(false);
        if(DirOb!=null)DirOb.SetActive(false);
    }
    void SetBgTouchEnable(bool b)
    {
        if (AlphaBG == null) return;
        if (b)
        {
            UIEventListener.Get(AlphaBG).onClick = (g) =>
            {
                if (CurGuiddata.Tag == "f5")
                {
                    //Debug.Log("------"+gameObject);
                    gamecontrol.UserInputLock = false;
                    gamecontrol.SetEliminatesCollider(true);
                    gamecontrol.FirstLineOver = true;
                    Destroy(gameObject);
                    //gameObject.SetActive(false);                  
                }
                UserManager.CurUserInfo.GuideId++;
                ShowCurGuider();
            };
        }
        else
        {
            UIEventListener.Get(AlphaBG).onClick = (g) =>
            {
                Debug.Log("bg ");
            };
        }
    }

    void OnDestroy()
    {
        PartyAnime.moveEnd = null;           
    }    
    public void clearCurList()
    {
        foreach (GameObject g in CurList)
        {
           //if (g != null)  DestroyImmediate(g);              
           g.transform.position = new Vector3(-30f, 0, 0);
           iTween.Stop(g);
        }
        CurList.Clear();
    }
}