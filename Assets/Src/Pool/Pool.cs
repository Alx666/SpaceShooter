using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool<T> where T : class, IPoolable
{
    private Queue<T>    m_hInstances;
    private Object      m_hPrefabResource;


    public Pool(Object hPrefabResource)
    {
        m_hInstances = new Queue<T>();
        m_hPrefabResource = hPrefabResource;
    }

    public T Get()
    {
        T hItem;

        if (m_hInstances.Count > 0)
        {
            hItem = m_hInstances.Dequeue();            
        }
        else
        {
            hItem = (GameObject.Instantiate(m_hPrefabResource) as GameObject).GetComponent<T>();            
        }

        hItem.Pool = this as Pool<IPoolable>;
        hItem.Enable();
        this.ActiveInstances++;
        return hItem;
    }

    public void Recycle(T hItem)
    {
        hItem.Disable();
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
    int PoolId { get; }

    Pool<IPoolable> Pool { get; set; }

    void Enable();

    void Disable();
}
