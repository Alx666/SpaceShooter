using UnityEngine;
using System.Collections;


[RequireComponent(typeof(LineRenderer))]
public class WeaponLaser : MonoBehaviour, IWeapon
{
    static Object resourceHitParticle;
    static Object resourceAudioSourceLaserfx;

    public float DPS                = 50f;
    public float MaxFireTemperature = 300f;
    public float CelsiusPerSecond   = 1f;


    public Color StartColorWeak     = Color.red;
    public Color StartColorStrong   = Color.cyan;
    public Color EndColorWeak       = Color.yellow;
    public Color EndColorStrong     = Color.blue;    
    public GameObject ShootLocator;
    public GameObject HitParticle;
    public GameObject ShootParticle;
    public GameObject AudioLaser;

    private AudioSource m_hLaserSFX;
    private ParticleSystem m_hHitParicleSystem;
    private ParticleSystem m_hStartParticleSystem;
    private LineRenderer m_hRenderer;
    private Light startLight;


    

    private float m_fDamage;
    private float perc;
    private float currentLerpTime;
    private float targetLerpTime;

    

    public WeaponLaser(GameObject sParticle, Light sLight, LineRenderer hRenderer, float fDmgSec)
    {
        
        ShootParticle = sParticle;
        m_hRenderer = hRenderer;
        m_fDamage = fDmgSec;
        startLight = sLight;
        resourceHitParticle = Resources.Load("Laser_ParticleHit");
        resourceAudioSourceLaserfx = Resources.Load("LaserSFX");
        m_hStartParticleSystem = ShootParticle.GetComponent<ParticleSystem>();
        targetLerpTime = 2f;        
    }

    public bool IsPressed { get; private set; }

    public void OnUpdate()
    {
        if (!m_hRenderer.enabled)
            return;

        RaycastHit vHit;
        if (Physics.Raycast(new Ray(ShootLocator.transform.position, ShootLocator.transform.forward), out vHit))
        {
            m_hRenderer.SetPosition(0, ShootLocator.transform.position);
            m_hRenderer.SetPosition(1, vHit.point);
            //m_hStartParticle.transform.position = m_hCannon.transform.position;

            IDamageable hDamageable = vHit.collider.gameObject.GetComponent<IDamageable>();

            if (hDamageable != null)
            {
                //lerptime for laser color lerp
                currentLerpTime += Time.deltaTime;
                if (currentLerpTime > targetLerpTime)
                    currentLerpTime = targetLerpTime;
                perc = currentLerpTime / targetLerpTime;
                //lerp smoothing
                perc = perc * perc * perc * (perc * (6f * perc - 15f) + 10f);
                //color lerp
                m_hRenderer.SetColors(Color.Lerp(StartColorWeak, StartColorStrong, perc), Color.Lerp(EndColorWeak, EndColorStrong, perc));


                hDamageable.Damage(m_fDamage * Time.deltaTime);
                HitParticle = GameObject.Instantiate(resourceHitParticle) as GameObject;
                m_hHitParicleSystem = HitParticle.GetComponent<ParticleSystem>();
                HitParticle.transform.position = vHit.point;
                //Vector3 dir = m_hCannon.transform.position - vHit.point;
                //dir.Normalize();
                //m_hHitParticle.transform.LookAt(m_hCannon.transform);
                m_hHitParicleSystem.Play(true);
                
            }

            //Istanziare l'effetto
        }
        else
        {
            m_hRenderer.SetPosition(0, ShootLocator.transform.position);
            m_hRenderer.SetPosition(1, ShootLocator.transform.position + ShootLocator.transform.forward * 500f);
            currentLerpTime = 0f;
            m_hRenderer.SetColors(Color.red, Color.yellow);
            
        }
    }

    public void OnbuttonReleased()
    {
        m_hRenderer.enabled = false;
        startLight.enabled = false;
        if (HitParticle)
            m_hHitParicleSystem.Stop(true);
        GameObject.Destroy(HitParticle);
        if (m_hStartParticleSystem.isPlaying)
            m_hStartParticleSystem.enableEmission = false;
        GameObject.DestroyImmediate(AudioLaser);

        IsPressed = false;
    }

    public void OnbuttonPressed()
    {
        m_hRenderer.enabled = true;
        startLight.enabled = true;
        m_hStartParticleSystem.enableEmission = true;
        m_hStartParticleSystem.Play(true);
        AudioLaser = GameObject.Instantiate(resourceAudioSourceLaserfx) as GameObject;
        m_hLaserSFX = AudioLaser.GetComponent<AudioSource>();
        
        

        IsPressed = true;
    }
}
