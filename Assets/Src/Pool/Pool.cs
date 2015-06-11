using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool
{
    private Queue<GameObject>       m_hInstances;
    private GameObject              m_hPrefabResource;

    public Pool(GameObject hPrefabResource)
    {
        m_hInstances        = new Queue<GameObject>();
        m_hPrefabResource   = hPrefabResource;
    }


    public GameObject Get()
    {
        return Get(true);
    }

    public GameObject Get(bool bEnable)
    {                
        GameObject hItem;

        if (m_hInstances.Count > 0)
        {
            hItem = m_hInstances.Dequeue();            
        }
        else
        {
            hItem = (GameObject.Instantiate(m_hPrefabResource) as GameObject);
        }

        IPoolable hPoolable = hItem.GetComponent<IPoolable>();
        if (hPoolable == null)
            throw new UnityException("Non IPoolable in Object Pool " + m_hPrefabResource.name);

        hPoolable.Pool = this;

        if(bEnable)
            hPoolable.Enable();

        this.ActiveInstances++;
        return hItem;
    }

    public void Recycle(GameObject hItem)
    {
        IPoolable hPollable = hItem.GetComponent<IPoolable>();
        hPollable.Disable();
        this.ActiveInstances--;
        m_hInstances.Enqueue(hItem);
    }

    public int Count
    {
        get { return m_hInstances.Count; }
    }

    public int ActiveInstances { get; private set; }
}

public interface IPoolable
{
    Pool Pool { get; set; }

    void Enable();

    void Disable();
}
