using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

//复活
public class Resurrenction : MonoBehaviour
{
    public static int SysType = 1;//1 relive; 2 exit ;3 tip;
    
    public static int Type=1;// 1 ui1；2 ui2

    public GameObject Ui1;
    public GameObject Ui2;

    public GameObject BttmCancel;
    public GameObject BttmConfirm;

    public GameObject BttmExit;
    public GameObject BttmExitCancel;
    public GameObject BttmExitKnow;
    //public GameObject ExitTopMask;
    //public GameObject ExitBttonMask;

	public GameObject GetPetCont;
	public GameObject GetCellCont;

    public InfoLabel resGoldLab;
    public InfoLabel resExpLab;
    public InfoLabel resJewelLab;

    public GameObject Title_text;
    public InfoLabel Infor_text;
    public GameObject Tip_text;

    public GameObject TipPic;
    public GameObject DropIcon;
    public GameObject RetipBg;

    public GameObject ReliveTips;
   
    public PveGameControl GameControl;
    GameObject gameobj;


    public GameObject chongjiuIcon;
    public GameObject chongjiuText;

    int X0 = -1200;


	/// <summary>
    /// 添加素材  @ui2
	/// </summary>
	/// <param name="m"></param>
	void AddMaterial(List<string> m)
	{
        int c=1;
        int cb= PveGameControl.CurBoxNum;
		foreach (string s in m)
		{
            if (c > cb) break;
            if (c > 4) break;

            Transform tt = (Transform)GetCellCont.transform.FindChild("Itemc" + c);
            tt.gameObject.SetActive(true);
            SpriteRenderer sr =(SpriteRenderer) tt.FindChild("Avata").GetComponent<SpriteRenderer>();
            //Debug.Log("Atlas/ItemIcons/Icon" + s.Substring(4));
            sr.sprite = Resources.Load<Sprite>(Tools.GetIconPath(s));
            c++;

		}
	}
	
	/// <summary>
    /// 添加宠物 @ui2
	/// </summary>
	/// <param name="p"></param>
	void AddPet(List<string> p)
	{
        int c = 1;
        int cb = PveGameControl.CurEggNum;
		foreach (string s in p)
		{
            if (c > cb) break;
            if (c > 4) break;

            Transform tt = (Transform)GetPetCont.transform.FindChild("Petc" + c);
            tt.gameObject.SetActive(true);
            SpriteRenderer sr = (SpriteRenderer)tt.FindChild("Avata").GetComponent<SpriteRenderer>();
            SpriteRenderer fr = (SpriteRenderer)tt.FindChild("Frame").GetComponent<SpriteRenderer>();
            PetData pd= ConfigManager.PetConfig.GetPetById(s);
            
            fr.sprite = Resources.Load<Sprite>("Atlas/Fight/ff3-21 (" + ((int)pd.PetPro+3)+")");
            sr.sprite = Resources.Load<Sprite>(Tools.GetIconPath(s));
            sr.sortingOrder = 8;
            fr.sortingOrder = 8;
            fr.transform.localScale=new Vector3(0.85f,0.85f,1);

            /*GameObject ga = NGUITools.AddChild(tt.gameObject, new GameObject());
            SpriteRenderer sr1 = ga.AddComponent<SpriteRenderer>();
            sr1.transform.localPosition = new Vector3(-31, 31, 0);
            sr1.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            sr1.sprite = Resources.Load<Sprite>("Atlas/Fight/pveNewCell/" + (int)pd.PetPro);
            sr1.sortingOrder = 8; 
             */

            c++;
		}
	}


