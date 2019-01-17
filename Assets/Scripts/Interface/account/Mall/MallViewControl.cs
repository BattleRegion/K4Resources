using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MallViewControl : MonoBehaviour
{
    public List<GameObject> MallViews = new List<GameObject>();


    public void SetBSViews()
    {
        foreach (GameObject g in MallViews)
        {
            if (g.name == "Sprite_Main" || g.name == "Background")
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

    public void ToDiamondConsumeView()
    {
        foreach(GameObject g in MallViews)
        {
            if(g.activeSelf)
            {
                g.SetActive(false);
            }
        }
        MallViews[5].SetActive(true);
    }

    void OnEnable()
    {
        SetBSViews();
    }
}
