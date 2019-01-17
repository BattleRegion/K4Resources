using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoticeViewControl : MonoBehaviour
{
    public List<GameObject> Views = new List<GameObject>();


    public void SetViews()
    {
        foreach (GameObject g in Views)
        {
            if (g.name == "Sprite_Main")
            {
                if (g.activeSelf == false)
                {
                    g.SetActive(true);
                }
            }
            else if (g.activeSelf == true)
            {
                g.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        SetViews();
    }

}
