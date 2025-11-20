using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeBreakLogic : MonoBehaviour
{
    [Header("Settings")]
    public float connectionRadius = 0.2f; // Bán kính tìm hàng xóm (tùy chỉnh theo kích thước mảnh vỡ)
    public LayerMask iceLayer; // Layer của các mảnh băng (IceDebris)
    public LayerMask staticLayer; // Layer của Tường/Sàn (Wall, Floor) - Những thứ giữ băng lại

    private List<IceCubeBreakLogic> iceNeighbors = new List<IceCubeBreakLogic>();
    private bool isAnchored = false; // Biến kiểm tra xem mảnh này có dính vào ice khac thật không
    private Rigidbody rb;
    private bool isBroken = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        this.FindNeighBorAndAnchor();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isBroken && collision.gameObject.CompareTag("SnowBall"))
        {
            Vector3 dir = collision.relativeVelocity.normalized;
            BreakOff(dir, 5f);
        }
    }
    private void BreakOff(Vector3 forceDir, float forceStrenght)
    {
        if (isBroken) return;
        isBroken = true;

        rb.isKinematic = false;


        rb.AddForce(forceDir * forceStrenght, ForceMode.Impulse);

        Destroy(gameObject, 5);

        this.NotifyNeibors();
    }
    private void FindNeighBorAndAnchor()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, connectionRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && (iceLayer.value & (1 << hit.gameObject.layer)) > 0)
            {
                IceCubeBreakLogic neighbor = hit.GetComponent<IceCubeBreakLogic>();
                if (neighbor != null)
                {
                    iceNeighbors.Add(neighbor);
                }
            }
            if ((staticLayer.value & (1 << hit.gameObject.layer)) > 0)
            {
                isAnchored = true;
            }
        }
    }
    private void NotifyNeibors() // Báo cho tất cả hàng xóm biết là tôi đã rơi rồi
    {
        foreach (var neighbor in iceNeighbors) 
        {
            // Nếu hàng xóm chưa rơi, bảo nó kiểm tra lại độ vững chãi
            if (neighbor != null && !neighbor.isBroken)
            {
                neighbor.CheckStability();
            }
        }
    }
    private void CheckStability() 
    {
        if (isBroken) return;

        // Nếu mình đang dính chặt vào tường/đất -> OK, không bao giờ rơi
        if (isAnchored) return;

        // Đếm xem còn bao nhiêu hàng xóm "còn sống" (chưa rơi) đang giữ mình
        int activeNeighbors = 0;
        foreach (var neighbor in iceNeighbors)
        {
            if (neighbor != null && !neighbor.isBroken)
            {
                activeNeighbors++;
            }
        }

        // --- LOGIC QUYẾT ĐỊNH RƠI ---
        // Nếu không còn ai giữ mình (0 hàng xóm), hoặc quá ít hàng xóm (ví dụ < 2) -> RƠI LUÔN
        if (activeNeighbors == 0)
        {
            // Rơi tự do (không cần lực đẩy mạnh, chỉ cần gravity)
            BreakOff(Vector3.down, 0.1f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, connectionRadius);
    }
}
