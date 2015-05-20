using UnityEngine;
using System.Collections;

public class WeaponAntiMatter : IWeapon
{        
    private GameObject  m_hCannon;        
    private float       m_fCoolDownTime;
    private float       m_fCoolDown;
    private float       m_fSpeedCoeff;
    private bool        m_bFire;
    private bool        m_bConeFire;
    private float       m_bConeFireAngle;

    public bool IsPressed { get; private set; }

    public WeaponAntiMatter(GameObject hcannon, float fCoolDown, bool bConefire, float fSpeedModif, float fConefireAngle)
    {
        m_hCannon = hcannon;
        
        m_fCoolDownTime = fCoolDown;
        m_fCoolDown = m_fCoolDownTime;
        m_fSpeedCoeff = fSpeedModif;
        m_bConeFire = bConefire;
        m_bConeFireAngle = fConefireAngle;
    }

    public void OnUpdate()
    {
        if (m_bFire && m_fCoolDown == 0f)
        {
            m_fCoolDown = m_fCoolDownTime;

            Bullet hBullet = Bullet.Pool.Get();//(GameObject.Instantiate(Resources.Load("Bullet")) as GameObject).GetComponent<Bullet>();
            
            hBullet.transform.position  = m_hCannon.transform.position;
            Vector3 vPosition           = hBullet.transform.position;
            vPosition.y                 = 0;
            hBullet.transform.position  = vPosition;

            if (m_bConeFire)
            {
                float fRange = UnityEngine.Random.Range(-m_bConeFireAngle, m_bConeFireAngle);

                Vector3 vBulletForward = Quaternion.Euler(0f, fRange, 0f) * m_hCannon.transform.forward;
                vBulletForward.y = 0f;
                vBulletForward.Normalize();
                hBullet.transform.forward = vBulletForward;                
            }
            else
            {
                hBullet.transform.forward = m_hCannon.transform.forward;
            }

            hBullet.RigidBody.AddForce(hBullet.gameObject.transform.forward * (hBullet.Speed * m_fSpeedCoeff), ForceMode.VelocityChange);
        }

        if (m_fCoolDown > 0f)
        {
            m_fCoolDown -= Time.deltaTime;
        }
        else
        {
            m_fCoolDown = 0.0f;
        }
    }

    public void OnbuttonReleased()
    {
        m_bFire     = false;
        IsPressed   = m_bFire;
    }

    public void OnbuttonPressed()
    {
        m_bFire     = true;
        IsPressed   = m_bFire;
    }
}

            


