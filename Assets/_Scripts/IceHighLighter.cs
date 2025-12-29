using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceHighLighter : MonoBehaviour
{
    private Renderer rend;
    private Material targetMat;
    private Color originalColor;
    private Tween blinkTween;
    private Coroutine cycleCoroutine; // Biến để quản lý vòng lặp Show/Hide

    [Header("Highlight Visuals")]
    public Color glowColor = new Color(0.6f, 0.6f, 0.6f);
    public float blinkDuration = 0.8f;

    [Header("Hint Cycle Settings")]
    public float showDuration = 3.0f; // Thời gian hiện (3s)
    public float hideDuration = 3.0f; // Thời gian ẩn (3s)

    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            targetMat = rend.material;
            if (targetMat.HasProperty("_Ice_Color")) originalColor = targetMat.GetColor("_Ice_Color");
            cycleCoroutine = StartCoroutine(HintCycleRoutine());
        }
    }
    IEnumerator HintCycleRoutine() 
    {
        while (true) 
        {
            StartBlinking();
            yield return new WaitForSeconds(showDuration);
            StopTweenOnly(); // Chỉ tắt tween, chưa hủy component
            yield return new WaitForSeconds(hideDuration);
        }
    }
    public void StartBlinking()
    {
        if (!targetMat.HasProperty("_Ice_Color")) return;
        Color targetGlow = originalColor + glowColor;// Tính toán màu mục tiêu (Màu gốc + Màu glow)
        // Dùng DOTween để thay đổi màu sắc của thuộc tính "_Ice_Color"

        blinkTween = targetMat.DOColor(targetGlow, "_Ice_Color", blinkDuration).
                                                    SetLoops(-1, LoopType.Yoyo).// Lặp vô tận kiểu sáng-tối-sáng
                                                    SetEase(Ease.InOutSine).// Chuyển động mượt mà
                                                    SetLink(gameObject);// Tự hủy tween nếu object bị hủy
    }
    public void StopTweenOnly() 
    {
        if(blinkTween != null) blinkTween.Kill();
        // Trả lại màu gốc ngay lập tức để trong 3s nghỉ nhìn nó bình thường
        if (rend != null && targetMat != null && targetMat.HasProperty("_Ice_Color")) 
        {
            targetMat.SetColor("_Ice_Color", originalColor);
        }
    }
    public void StopHighlight()// Hàm public: Dừng HẲN và hủy component (Được NPC gọi khi ném trúng)
    {
        if(cycleCoroutine!= null) StopCoroutine(cycleCoroutine);
        StopTweenOnly();
        // Tự hủy component này đi để tiết kiệm tài nguyên
        Destroy(this);
    }
    // Đảm bảo dọn dẹp nếu object bị tắt đột ngột
    private void OnDisable()
    {

        if (cycleCoroutine != null) StopCoroutine(cycleCoroutine);
        StopTweenOnly();
        
    }
}