    /// <summary>
    /// 添加TipsIcons @ui1
    /// </summary>
    /// <param name="p"></param>
    void AddTipsIcons(string [] p)
    {
        int c = 1;
        //Debug.Log("tipsIcons Length=" + p.Length);
        foreach (string s in p)
        {
            Debug.Log(s);
            if (c > 4) break;
            Transform tt = (Transform)DropIcon.transform.FindChild("Itemc" + c);

            if (s.Substring(0, 1) == "P")
            {
                //宠
                tt.gameObject.SetActive(true);
                SpriteRenderer sr = (SpriteRenderer)tt.FindChild("Avata").GetComponent<SpriteRenderer>();
                SpriteRenderer fr = (SpriteRenderer)tt.FindChild("Frame").GetComponent<SpriteRenderer>();
                PetData pd = ConfigManager.PetConfig.GetPetById(s);

                fr.sprite = Resources.Load<Sprite>("Atlas/Fight/ff3-21 (" + ((int)pd.PetPro + 3) + ")");
                sr.sprite = Resources.Load<Sprite>(Tools.GetIconPath(s));
                sr.sortingOrder = 8;
                fr.sortingOrder = 8;
                fr.transform.localScale = new Vector3(0.85f, 0.85f, 1);
            }
            else
            {
                //物               
                tt.gameObject.SetActive(true);
                SpriteRenderer sr = (SpriteRenderer)tt.FindChild("Avata").GetComponent<SpriteRenderer>();
                Debug.Log("Atlas/ItemIcons/Icon" + s.Substring(4));
                sr.sprite = Resources.Load<Sprite>(Tools.GetIconPath(s));
            }
            c++;
        }
    }

    void OnEnable()
    {
        gameobj = transform.gameObject;
        DungeonData dungeonData = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);

