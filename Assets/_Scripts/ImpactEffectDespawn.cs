using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffectDespawn : MonoBehaviour
{
    public float timer = 0f;
    public float delay = 2f;
    private void FixedUpdate()
    {
        this.timer += Time.fixedDeltaTime;
        if (this.timer < this.delay) return;
        this.timer = 0;
        gameObject.SetActive(false);
    }
}
