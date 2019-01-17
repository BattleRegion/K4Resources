using UnityEngine;
using System.Collections;
using SimpleJson;
public class UserItem  
{
    public int UserItemId;

    public ItemData CurItemData;

    public UserItem(JsonObject data)
    {
        if (data.ContainsKey("house_id")) UserItemId = int.Parse(data["house_id"].ToString());
        string itemId = data["id"].ToString();
        CurItemData = ConfigManager.ItemConfig.GetItemById(itemId);
    }
}
