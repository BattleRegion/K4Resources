using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;
public class MapConfig : GameConfig 
{
    public MapConfig()
    {
        this.ConfigName = "Map";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            MapElementData element = new MapElementData(data);
            Configs.Add(element);
        }
    }

    public List<MapElementData> GetElementsData(string dungeonId, int floor)
    {
        List<MapElementData> elementsData = new List<MapElementData>();
        foreach (MapElementData elementData in Configs)
        {
            if (elementData.DungeonId == dungeonId && elementData.Floor == floor)
            {
                elementsData.Add(elementData);
            }
        }
        return elementsData;
    }
}

public class MapElementData:ConfigData
{
    public enum ElementType
    {
        Barrier = 1,
        Item =2,
        Boss =3,
        Monster =4,
        Trap = 5,
        DontDe=7
    }
    public string DungeonId;
    public string ElementId;
    public int XPosition;
    public int YPosition;
    public int Floor;
    public ElementType Type;
    public int Xrange;
    public int Yrange;
    public MapElementData(JsonObject data)
    {
        DungeonId = data["DungeonId"].ToString();
        ElementId = data["EleId"].ToString();
        XPosition = int.Parse(data["Xposition"].ToString());
        YPosition = int.Parse(data["Yposition"].ToString());
        Floor = int.Parse(data["Floor"].ToString());
        Type = (ElementType)int.Parse(data["ElementType"].ToString());
        Xrange = int.Parse(data["Xrange"].ToString());
        Yrange = int.Parse(data["Yrange"].ToString());
    }
}
