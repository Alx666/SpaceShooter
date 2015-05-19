using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class BulletController : MonoBehaviour 
{
    public float Speed;
    public float Damage;
    public float DestoryTime = 5.0f;


    private AudioSource audioShot;
    
    Vector3 dir;
    PlayerController playerController;



	
	void Start () 
    {
        this.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * Speed, ForceMode.Impulse);
        audioShot = this.GetComponent<AudioSource>();
        audioShot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioShot.priority = UnityEngine.Random.Range(128, 256);
        audioShot.volume = UnityEngine.Random.Range(0.5f, 1f);
	}
	
	
	void Update () 
    {
        OutOfScreenDestroy();
        
	}

    void OnTriggerEnter(Collider hColl)
    {
        IDamageable hToDmg = hColl.gameObject.GetComponent<IDamageable>();
        if (hToDmg != null)
        {
            try
            {
                hToDmg.Damage(this.Damage);
            }
            catch (Exception)
            {
                Debug.LogError("Bad Damage Implemenation");
            }
            finally
            {
                GameObject.Destroy(this.gameObject);
            }
        }
        else
        {
            Shield hShield = hColl.gameObject.GetComponent<Shield>();
            if (hShield)
            {
                //Has hit the shield, so reverse bullet direction
                Vector3 vCounterDirection = -(hShield.transform.position - this.transform.position);
                //Quaternion vRandomAngle = Quaternion.EulerAngles(0f, UnityEngine.Random.Range(0f, 10.0f), 0f);
                //vCounterDirection = vCounterDirection;
                vCounterDirection.Normalize();

                Rigidbody hPhysBody = this.GetComponent<Rigidbody>();
                this.transform.forward = vCounterDirection;
                hPhysBody.velocity = vCounterDirection * hPhysBody.velocity.magnitude;                
            }
        }
        
    }

    void OutOfScreenDestroy()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(this.transform.position);
        if((screenPos.x < -0.5f || screenPos.x > 1.5f) || (screenPos.y < -0.5f || screenPos.y > 1.5f))
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
