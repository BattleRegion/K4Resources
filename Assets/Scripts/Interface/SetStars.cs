using UnityEngine;
using System.Collections;

public class SetStars : MonoBehaviour
{
    public UIGrid stars;
    public GameObject star;

    public void AddStar(int index)
    {
        GameObject g = NGUITools.AddChild(stars.gameObject, star);
        g.GetComponent<UIWidget>().depth = 18 + index;
    }

    void ClearStar()
    {
        foreach (Transform t in stars.transform)
        {
            Destroy(t.gameObject);
        }
    }

    public void SetStar(int Num)
    {
        ClearStar();
        while (Num > 0)
        {
            AddStar(Num);
            Num--;
        }
    }

    void Update()
    {
        stars.Reposition();
    }
}