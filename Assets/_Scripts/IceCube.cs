using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCube : MonoBehaviour
{
    [Header("Cài đặt Máu")]
    public int maxHits = 2; // Số lần ném cần thiết để vỡ (bạn đặt là 2)
    private int currentHits = 0;
    [Header("Cài đặt Hình ảnh")]
    public GameObject shatteredPrefab; // Prefab các mảnh vỡ
    [Header("Cài đặt Lực nổ")]
    public float explosionForce = 300f;
    public float explosionRadius = 2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SnowBall"))
        {
            TakeDamage();
        }
    }
    void TakeDamage()
    {
        currentHits++;

        if (currentHits >= maxHits)
        {
            // Nếu đủ số lần ném -> VỠ TAN
            Shatter();
        }
        else
        {
            // Nếu chưa đủ -> HIỆN VẾT NỨT
        }
    }
    void Shatter()
    {
        // 1. Tạo phiên bản vỡ
        GameObject brokenObj = Instantiate(shatteredPrefab, transform.position, transform.rotation);
        brokenObj.transform.localScale = transform.localScale;

        // 2. Tạo lực nổ cho các mảnh vỡ
        Rigidbody[] rbs = brokenObj.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // 3. Dọn dẹp
        Destroy(brokenObj, 3f); // Hủy mảnh vỡ sau 5s
        Destroy(gameObject);    // Hủy khối băng gốc ngay lập tức
    }
}