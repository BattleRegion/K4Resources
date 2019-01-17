using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;
/// <summary>
/// Guide use
/// </summary>
public class EliminateConfig : GameConfig
{
    public EliminateConfig()
    {
        this.ConfigName = "Eliminate";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            EliminateGuideData m = new EliminateGuideData(data);
            Configs.Add(m);
        }
    }
    public List<EliminateGuideData> GetAllData()
    {
        List<EliminateGuideData> dd = new List<EliminateGuideData>();
        foreach (EliminateGuideData d in Configs)
        {
            dd.Add(d);

        }
        return dd;
    }
    public int GetElementWithXY(string tag, int xp, int yp)
    {
        List<EliminateGuideData> li = GetAllData();
        foreach (EliminateGuideData d in li)
        {
            string s = d.YPosition.Substring(1, 1);
            int yv = int.Parse(s);

            //Debug.Log("tag=" + tag + "   yv=" + yv + "    " + d.Tag + "      " + xp);

            if (tag == d.Tag && yv == yp)
            {
				if(d.Elements[xp]==0){
					return 1;
				}else{
					return d.Elements[xp];
				}
                
            }
        }
        return 1;
    }

}
public class EliminateGuideData : ConfigData
{
    public string Tag;
    public string YPosition;
    //public int Id;
    public int X0;
    public int X1;
    public int X2;
    public int X3;
    public int X4;
    public int X5;
    public int X6;
    public List<int> Elements;


    public EliminateGuideData(JsonObject data = null)
    {
        try
        {
            Tag = data["Tag"].ToString();
            YPosition = data["Yposition"].ToString();
            //Id = int.Parse(data["Idx"].ToString());

            X0 = int.Parse(data["X0"].ToString());
            X1 = int.Parse(data["X1"].ToString());
            X2 = int.Parse(data["X2"].ToString());
            X3 = int.Parse(data["X3"].ToString());
            X4 = int.Parse(data["X4"].ToString());
            X5 = int.Parse(data["X5"].ToString());
            X6 = int.Parse(data["X6"].ToString());


        }
        catch
        {
        }

        Elements = new List<int>();
        Elements.Add(X0);
        Elements.Add(X1);
        Elements.Add(X2);
        Elements.Add(X3);
        Elements.Add(X4);
        Elements.Add(X5);
        Elements.Add(X6);
        //Debug.Log(Elements.Count);
    }
}
