using UnityEngine;
using System.Collections;

public class GoldFlyAc : MonoBehaviour {
    
    ParticleSystem _ps;
    bool Sta=false;
    int GoldNum=6;
    Vector3 SpeedFly;
    float CurTime = 0f;
    public PveGameControl gamecontrol;

    public Vector3 EndVe ;    
    public float Ftime;

   
    void Awake()
    {
        _ps = this.GetComponent<ParticleSystem>();
        //_ps.maxParticles = 20;
    } 
    void Start()
    {
        Invoke("FlyAction", 1.0f);        
    }
    void FlyAction()
    {
        Sta = true;
	}

    public void SetVeAndNum(int n)
    {
        EndVe = PveGameControl.CurShakObj_gold.transform.position;
        GoldNum =(int) n/40;
        //Debug.Log(v + "   n=" + n + "   GoldNum=" + GoldNum);
        if(_ps)_ps.maxParticles = GoldNum;
    }
    bool Shake = false;
    void Update()
    {
        if (Sta == false) return;

        int maxCount = _ps.maxParticles;
        ParticleSystem.Particle[] arrParticles = new ParticleSystem.Particle[maxCount];
        int activeCount = _ps.GetParticles(arrParticles);

        CurTime = CurTime + Time.deltaTime;
        float per=CurTime/Ftime;
        per = per * 0.8f;

       // Debug.Log(activeCount + "   maxCount= " + maxCount);

        for (int n = 0; n < activeCount; ++n)
        {           
            SpeedFly = (EndVe - arrParticles[n].position) *per;
            arrParticles[n].position += SpeedFly;
            //Debug.Log("df " + n + "   " + arrParticles[n].position);
            if (arrParticles[n].position.y > EndVe.y)
            {
                arrParticles[n].position=EndVe;
            }
        }

        particleSystem.SetParticles(arrParticles, activeCount);

        if (CurTime > Ftime * 0.3f)
        {
            if (Shake == false)
            {
              Shake = true;
				if(gamecontrol!=null)gamecontrol.PvePlayerInfo.setGoldLabelNum();
              iTween.ShakeScale(PveGameControl.CurShakObj_gold, new Vector3(1.5f, 1.5f, 1.5f), 0.4f);
            }                    
        }

        if (CurTime > Ftime * 0.8f)
        {          
            Destroy(gameObject);
        }
    }
}
