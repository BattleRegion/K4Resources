using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class PetAvatar : MonoBehaviour {

    public Sprite[] FrameSprites;

    public SpriteRenderer AvatarSprite;

    public SpriteRenderer FrameSprite;

    public int UserPetId;

    public MonsterData.Properties CurPro;

    public List<GameObject> PetFlyParResources = new List<GameObject>();
    public List<GameObject> PetFlyBumpParResources = new List<GameObject>();
    public List<GameObject> ShineParResources = new List<GameObject>();
    public bool hasReady = false;
    public GameObject ShineFrame;
    public Vector3 AppearPosition;
	// Use this for initialization
	void Start () 
    {
       
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetAvatar(MonsterData.Properties pro, string headId)
    {
        CurPro = pro;
        FrameSprite.sprite = FrameSprites[GetFrameIndex(pro)];
        string path = "Atlas/PetAvatars/" + headId;
        Sprite avatarS = Resources.Load<Sprite>(path);
        AvatarSprite.sprite = avatarS;
    }

    int GetFrameIndex(MonsterData.Properties pro)
    {
        if (pro == MonsterData.Properties.Earth)
        {
            return 0;
        }
        else if (pro == MonsterData.Properties.Fire)
        {
            return 1;
        }
        else if (pro == MonsterData.Properties.Wind)
        {
            return 2;
        }
        else if (pro == MonsterData.Properties.Water)
        {
            return 3;
        }
        return 0;
    }

    public void AvatarShow(bool show)
    {
        if (hasReady != show)
        {
            hasReady = show;
            Vector3 v;
            if (show)
            {
                v = new Vector3(transform.localPosition.x, transform.localPosition.y + 15, transform.localPosition.z);
            }
            else
            {
                v = new Vector3(transform.localPosition.x, -508, transform.localPosition.z);
            }
            Hashtable args = new Hashtable();
            args.Add("time", 0.3f);
            args.Add("islocal", true);
            args.Add("position", v);
            iTween.MoveTo(gameObject, args);
        }
    }
    Action<PetAvatar> curAvatarFlyCallback;
    public void PetTryFlyIntoScene(Vector3 to,Action<PetAvatar> callback)
    {
        curAvatarFlyCallback = callback;
        ParAnimation(to);
        AvatarSprite.gameObject.SetActive(false);
    }

    Vector3 backTo;
    Action<PetAvatar> curAvatarFlyBackCallback;
    public void PetFlyBack(Vector3 from, Action<PetAvatar> callback)
    {
        curAvatarFlyBackCallback = callback;
        Vector3 to = transform.position;
        backTo = to;
        flyPar = Instantiate(GetFlyPar()) as GameObject;
        flyPar.transform.position = from;
        float rotateAngle = 0;
        float x = Mathf.Abs(to.x - flyPar.transform.position.x);
        float y = Mathf.Abs(to.y - flyPar.transform.position.y);
        rotateAngle = 90 - Mathf.Atan(y / x) * 180 / Mathf.PI;
        if (to.x < flyPar.transform.position.x)
        {
            rotateAngle = rotateAngle * -1;
        }
        flyPar.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);

        Hashtable args = new Hashtable();
        args.Add("time", 0.1f);
        args.Add("scale", new Vector3(1.2f, 1.2f, 1.2f));
        args.Add("easytype", iTween.EaseType.easeOutExpo);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "ParBackScaleEnd");
        iTween.ScaleTo(flyPar, args);
    }

    void ParBackScaleEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.2f);
        args.Add("scale", new Vector3(0.2f, 1.4f, 1));
        args.Add("easytype", iTween.EaseType.easeInExpo);
        iTween.ScaleTo(flyPar, args);
        Invoke("ParBackFly", 0.1f);
    }

    void ParBackFly()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.2f);
        args.Add("position", new Vector3(backTo.x, backTo.y, 1));
        args.Add("easytype", iTween.EaseType.linear);
        iTween.MoveTo(flyPar, args);
        Invoke("BackParFlyEnd", 0.16f);
    }

    void BackParFlyEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.05f);
        args.Add("scale", new Vector3(0.5f, 0.5f, 0.5f));
        args.Add("easytype", iTween.EaseType.easeOutExpo);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "BackParFlyEnd1");
        iTween.ScaleTo(flyPar, args);
        FrameAnimation();
    }


    void BackParFlyEnd1()
    {
        Destroy(flyPar);
        GameObject parBump = Instantiate(GetBumpPar()) as GameObject;
        parBump.transform.position = flyPar.transform.position;
        AvatarSprite.gameObject.SetActive(true);
        curAvatarFlyBackCallback(this);
    }

    GameObject flyPar;
    Vector3 curTo;
    void ParAnimation(Vector3 to)
    {
        curTo = to;
        flyPar = Instantiate(GetFlyPar()) as GameObject;
        flyPar.transform.position = transform.position;
        float rotateAngle = 0;
        float x = Mathf.Abs(to.x - flyPar.transform.position.x);
        float y = Mathf.Abs(to.y - flyPar.transform.position.y);
        rotateAngle = 90 - Mathf.Atan(y / x) * 180 / Mathf.PI;
        if (to.x > flyPar.transform.position.x)
        {
            rotateAngle = rotateAngle * -1;
        }
        flyPar.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);

        Hashtable args = new Hashtable();
        args.Add("time", 0.1f);
        args.Add("scale", new Vector3(1.2f, 1.2f,1.2f));
        args.Add("easytype", iTween.EaseType.easeOutExpo);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "ParScaleEnd");
        iTween.ScaleTo(flyPar, args);
    }

    void ParScaleEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.2f);
        args.Add("scale", new Vector3(0.2f, 1.4f,1));
        args.Add("easytype", iTween.EaseType.easeInExpo);
        iTween.ScaleTo(flyPar, args);
        Invoke("ParFly", 0.1f);
    }

    void ParFly()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.2f);
        args.Add("position", new Vector3(curTo.x, curTo.y,1));
        args.Add("easytype", iTween.EaseType.linear);
        iTween.MoveTo(flyPar, args);
        FrameAnimation();

        Invoke("ParFlyEnd", 0.16f);
    }

    void ParFlyEnd()
    {
        Hashtable args = new Hashtable();
        args.Add("time", 0.05f);
        args.Add("scale", new Vector3(0.5f, 0.5f, 0.5f));
        args.Add("easytype", iTween.EaseType.easeOutExpo);
        args.Add("oncompletetarget", gameObject);
        args.Add("oncomplete", "ParFlyEnd1");
        iTween.ScaleTo(flyPar, args);

    }

    void ParFlyEnd1()
    {
        Destroy(flyPar);
        GameObject parBump = Instantiate(GetBumpPar()) as GameObject;
        parBump.transform.position = flyPar.transform.position;
        curAvatarFlyCallback(this);
    }

    void FrameAnimation()
    {
        Vector3 v = new Vector3(transform.localPosition.x, -530, transform.localPosition.z);
        Hashtable args = new Hashtable();
        args.Add("time", 0.1f);
        args.Add("islocal", true);
        args.Add("position", v);
        iTween.MoveTo(gameObject, args);

        Hashtable args1 = new Hashtable();
        args1.Add("time", 0.1f);
        args1.Add("scale", new Vector3(1.4f,1.4f,1.4f));
        args1.Add("easytype", iTween.EaseType.easeOutExpo);
        args1.Add("oncompletetarget", gameObject);
        args1.Add("oncomplete", "ScaleEnd");
        iTween.ScaleTo(gameObject, args1);
    }

    void ScaleEnd()
    {
        Vector3 v = new Vector3(transform.localPosition.x, -508, transform.localPosition.z);
        Hashtable args = new Hashtable();
        args.Add("time", 0.3f);
        args.Add("islocal", true);
        args.Add("position", v);
        iTween.MoveTo(gameObject, args);

        Hashtable args1 = new Hashtable();
        args1.Add("time", 0.3f);
        args1.Add("scale", new Vector3(1, 1, 1));
        args1.Add("easytype", iTween.EaseType.easeInExpo);
        iTween.MoveTo(gameObject, args);
        iTween.ScaleTo(gameObject, args1);
    }

    GameObject GetFlyPar()
    {
        if (CurPro == MonsterData.Properties.Earth)
        {
            return PetFlyParResources[1];
        }
        else if (CurPro == MonsterData.Properties.Fire)
        {
            return PetFlyParResources[2];
        }
        else if (CurPro == MonsterData.Properties.Wind)
        {
            return PetFlyParResources[3];
        }
        else if (CurPro == MonsterData.Properties.Water)
        {
            return PetFlyParResources[0];
        }
        return null;
    }

    GameObject GetShinePar()
    {
        if (CurPro == MonsterData.Properties.Earth)
        {
            return ShineParResources[1];
        }
        else if (CurPro == MonsterData.Properties.Fire)
        {
            return ShineParResources[2];
        }
        else if (CurPro == MonsterData.Properties.Wind)
        {
            return ShineParResources[3];
        }
        else if (CurPro == MonsterData.Properties.Water)
        {
            return ShineParResources[0];
        }
        return null;
    }

    GameObject GetBumpPar()
    {
        if (CurPro == MonsterData.Properties.Earth)
        {
            return PetFlyBumpParResources[1];
        }
        else if (CurPro == MonsterData.Properties.Fire)
        {
            return PetFlyBumpParResources[2];
        }
        else if (CurPro == MonsterData.Properties.Wind)
        {
            return PetFlyBumpParResources[3];
        }
        else if (CurPro == MonsterData.Properties.Water)
        {
            return PetFlyBumpParResources[0];
        }
        return null;
    }

    GameObject shine;
    public void ActionShine(bool action)
    {
        if (action)
        {
            shine = Instantiate(GetShinePar()) as GameObject;
            shine.transform.parent = transform;
            shine.transform.localPosition = new Vector3(-28, 28, 0);
            ShineFrame.SetActive(true);
        }
        else
        {
            Destroy(shine);
            ShineFrame.SetActive(false);
        }
    }
}

