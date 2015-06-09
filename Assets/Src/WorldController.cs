using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour 
{

    private const float m_fJumpDist = 0.01f;

	void Start () 
    {
	}
	

	void Update () 
    {       
        Vector3 vScreenPosition = Camera.main.WorldToViewportPoint(this.gameObject.transform.position); //0,1

        if (vScreenPosition.x > 1f)
        {
            Vector3 vNewPoint = new Vector3(m_fJumpDist, vScreenPosition.y, Camera.main.transform.position.y);
            Vector3 vWorldPoint = Camera.main.ViewportToWorldPoint(vNewPoint);
            this.gameObject.transform.position = vWorldPoint;
        }
        else if (vScreenPosition.x < 0f)
        {
            Vector3 vNewPoint = new Vector3(1f - m_fJumpDist, vScreenPosition.y, Camera.main.transform.position.y);
            Vector3 vWorldPoint = Camera.main.ViewportToWorldPoint(vNewPoint);
            this.gameObject.transform.position = vWorldPoint;
        }
        else if (vScreenPosition.y > 1f)
        {
            Vector3 vNewPoint = new Vector3(vScreenPosition.x, m_fJumpDist, Camera.main.transform.position.y);
            Vector3 vWorldPoint = Camera.main.ViewportToWorldPoint(vNewPoint);
            this.gameObject.transform.position = vWorldPoint;
        }
        else if (vScreenPosition.y < 0f)
        {
            Vector3 vNewPoint = new Vector3(vScreenPosition.x, 1 - m_fJumpDist, Camera.main.transform.position.y);
            Vector3 vWorldPoint = Camera.main.ViewportToWorldPoint(vNewPoint);
            this.gameObject.transform.position = vWorldPoint;
        }
	}
}
