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

    private bool hasCollided = false;

    private void OnEnable()
    {
        hasCollided = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            PerformHit(contact.point, contact.normal, collision.gameObject);
        }

        //hasCollided = true;

    }
    public void ForceCollision(RaycastHit hit)
    {
        if (hasCollided) return;
        PerformHit(hit.point, hit.normal, hit.collider.gameObject);
    }
    private void PerformHit(Vector3 hitPoint, Vector3 hitNormal, GameObject hitObject)
    {
        hasCollided = true; // Khóa lại ngay lập tức để tránh nổ kép
        Vector3 impactPoint = hitPoint;// lay diem va cham dau tien
        Vector3 impactNormal = hitNormal;// lay vecto phap tuyen diem va cham
        if (impactSnowBallEffect != null)
        {
            Vector3 effectSpawn = impactPoint + (impactNormal * 0.5f);
            GameObject g = PoolManager.Instance.GetFromPool(impactSnowBallEffect);
            g.transform.position = effectSpawn;
            g.transform.rotation = Quaternion.identity;
            //Instantiate(impactSnowBallEffect, impactPoint, transform.rotation);// can obj pooling de toi uu }
        }
        //Debug.Log("sasd");

        //KẾT NỐI VỚI ICE BREAK MANAGER
        IceBreakManager manager = FindObjectOfType<IceBreakManager>();
        if (manager != null)
        {
            // Báo cáo: "Tôi ném trúng thằng này, tại vị trí này"
            manager.ProcessHit(hitObject, hitPoint);
        }
        else
        {
            // Debug để biết nếu quên chưa tạo Manager
            // Debug.LogWarning("Chưa có IceBreakManager trong Scene!");
        }

        if (myTween != null)
        {
            myTween.Kill();
            myTween = null;
        }

        if (splatProjector != null && hitObject.gameObject.CompareTag("Wall"))
        {
            Quaternion splatRotation = Quaternion.LookRotation(-impactNormal);// "LookRotation" sẽ xoay cái Quad của chúng ta
                                                                              // sao cho nó "hướng mặt" theo hướng của bề mặt (normal)
            Vector3 splatPos = impactPoint + (impactNormal * 0.5f) + new Vector3(0, -0.05f, 0);// Nudge (đẩy) vết bắn ra ngoài một chút (0.01f)
                                                                                               // để tránh nó bị "nhấp nháy" (Z-fighting) khi nằm trùng mặt phẳng với tường
                                                                                               //GameObject splat = Instantiate(splatProjector, splatPos, splatRotation);
            GameObject g = PoolManager.Instance.GetFromPool(splatProjector);
            g.transform.position = splatPos;
            g.transform.rotation = splatRotation;
            g.transform.SetParent(hitObject.transform);
            Projector proj = g.GetComponent<Projector>();
            if (proj != null)
            {

                //if (!proj.material.name.Contains("Instance")) proj.material = new Material(proj.material);
                Material mat = proj.material;
                //Material matInstance = new Material(proj.material);// Tạo bản sao material để không bị ảnh hưởng các vết khác
                Color c = mat.color;// Reset màu về 1 (vì lấy từ pool có thể đang tàng hình)
                c.a = 1f;
                mat.color = c;

                proj.material.DOFade(0, 2f).SetDelay(2f).SetLink(g).OnComplete(() =>
                {
                    //Destroy(splat);
                    if (g != null) { g.gameObject.SetActive(false); g.transform.SetParent(null); }
                    
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
