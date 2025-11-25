using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMyPool : MonoBehaviour
{
    public MyPool myPool;

    public void OnDisable()
    {
        myPool.AddToMyPool(gameObject);
    }
}

