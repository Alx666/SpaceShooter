using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletFactory 
{
    private static Dictionary<int, Pool<IPoolable>> m_hPools;

    static BulletFactory()
    {
        m_hPools = new Dictionary<int, Pool<IPoolable>>();
    }

    internal static Pool<IPoolable> GetPool(Object BulletPrefab)
    {
        GameObject hItem = BulletPrefab as GameObject;
        IPoolable hPoolable = hItem.GetComponent<IPoolable>();

        if (!m_hPools.ContainsKey(hPoolable.PoolId))
            m_hPools.Add(hPoolable.PoolId, new Pool<IPoolable>(BulletPrefab));

        return m_hPools[hPoolable.PoolId];
    }
}
