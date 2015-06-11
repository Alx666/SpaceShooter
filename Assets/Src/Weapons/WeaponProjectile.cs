using UnityEngine;
using System.Collections;


public class WeaponProjectile : MonoBehaviour, IWeapon
{
    public GameObject       BulletPrefab;

    public GameObject       ShootLocator;

    [Range(0f, 90f)]
    public float            Spread         = 0f;

    [Range(0.01f, 3f)]
    public float            BulletDelay    = 0.3f; 

    [Range(-10f, 10f)]
    public float            ForceMult      = 1f;
   
    [Range(0.01f, 3f)]
    public float            BarrageDelay   = 0.3f;
    
    public int              Barrage        = 1;

    private Pool            m_hPool;
    private IWeaponState    m_hStateMachine;
    private WeaponReady     m_hTrigger;
        
    void Awake()
    {
        m_hPool = GlobalFactory.GetPool(BulletPrefab);

        //Automatic State Machine Composition
        //One Ready State
        m_hTrigger = new WeaponReady();

        //N Wait And Shoot for Barrage Implemenation
        IWeaponState hLast = m_hTrigger;      
        for (int i = 0; i < Barrage; i++)
        {
            WeaponShoot hShoot  = new WeaponShoot(this);            
            WeaponWait  hWait   = new WeaponWait(BulletDelay);

            hLast.Next  = hShoot;
            hShoot.Next = hWait;
            hLast       = hWait;
        }

        (hLast as WeaponWait).Delay = BarrageDelay;

        hLast.Next      = m_hTrigger;
        m_hStateMachine = hLast;
    }

	void Start () 
    {
	
	}
		
	void Update () 
    {
        m_hStateMachine = m_hStateMachine.Update();
	}

    public void OnbuttonReleased()
    {
        m_hTrigger.OnButtonReleased();                
    }

    public void OnbuttonPressed()
    {
        m_hTrigger.OnButtonPressed();
    }


    public void OnUpdate()
    {

    }

    public bool IsPressed
    {
        get { return false; }
    }

    #region Internal State Machine

    private interface IWeaponState
    {
        IWeaponState Update();
        IWeaponState Next { get; set; }
    }

    private class WeaponWait : IWeaponState
    {
        public float Delay;
        private float m_fElapsedTime;

        public IWeaponState Next { get; set; }

        public WeaponWait(float fDelay)
        {
            Delay = fDelay;
            m_fElapsedTime = Delay;
        }
        public IWeaponState Update()
        {
            if (m_fElapsedTime <= 0f)
            {
                m_fElapsedTime = Delay;
                return Next;
            }
            else
            {
                m_fElapsedTime -= Time.deltaTime;
                return this;
            }
        }
    }

    private class WeaponShoot : IWeaponState
    {
        private WeaponProjectile m_hOwner;
        public IWeaponState Next { get; set; }


        public WeaponShoot(WeaponProjectile hWeap)
        {
            m_hOwner = hWeap;
        }


        public IWeaponState Update()
        {
            Vector3 vPosition = m_hOwner.ShootLocator.transform.position;
            vPosition.y = 0f;

            Vector3 vDirection;

            if (m_hOwner.Spread > 0f)
            {
                float fRange = UnityEngine.Random.Range(-m_hOwner.Spread, m_hOwner.Spread);
                vDirection = Quaternion.Euler(0f, fRange, 0f) * m_hOwner.ShootLocator.transform.forward;
                vDirection.y = 0f;
                vDirection.Normalize();
            }
            else
            {
                vDirection = m_hOwner.ShootLocator.transform.forward;
            }


            Bullet hBullet = GlobalFactory.GetInstance<Bullet>(m_hOwner.BulletPrefab);
            hBullet.gameObject.transform.position = vPosition;
            hBullet.gameObject.transform.forward = vDirection;
            hBullet.RigidBody.AddForce(hBullet.gameObject.transform.forward * hBullet.Speed, ForceMode.VelocityChange);

            return Next;
        }
    }

    private class WeaponReady : IWeaponState
    {
        public bool m_bFire;

        public IWeaponState Next { get; set; }


        public IWeaponState Update()
        {
            if (m_bFire)
            {
                
                return Next;
            }
            else
            {
                return this;
            }
        }

        public void OnButtonPressed()
        {
            m_bFire = true;
        }

        public void OnButtonReleased()
        {
            m_bFire = false;
        }
    }

    #endregion
}
