using UnityEngine;
using System.Collections;

public class DelayedPoolRecycler : MonoBehaviour, IPoolable
{
    public float Delay = 3f;


    public Pool Pool { get; set; }

    public void Enable()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(RecycleAfterTime());
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
    }

    private IEnumerator RecycleAfterTime()
    {
        yield return new WaitForSeconds(Delay);
        Pool.Recycle(this.gameObject);
    }
}
