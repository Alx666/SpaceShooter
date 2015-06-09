using UnityEngine;
using System.Collections;



[RequireComponent(typeof(LineRenderer))]
public class WeaponLaser : MonoBehaviour, IWeapon
{
    public float DPS                            = 50f;
    public float MaxFireTemperature             = 300f;
    public float CelsiusPerSecond               = 1f;
    public float TargetLerpTime                 = 2f;

    public Color StartColorWeak                 = Color.red;
    public Color StartColorStrong               = Color.cyan;
    public Color EndColorWeak                   = Color.yellow;
    public Color EndColorStrong                 = Color.blue;    

    public GameObject ShootLocator;
    public GameObject AudioLaser;

    public GameObject     StartEffect;
    public GameObject     HitEffect;
    
            
    private LineRenderer m_hRenderer;
    private float m_fCurrentLerpTime;
    private bool  m_bFire;


    void Awake()
    {
        m_hRenderer = this.GetComponent<LineRenderer>();
    }


    public WeaponLaser()
    {                               
        TargetLerpTime = 2f;
    }

    
    public void Update()
    {
        if (!m_bFire)
            return;


        if (!m_hRenderer.enabled)
            m_hRenderer.enabled = true;


        RaycastHit  vHit;
        Ray         vRay = new Ray(ShootLocator.transform.position, ShootLocator.transform.forward);

        if (Physics.Raycast(vRay, out vHit))
        {
            m_hRenderer.SetPosition(0, ShootLocator.transform.position);
            m_hRenderer.SetPosition(1, vHit.point);

            HitEffect.transform.position = vHit.point;
            
            
            IDamageable hDamageable = vHit.collider.gameObject.GetComponent<IDamageable>();

            if (hDamageable != null)
            {
                
                //lerptime for laser color lerp
                m_fCurrentLerpTime += Time.deltaTime;

                if (m_fCurrentLerpTime > TargetLerpTime)
                    m_fCurrentLerpTime = TargetLerpTime;

                float fPerc = m_fCurrentLerpTime / TargetLerpTime;

                //lerp smoothing
                //perc = perc * perc * perc * (perc * (6f * perc - 15f) + 10f);
                //color lerp
                m_hRenderer.SetColors(Color.Lerp(StartColorWeak, StartColorStrong, fPerc), Color.Lerp(EndColorWeak, EndColorStrong, fPerc));


                hDamageable.Damage(DPS * Time.deltaTime);                                                
            }

            
        }
        else
        {
            Vector3 vAway = ShootLocator.transform.position + ShootLocator.transform.forward * 500f;
            HitEffect.transform.position = vAway;
            m_hRenderer.SetPosition(0, ShootLocator.transform.position);
            m_hRenderer.SetPosition(1, vAway);
            m_hRenderer.SetColors(Color.red, Color.yellow);


            m_fCurrentLerpTime = 0f;
        }
    }



    public void OnbuttonPressed()
    {
        m_bFire = true;
        StartEffect.SetActive(true);
        HitEffect.SetActive(true);
    }


    public void OnbuttonReleased()
    {
        m_bFire = false;
        m_hRenderer.enabled = false;
        StartEffect.SetActive(false);
        HitEffect.SetActive(false);
        m_fCurrentLerpTime = 0f;
    }

}
