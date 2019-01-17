using UnityEngine;
using System.Collections;

public class Houses : MonoBehaviour {
    public GameObject DungeonObject;

    public DungeonDetail DungeonDetail;

	// Use this for initialization
	void Start () {
        UIEventListener.Get(DungeonObject).onClick = (go) =>
        {
            DungeonDetail.CurDetailType = DungeonDetail.type.Chapter;
            DungeonDetail.gameObject.SetActive(true);
        };
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}
}
