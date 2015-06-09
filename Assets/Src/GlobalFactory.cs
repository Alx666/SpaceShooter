using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalFactory 
{
    private static Dictionary<int, Pool<IPoolable>> m_hPools;

    static GlobalFactory()
    {
        m_hPools = new Dictionary<int, Pool<IPoolable>>();
    }

    internal static Pool<IPoolable> GetPool(Object hPrefab)
    {
        GameObject hItem = hPrefab as GameObject;
        IPoolable hPoolable = hItem.GetComponent<IPoolable>();

        if (!m_hPools.ContainsKey(hPoolable.PoolId))
            m_hPools.Add(hPoolable.PoolId, new Pool<IPoolable>(hPrefab));

        return m_hPools[hPoolable.PoolId];
    }
}