        if (SysType == 1)
        {
            //relive

            BttmExit.SetActive(false);
            BttmExitKnow.SetActive(false);
            BttmExitCancel.SetActive(false);
            //ExitBttonMask.SetActive(false);
            //ExitTopMask.SetActive(false);
            ReliveTips.SetActive(true);

            BttmCancel.SetActive(true);
            BttmConfirm.SetActive(true);
            RetipBg.SetActive(true);
            

            JsonArray allDrop = (JsonArray)GameControl.CurFloorGetJA;
            OverControl.AddElement(allDrop);
            AddMaterial(OverControl.materialIds);
            AddPet(OverControl.petIds);

            resGoldLab.BeginX = 170;
            resGoldLab.SetNum(PveGameControl.CurTotalGode.ToString());
            resExpLab.BeginX = 170;
            resExpLab.SetNum(dungeonData.Exp.ToString());

            ShowUI1();

            //取消复活  Resurrection
            UIEventListener.Get(BttmCancel).onClick = (go) =>
            {
                if (Type == 2)
                {
                    Resurrenction.Type = 1;
                    AnimationHelper.AnimationMoveTo(new Vector3(X0, 0, 0), gameobj, iTween.EaseType.linear, gameobj, "ShowUI1", 0.2f);
                }
                else
                {
                    //动作
                    Resurrenction.Type = 2;
                    AnimationHelper.AnimationMoveTo(new Vector3(X0, 0, 0), gameobj, iTween.EaseType.linear, gameobj, "ShowUI1", 0.2f);
                }

            };

            //确定复活
            UIEventListener.Get(BttmConfirm).onClick = (go) =>
            {
                if (Resurrenction.Type == 2)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        gameobj.SetActive(false);
                    });
                    GameControl.RenderFail();
                    return;
                }
                SocketCenter.Request(GameRouteConfig.relive, null, (r) =>
                {
                    Debug.Log("--- " + GameRouteConfig.relive);
                    if (r.Code == SocketResult.ResultCode.Success)
                    {
                        //继续战斗
                        Loom.QueueOnMainThread(() =>
                        {
                            GameControl.GoOnWithRelive();
                            UserManager.CurUserInfo.Diamond = UserManager.CurUserInfo.Diamond - (int)ConfigManager.ParamConfig.GetParam().ResurrectionPrice;
                            gameobj.SetActive(false);
                        });
                    }
                }, null, true, true);
                //Debug.Log("Confirm");
            };	 




        }
        else
        {
            //exit
            if (SysType == 2)
            {
                BttmExit.SetActive(true);
                BttmExitKnow.SetActive(false);
                BttmExitCancel.SetActive(true);
            }
            else if (SysType == 3)
            {
                BttmExit.SetActive(false);
                BttmExitKnow.SetActive(true);
                BttmExitCancel.SetActive(false);
            }
            GameControl.UserInputLock = true;

            ReliveTips.SetActive(false);
            //ExitBttonMask.SetActive(true);
            //ExitTopMask.SetActive(true);

            BttmCancel.SetActive(false);
            BttmConfirm.SetActive(false);
            RetipBg.SetActive(false);

            gameobj.transform.localPosition = new Vector3(X0, 0, 0);
            AnimationHelper.AnimationMoveTo(new Vector3(0, 0, 0), gameobj, iTween.EaseType.linear, null, null, 0.2f);


            UIEventListener.Get(BttmExit).onClick = (go) =>
            {
                GameControl.RenderFail();
                gameobj.SetActive(false);                             
            };
            UIEventListener.Get(BttmExitKnow).onClick = (go) =>
            {
                gameobj.SetActive(false);
                GameControl.UserInputLock = false;
            };
            UIEventListener.Get(BttmExitCancel).onClick = (go) =>
            {
                gameobj.SetActive(false);
                GameControl.UserInputLock = false;
            };


        }
        
        TextMesh tempText;
        tempText = Title_text.transform.GetComponent<TextMesh>();
        //Debug.Log(dungeonData.Description);

        tempText.text = dungeonData.Description;


        SetAchievement();


        string bosstips = dungeonData.BossTips;
        SpriteRenderer sr = (SpriteRenderer)TipPic.GetComponent<SpriteRenderer>();
        //Debug.Log(bosstips);
        if(bosstips!="")sr.sprite = Resources.Load<Sprite>("Atlas/BossTips/" + bosstips );

        if (dungeonData.DungeonIcons!=null) AddTipsIcons(dungeonData.DungeonIcons);
                 
	}
    void SetAchievement()
    {
        DungeonData CurDungeonData = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);

        if (UserManager.CurUserInfo.HasAchievedDungeon(CurDungeonData.Id))
        {
            chongjiuIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Atlas/Relive/chongjiuicon2");
        }
        else
        {
            chongjiuIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Atlas/Relive/chongjiuicon1");
        }
        string temps = CurDungeonData.QuestTips;
        string s=temps;
        //Debug.Log("length=  "+temps.Length);
        if (temps.Length > 15)
        {
           s = temps.Substring(0, 15) +"\r\n"+temps.Substring(15);  
        }
        chongjiuText.GetComponent<TextMesh>().text = s;
    }
    void ShowUI1()
    {

		string pa="Atlas/Relive/";
		Sprite tempspr;
		SpriteRenderer conf=BttmConfirm.GetComponent<SpriteRenderer>();
		SpriteRenderer cancel=BttmCancel.GetComponent<SpriteRenderer>();

        TextMesh tempText;

		if( Resurrenction.Type ==1){
            this.Ui2.SetActive(false);
            this.Ui1.SetActive(true);
			string spath2="goon";
			string spath4="notgoon";
			tempspr=Resources.Load<Sprite>(pa+spath2);
			conf.sprite=tempspr;
			tempspr=Resources.Load<Sprite>(pa+spath4);
			cancel.sprite=tempspr;

            Infor_text.BeginX = -170f;
            Infor_text.SetNum(ConfigManager.ParamConfig.GetParam().ResurrectionPrice.ToString());
            tempText = Tip_text.transform.GetComponent<TextMesh>();
            //tempText.text = "是否继续?";

            resJewelLab.BeginX = -36f;
            resJewelLab.SetNum(UserManager.CurUserInfo.Diamond.ToString());

		}
		else if( Resurrenction.Type ==2){
            this.Ui2.SetActive(true);
            this.Ui1.SetActive(false);

			string spath1="queding";			
			string spath3="quxiao";
			tempspr=Resources.Load<Sprite>(pa+spath1);
			conf.sprite=tempspr;
			tempspr=Resources.Load<Sprite>(pa+spath3);
			cancel.sprite=tempspr;
           
            tempText = Tip_text.transform.GetComponent<TextMesh>();
            //tempText.text = "确认放弃冒险？";
            resJewelLab.Clear();            
		}

        gameobj.transform.localPosition = new Vector3(X0, 0, 0);      
        AnimationHelper.AnimationMoveTo(new Vector3(0, 0, 0), gameobj, iTween.EaseType.linear, null, null, 0.2f);
    }
	void Update () {
	
	}
    
}
