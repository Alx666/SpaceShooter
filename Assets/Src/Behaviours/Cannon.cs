using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour 
{
    private IWeapon Weapon { get; set; }

    public float CoolDown = 0.3f;
    public bool ConeFire = true;
    public float ConeAngle = 1.0f;
    public float BulletSpeedModif = 1.0f;

    void Awake()
    {
        Weapon = new WeaponAntiMatter(this.gameObject, CoolDown, ConeFire, BulletSpeedModif, ConeAngle);
        //Weapon = new WeaponLaser(this.gameObject, this.GetComponent<LineRenderer>(), 20f);
    }

    public void Update()
    {
        Weapon.OnUpdate();
    }

    public void Fire()
    {
        Weapon.OnbuttonPressed();
        Debug.Log("Fire");
    }

    public void StopFire()
    {
        Weapon.OnbuttonReleased();
    }

	void Start () 
    {
	
	}
}
