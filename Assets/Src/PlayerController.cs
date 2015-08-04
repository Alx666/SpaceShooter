﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IAIActor
{
    public static PlayerController Instance { get; private set; }

    #region IAIActor

    public Rigidbody Rigidbody      { get; private set; }
    public Collider Collider        { get; private set; } 
    public IAIActor Target          { get; set; }
    public Transform Transform      { get { return this.transform; } }
    public float EngineForce        { get { return MainThruster; } }
    public float TurnForce          { get { return SideThruster; } }
    public List<IWeapon> Weapons    { get; private set; }

    #endregion

    public float MainThruster = 5.0f;
    public float SideThruster = 5.0f;



    public bool     ShieldEnabled   { get; private set; }

    public GameObject ExplosionPrefab;


    public float Hp = 100f;


    public Shield               Shield              { get; set; }

    public WeaponProjectile     WeaponAntiMatter;

    public WeaponLaser          WeaponLaser;

    public WeaponProjectile     WeaponFlak;

    public WeaponProjectile     WeaponRail;

    private IWeapon             m_hCurrentWeapon;



    #region Refactoring 

    private float ab_lerpSpeed = 1.0f;
    private float m_fLerpTime = 1f;
    private float m_fCurrentLerpTime = 0f;
    private Vector3 afterBurnerScale;
    private Vector3 afterBurnerCurrentScale;    
    private LineRenderer m_hLineRenderer;
    


    public GameObject AfterBurner;
    public Slider HealthSlider;
    public ParticleSystem LightDamageFX;
    public ParticleSystem HeavyDamageFX;

    #endregion


    void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Multiple PlayerController Detected!!!");

        Instance         = this;
        this.Rigidbody     = this.GetComponent<Rigidbody>();       
        Shield           = this.GetComponentInChildren<Shield>();
        Weapons          = this.GetComponentsInChildren<IWeapon>().ToList();

        m_hCurrentWeapon = WeaponAntiMatter;


        afterBurnerScale = AfterBurner.transform.localScale;
        afterBurnerCurrentScale = new Vector3(afterBurnerScale.x, 0.0f, afterBurnerScale.z);
        AfterBurner.transform.localScale = afterBurnerCurrentScale;
        Health = this.Hp;
        HealthSlider.maxValue   = Health;
        HealthSlider.value      = Health;

        m_hLineRenderer = this.GetComponent<LineRenderer>();

        EnableControl = false;
        this.transform.rotation = Quaternion.identity;

        
    }

	void Start () 
    {        
	}
	

	void Update () 
    {
        if (!EnableControl)
            return;

        AfterBurner.transform.localScale = afterBurnerCurrentScale;

        if (Input.GetMouseButtonDown(0))
        {
            m_hCurrentWeapon.OnbuttonPressed();
        }  

        if (Input.GetMouseButtonUp(0))
        {
            m_hCurrentWeapon.OnbuttonReleased();
        }




        //if (ShieldEnabled && m_hCurrentWeapon.IsPressed)
        //{
        //    m_hCurrentWeapon.OnbuttonReleased();
        //}
        //else if (!ShieldEnabled)
        //{
        //    if (Input.GetMouseButtonUp(0))
        //    {
        //        WeaponAntiMatter.OnbuttonReleased();
        //    }

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        WeaponAntiMatter.OnbuttonPressed();
        //    }
        //}
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
                Rigidbody.AddForce(this.gameObject.transform.forward * MainThruster * 2, ForceMode.VelocityChange);   
            else
            {
                Rigidbody.AddForce(this.gameObject.transform.forward * MainThruster, ForceMode.VelocityChange);
                Thrust();
            }
        }
        else
            ThrustRelease();


        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.Space))
                Rigidbody.AddForce(-this.gameObject.transform.forward * MainThruster * 2, ForceMode.VelocityChange);
            else
            {
                Rigidbody.AddForce(-this.gameObject.transform.forward * MainThruster, ForceMode.VelocityChange);
            }
        }




        Plane hP = new Plane(Vector3.up, 0f);
        Ray vRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        float fDist;
        hP.Raycast(vRay, out fDist);
        Vector3 vMousePoint = vRay.GetPoint(fDist);
        Vector3 vRes = (vMousePoint - this.transform.position).normalized;

        this.transform.forward = vRes;


        if (Input.GetKey(KeyCode.Alpha1))
            m_hCurrentWeapon = WeaponAntiMatter;

        if (Input.GetKey(KeyCode.Alpha2))
            m_hCurrentWeapon = WeaponLaser;

        if (Input.GetKey(KeyCode.Alpha3))
            m_hCurrentWeapon = WeaponFlak;

        if (Input.GetKey(KeyCode.Alpha4))
            m_hCurrentWeapon = WeaponRail;

        if (Input.GetKey(KeyCode.A))
        {
            Rigidbody.AddForce(-this.transform.right * SideThruster, ForceMode.VelocityChange);           
        }

        if (Input.GetKey(KeyCode.D))
        {
            Rigidbody.AddForce(this.transform.right * SideThruster, ForceMode.VelocityChange);
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

    public float Health { get; private set; }

    public void Damage(float fDmg)
    {
        Health -= fDmg;

        if (Health <= this.Hp / 2 && m_hDamageLevel == 0)
        {
            LightDamageFX.Play(true);
            m_hDamageLevel++;
        }

        if (Health < this.Hp / 3 && m_hDamageLevel == 1)
        {
            HeavyDamageFX.Play(true);
            m_hDamageLevel++;
        }

        if (Health <= 0f)
        {
            Destroy();
        }

        HealthSlider.value = Health;
    }

    public void RecHealth()
    {
    }

    public void Destroy()
    {
        GameObject hExplosion = GlobalFactory.GetInstance(ExplosionPrefab);
        hExplosion.transform.position = this.transform.position;
        GameObject.Destroy(this.gameObject);
        //TODO: Game Over
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

 

    public Pool Pool
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }

    public void Enable()
    {
        throw new System.NotImplementedException();
    }

    public void Disable()
    {
        throw new System.NotImplementedException();
    }
}
