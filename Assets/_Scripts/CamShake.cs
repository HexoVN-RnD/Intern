using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    private static CamShake instance;
    public static CamShake Instance => instance;
    private void Awake()
    {
        instance = this;
    }
    public void Shake(float duration = 0.2f, float strength = 0.5f)
    {
        transform.DOShakePosition(duration, strength, 10, 90, false, true).SetLink(gameObject);
    }
}
