using UnityEngine;
using System.Collections;

public class WeaponAntiMatter : IWeapon
{
    

    GameObject cannon;
    GameObject bullet;
    
    Rigidbody bulletRigidBody;

    static Object resourceBullet;

    private float m_fCoolDownTime;
    private float m_fCoolDown;
    private float m_fSpeedCoeff;
    private bool  m_bFire;
    private bool  m_bConeFire;
    private float m_bConeFireAngle;

    public bool IsPressed { get; private set; }

    static WeaponAntiMatter()
    {
        resourceBullet = Resources.Load("Bullet");
        
    }

    public WeaponAntiMatter(GameObject hcannon, float fCoolDown, bool bConefire, float fSpeedModif, float fConefireAngle)
    {
        cannon = hcannon;
        
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

            bullet = GameObject.Instantiate(resourceBullet) as GameObject;
            BulletController hCtrl = bullet.GetComponent<BulletController>();
            hCtrl.Speed *= m_fSpeedCoeff;
            bullet.transform.position = cannon.transform.position;
            Vector3 vPosition =  bullet.transform.position;
            vPosition.y = 0;
            bullet.transform.position = vPosition;

            if (m_bConeFire)
            {
                float fRange = UnityEngine.Random.Range(-m_bConeFireAngle, m_bConeFireAngle);

                Vector3 vBulletForward = Quaternion.Euler(0f, fRange, 0f) * cannon.transform.forward;
                vBulletForward.y = 0f;
                vBulletForward.Normalize();
                bullet.transform.forward = vBulletForward;
                
            }
            else
            {
                bullet.transform.forward = cannon.transform.forward;
            }

            
            
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

            


