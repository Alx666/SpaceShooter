using UnityEngine;
using System.Collections;

public class AsteroidController : MonoBehaviour, IDamageable
{
    public float ContactDamage;
    public float Health;
    public float Tumble;


    public float currentHealth { get; private set; }
	// Use this for initialization

    static Object resourceExplosion;
    GameObject explosion;
    Rigidbody rBody;


    void Awake()
    {
        currentHealth = Health;
        rBody = this.GetComponent<Rigidbody>();
        resourceExplosion = Resources.Load("Explosion_FX");
    }

    void Start()
    {
        rBody.angularVelocity = Random.insideUnitSphere * Tumble;
    }
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    public void Damage(float fDmg)
    {
        currentHealth -= fDmg;
        if (currentHealth <= 0)
            Death();
    }

    public void Death()
    {
        explosion = GameObject.Instantiate(resourceExplosion) as GameObject;
        explosion.gameObject.transform.position = this.transform.position;
        GameObject.Destroy(this.gameObject);
        
    }
}
