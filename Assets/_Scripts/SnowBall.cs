using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    [SerializeField] private GameObject impactSnowBallEffect;
    [SerializeField] private GameObject splatProjector;
    private Tween myTween;
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 impactPoint = collision.contacts[0].point;// lay diem va cham dau tien
        Vector3 impactNormal = collision.contacts[0].normal;// lay vecto phap tuyen diem va cham
        if (impactSnowBallEffect != null)
        {
            GameObject g = PoolManager.Instance.GetFromPool(impactSnowBallEffect);
            g.transform.position = impactPoint;
            g.transform.rotation = Quaternion.identity;
            //Instantiate(impactSnowBallEffect, impactPoint, transform.rotation);// can obj pooling de toi uu }
        }
        Debug.Log("sasd");
        //Destroy(gameObject);
        if (myTween != null)
        {
            myTween.Kill();
            myTween = null;
        }

        if (splatProjector != null && collision.gameObject.CompareTag("Wall"))
        {
            Quaternion splatRotation = Quaternion.LookRotation(-impactNormal);// "LookRotation" sẽ xoay cái Quad của chúng ta
                                                                              // sao cho nó "hướng mặt" theo hướng của bề mặt (normal)
            Vector3 splatPos = impactPoint + (impactNormal * 0.5f) + new Vector3(0, -0.05f, 0);// Nudge (đẩy) vết bắn ra ngoài một chút (0.01f)
                                                                                               // để tránh nó bị "nhấp nháy" (Z-fighting) khi nằm trùng mặt phẳng với tường
                                                                                               //GameObject splat = Instantiate(splatProjector, splatPos, splatRotation);
            GameObject g = PoolManager.Instance.GetFromPool(splatProjector);
            g.transform.position = splatPos;
            g.transform.rotation = splatRotation;
            g.transform.SetParent(collision.transform);
            Projector proj = g.GetComponent<Projector>();
            if (proj != null)
            {
                if (!proj.material.name.Contains("Instance")) proj.material = new Material(proj.material);
                                                                                                          //Material matInstance = new Material(proj.material);// Tạo bản sao material để không bị ảnh hưởng các vết khác
                Color c = proj.material.color;// Reset màu về 1 (vì lấy từ pool có thể đang tàng hình)
                c.a = 1f;
                proj.material.color = c;

                proj.material.DOFade(0, 2f).SetDelay(1.5f).SetLink(g).OnComplete(() =>
                {
                    //Destroy(splat);
                    if (g != null) g.gameObject.SetActive(false);
                });
            }
        }
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
    public void Setup(Tween jumpTween)
    {
        this.myTween = jumpTween;
    }
}
