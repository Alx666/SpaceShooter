using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance { get; private set; }
    public float CurrentHealth { get; private set; }

    private static Object resourceBullet;
    private static Object resourceExplosion;

    public GameObject AfterBurner;
    public Slider HealthSlider;
    public ParticleSystem LightDamageFX;
    public ParticleSystem HeavyDamageFX;
    public GameObject LaserStartFX;
    public Light LaserLight;
    

    

    
    
    
    public float MainThruster = 5.0f;
    public float SideThruster = 5.0f;
    public float Health = 100f;
        
    private IWeapon currentWeapon;
    private float ab_lerpSpeed = 1.0f;
    private float m_fLerpTime = 1f;
    private float m_fCurrentLerpTime = 0f;
    private Vector3 afterBurnerScale;
    private Vector3 afterBurnerCurrentScale;
    private Rigidbody rigidBody;
    private LineRenderer m_hLineRenderer;
    private GameObject explosion;

    private const float CONEFIRE_ANGLE = 5f;
    private const float LASER_DAMAGE = 50f;

    public bool ShieldEnabled { get; set; }


    void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Multiple PlayerController Detected!!!");

        Instance = this;
        rigidBody = this.GetComponent<Rigidbody>();

        if(resourceBullet == null)
            resourceBullet = Resources.Load("Bullet");

        if (resourceExplosion == null)
            resourceExplosion = Resources.Load("Explosion_FX");

        afterBurnerScale = AfterBurner.transform.localScale;
        afterBurnerCurrentScale = new Vector3(afterBurnerScale.x, 0.0f, afterBurnerScale.z);
        AfterBurner.transform.localScale = afterBurnerCurrentScale;
        CurrentHealth = Health;
        HealthSlider.maxValue = Health;
        HealthSlider.value = CurrentHealth;

        m_hLineRenderer = this.GetComponent<LineRenderer>();

        EnableControl = false;
        this.transform.rotation = Quaternion.identity;
    }

	void Start () 
    {
        currentWeapon = new WeaponAntiMatter(this.transform.Find("Cannon").gameObject, 0.05f, true, 3.0f, CONEFIRE_ANGLE);
	}
	

	void Update () 
    {
        if (!EnableControl)
            return;

        AfterBurner.transform.localScale = afterBurnerCurrentScale;

        if (ShieldEnabled && currentWeapon.IsPressed)
        {
            currentWeapon.OnbuttonReleased();
        }
        else if (!ShieldEnabled)
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentWeapon.OnbuttonReleased();
            }

            if (Input.GetMouseButtonDown(0))
            {
                currentWeapon.OnbuttonPressed();
            }
        }

        currentWeapon.OnUpdate();
	}

    void FixedUpdate()
    {
        if (!EnableControl)
            return;

        PlayerInput(); 
    }

    void PlayerInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.Space))
                rigidBody.AddForce(this.gameObject.transform.forward * MainThruster * 2, ForceMode.VelocityChange);   
            else
            {
                rigidBody.AddForce(this.gameObject.transform.forward * MainThruster, ForceMode.VelocityChange);
                Thrust();
            }
        }
        else
            ThrustRelease();

        Plane hP = new Plane(Vector3.up, 0f);
        Ray vRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        float fDist;
        hP.Raycast(vRay, out fDist);
        Vector3 vMousePoint = vRay.GetPoint(fDist);
        Vector3 vRes = (vMousePoint - this.transform.position).normalized;

        this.transform.forward = vRes;

  
        if(Input.GetKey(KeyCode.Alpha1))
            currentWeapon = new WeaponAntiMatter(this.transform.Find("Cannon").gameObject, 0.05f, true, 3.0f, CONEFIRE_ANGLE);

        if (Input.GetKey(KeyCode.Alpha2))
            currentWeapon = new WeaponLaser(this.transform.Find("Cannon").gameObject, LaserStartFX, LaserLight, m_hLineRenderer, LASER_DAMAGE);

        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.AddForce(-this.transform.right * SideThruster, ForceMode.VelocityChange);
            //this.transform.rotation *= Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, 30f), UpdateLerp() * 2f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rigidBody.AddForce(this.transform.right * SideThruster, ForceMode.VelocityChange);
            //this.transform.rotation *= Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -30f), UpdateLerp() * 2f);
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            m_fCurrentLerpTime = 0f;
        }
        
    }

    private float UpdateLerp()
    {
        m_fCurrentLerpTime += Time.deltaTime;

        if (m_fCurrentLerpTime > m_fLerpTime) 
            m_fCurrentLerpTime = m_fLerpTime;

        return  m_fCurrentLerpTime / m_fLerpTime;
    }



    void Thrust()
    {
        afterBurnerCurrentScale.y += ab_lerpSpeed;
        if (afterBurnerCurrentScale.y > 18.0f)
            afterBurnerCurrentScale.y = 18.0f;
        
    }

    void ThrustRelease()
    {
        afterBurnerCurrentScale.y -= ab_lerpSpeed;
        if (afterBurnerCurrentScale.y < 1.5f)
            afterBurnerCurrentScale.y = 1.5f;
        
    }

    private int m_hDamageLevel;
    

    public void Damage(float fDmg)
    {
        CurrentHealth -= fDmg;

        if (CurrentHealth < Health / 2 && m_hDamageLevel == 0)
        {
            LightDamageFX.Play(true);
            m_hDamageLevel++;
        }

        if (CurrentHealth < Health / 3 && m_hDamageLevel == 1)
        {
            HeavyDamageFX.Play(true);
            m_hDamageLevel++;
        }

        if (CurrentHealth <= 0f)
        {
            Death();
        }

        HealthSlider.value = CurrentHealth;
    }

    public void RecHealth()
    {
    }

    public void Death()
    {
        explosion = GameObject.Instantiate(resourceExplosion) as GameObject;
        explosion.transform.position = this.transform.position;
        GameObject.Destroy(this.gameObject);
        //istanziare particella esplosione
        //disattivare playerController
        //triggerare gui end game
    }

    //public void ResetPosition()
    //{
    //    Vector3 currentPosition;
    //    Vector3 newPosition = new Vector3();
    //    currentPosition = this.gameObject.transform.position;
    //    if (currentPosition.z > 35.4f || currentPosition.z < -35.4f)
    //        newPosition = new Vector3(currentPosition.x, currentPosition.y, -currentPosition.z);
    //    else if (currentPosition.x > 62.5 || currentPosition.x < -62.5)
    //        newPosition = new Vector3(-currentPosition.x, currentPosition.y, currentPosition.z);
    //    this.gameObject.transform.position = newPosition;
    //}

    public bool EnableControl { get; set; }
}
