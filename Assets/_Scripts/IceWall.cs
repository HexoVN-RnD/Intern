using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IceWall : MonoBehaviour
{
    [SerializeField] private int Hp = 5;
    [SerializeField] private GameObject iceWallFractured1;
    [SerializeField] private GameObject iceWallFractured2;
    private int currentHit = 0;
    [SerializeField] protected VideoPlayer vFXVideo;
    [SerializeField] protected VideoPlayer vFXVideo1;
    private Renderer vRenderer;
    private Collider vCollider;

    private void Start()
    {
        vRenderer = GetComponent<Renderer>();
        vCollider = GetComponent<Collider>();
        vFXVideo.gameObject.SetActive(false);
        vFXVideo1.gameObject.SetActive(false);
    }
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
            //gameObject.SetActive(false);
            //iceWallFractured1.gameObject.SetActive(true);
            //iceWallFractured2.gameObject.SetActive(true);
            StartCoroutine(BreakIceSequence());

            SoundManager.Instance.PlayCrackIce();
        }
    }
    IEnumerator BreakIceSequence() 
    {
        if (vRenderer != null) 
        {
            vRenderer.enabled = false;
        }
        if (vCollider != null) 
        {
            vCollider.enabled = false;
        }
        if (vFXVideo != null && vFXVideo1 != null) 
        {
            vFXVideo.gameObject.SetActive(true);
            vFXVideo1.gameObject.SetActive(true);
            vFXVideo.Play();
            vFXVideo1.Play();


            float videoDuration = (float)vFXVideo.length;
            float videoDuration1 = (float)vFXVideo1.length;
            if (videoDuration <= 0) videoDuration = 1;
            if (videoDuration1 <= 0) videoDuration1 = 1;

            yield return new WaitForSeconds(videoDuration);
            yield return new WaitForSeconds(videoDuration1);

            vFXVideo.gameObject.SetActive(false);
            vFXVideo1.gameObject.SetActive(false);
        }
        if (iceWallFractured1 != null) iceWallFractured1.SetActive(true);
        if (iceWallFractured2 != null) iceWallFractured2.SetActive(true);
        gameObject.SetActive(false);
        
    }
}