using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPool
{
    private Stack<GameObject> stack = new Stack<GameObject>();
    private GameObject baseObj;
    private GameObject tmp;
    private ReturnToMyPool returnToMyPool;

    public MyPool(GameObject baseObj)
    {
        this.baseObj = baseObj;
    }
    public GameObject Get()
    {
        if (stack.Count > 0)
        {
            tmp = stack.Pop();
            if (tmp != null)
            {
                tmp.transform.SetParent(null);
                tmp.SetActive(true);
                return tmp;
            }

        }
        tmp = GameObject.Instantiate(baseObj);
        returnToMyPool = tmp.GetComponent<ReturnToMyPool>();
        returnToMyPool.myPool = this;
        return tmp;
    }
    public void AddToMyPool(GameObject obj)
    {
        stack.Push(obj);
    }
}
