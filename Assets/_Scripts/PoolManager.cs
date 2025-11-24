using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance => instance;

    private Dictionary<GameObject,MyPool> dicPools = new Dictionary<GameObject,MyPool>();

    private void Awake()
    {
        PoolManager.instance = this;
    }
    public GameObject GetFromPool(GameObject obj) 
    {
        if (dicPools.ContainsKey(obj) == false) 
        {
            dicPools.Add(obj, new MyPool(obj));
        }
        return dicPools[obj].Get();
    }
}
