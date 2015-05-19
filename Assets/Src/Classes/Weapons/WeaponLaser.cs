using UnityEngine;
using System.Collections;

public class WeaponLaser : IWeapon
{
    static Object resourceHitParticle;
    static Object resourceAudioSourceLaserfx;

    private GameObject m_hCannon;
    private GameObject m_hHitParticle;
    private GameObject m_hStartParticle;
    private GameObject m_hAudioLaser;
    private AudioSource m_hLaserSFX;
    private ParticleSystem m_hHitParicleSystem;
    private ParticleSystem m_hStartParticleSystem;
    private LineRenderer m_hRenderer;
    private Light startLight;
    private Color startColorWeak = Color.red;
    private Color startColorStrong = Color.cyan;
    private Color endColorWeak = Color.yellow;
    private Color endColorStrong = Color.blue;
    private Color startColor;
    private Color endColor;

    private float m_fDamage;
    private float perc;
    private float currentLerpTime;
    private float targetLerpTime;

    

    public WeaponLaser(GameObject hCannon, GameObject sParticle, Light sLight, LineRenderer hRenderer, float fDmgSec)
    {
        m_hCannon = hCannon;
        m_hStartParticle = sParticle;
        m_hRenderer = hRenderer;
        m_fDamage = fDmgSec;
        startLight = sLight;
        resourceHitParticle = Resources.Load("Laser_ParticleHit");
        resourceAudioSourceLaserfx = Resources.Load("LaserSFX");
        m_hStartParticleSystem = m_hStartParticle.GetComponent<ParticleSystem>();
        targetLerpTime = 2f;

        
        //m_hStartParticle = GameObject.Instantiate(resourceStartParticle) as GameObject;
        //m_hStartParticle.transform.position = m_hCannon.transform.position;
        //m_hStartParticleSystem = m_hStartParticle.GetComponent<ParticleSystem>();

        
        //istanziare startParticle!!!

    }

    public bool IsPressed { get; private set; }

    public void OnUpdate()
    {
        if (!m_hRenderer.enabled)
            return;

        RaycastHit vHit;
        if (Physics.Raycast(new Ray(m_hCannon.transform.position, m_hCannon.transform.forward), out vHit))
        {
            m_hRenderer.SetPosition(0, m_hCannon.transform.position);
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
                m_hRenderer.SetColors(Color.Lerp(startColorWeak, startColorStrong, perc), Color.Lerp(endColorWeak, endColorStrong, perc));


                hDamageable.Damage(m_fDamage * Time.deltaTime);
                m_hHitParticle = GameObject.Instantiate(resourceHitParticle) as GameObject;
                m_hHitParicleSystem = m_hHitParticle.GetComponent<ParticleSystem>();
                m_hHitParticle.transform.position = vHit.point;
                //Vector3 dir = m_hCannon.transform.position - vHit.point;
                //dir.Normalize();
                //m_hHitParticle.transform.LookAt(m_hCannon.transform);
                m_hHitParicleSystem.Play(true);
                
            }




            //Istanziare l'effetto
        }
        else
        {
            m_hRenderer.SetPosition(0, m_hCannon.transform.position);
            m_hRenderer.SetPosition(1, m_hCannon.transform.position + m_hCannon.transform.forward * 500f);
            currentLerpTime = 0f;
            m_hRenderer.SetColors(Color.red, Color.yellow);
            
        }
    }

    public void OnbuttonReleased()
    {
        m_hRenderer.enabled = false;
        startLight.enabled = false;
        if (m_hHitParticle)
            m_hHitParicleSystem.Stop(true);
        GameObject.Destroy(m_hHitParticle);
        if (m_hStartParticleSystem.isPlaying)
            m_hStartParticleSystem.enableEmission = false;
        GameObject.DestroyImmediate(m_hAudioLaser);

        IsPressed = false;
    }

    public void OnbuttonPressed()
    {
        m_hRenderer.enabled = true;
        startLight.enabled = true;
        m_hStartParticleSystem.enableEmission = true;
        m_hStartParticleSystem.Play(true);
        m_hAudioLaser = GameObject.Instantiate(resourceAudioSourceLaserfx) as GameObject;
        m_hLaserSFX = m_hAudioLaser.GetComponent<AudioSource>();
        
        

        IsPressed = true;
    }
}
