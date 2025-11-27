using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    [SerializeField] private int Hp = 5;
    private int currentHit = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SnowBall"))
        {
            this.TakeDame();
            SoundManager.Instance.PlaySnowBallHitSound();
        }
    }
    private void TakeDame()
    {
        currentHit++;
        if (currentHit >= Hp) 
        {
            gameObject.SetActive(false);
        }
    }
}
