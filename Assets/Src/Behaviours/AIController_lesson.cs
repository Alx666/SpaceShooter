using UnityEngine;
using System.Collections;

public class AIController_lesson : MonoBehaviour {

    WorldController worldController;
    new Rigidbody rBody;


    void Awake()
    {
        worldController = this.gameObject.GetComponent<WorldController>();
        rBody = this.gameObject.GetComponent<Rigidbody>();
        worldController.enabled = false;
        //GameManager.Instance.RegisterEnemy(this);
    }
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void FixedUpdate()
    {
        if (!worldController.enabled)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            if((screenPos.x > 0f && screenPos.y > 0f) && (screenPos.x < Camera.main.pixelWidth && screenPos.y < Camera.main.pixelHeight))
            {
                worldController.enabled = true;
            }
        }
    }
}
