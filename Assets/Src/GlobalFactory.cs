using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GlobalFactory 
{
    private static Dictionary<GameObject, Pool> m_hPools;

    static GlobalFactory()
    {
        m_hPools = new Dictionary<GameObject, Pool>();
    }

    internal static Pool GetPool(GameObject hPrefab)
    {
        if (!m_hPools.ContainsKey(hPrefab))
            m_hPools.Add(hPrefab, new Pool(hPrefab));

        return m_hPools[hPrefab];
    }

    internal static T GetInstance<T>(GameObject hPrefab) where T : Component
    {
        return GetInstance<T>(hPrefab, true);
    }

    internal static T GetInstance<T>(GameObject hPrefab, bool bEnable) where T : Component
    {
        Pool hPool = GetPool(hPrefab);
        GameObject hObj = hPool.Get();

        T hComp = hObj.GetComponent<T>();

        if (hComp == null)
        {
            hPool.Recycle(hObj);
            return null;
        }
        else
        {
            return hComp;
        }
    }

    internal static GameObject GetInstance(GameObject hPrefab)
    {
        return GetPool(hPrefab).Get();
    }
    internal static GameObject GetInstance(GameObject hPrefab, bool bEnable)
    {
        return GetPool(hPrefab).Get(bEnable);
    }

    internal static void Recycle(GameObject hItem)
    {
        hItem.GetComponent<IPoolable>().Pool.Recycle(hItem);
    }
}
