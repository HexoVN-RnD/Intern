using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FixVideoGlitch : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private Renderer mRen;

    // Tên biến màu của Shader Legacy/Particles/Additive là _TintColor
    // Nếu bạn đổi shader khác, hãy đổi string này thành "_Color"
    private string colorProperty = "_TintColor";

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        mRen = GetComponent<Renderer>();

        if (mRen != null)
        {
            mRen.enabled = false;

            // SỬA: Kiểm tra xem shader dùng tên gì (_Color hay _TintColor)
            if (mRen.material.HasProperty("_Color")) colorProperty = "_Color";
            else if (mRen.material.HasProperty("_TintColor")) colorProperty = "_TintColor";

            // SỬA: Lấy màu bằng tên chính xác
            Color color = mRen.material.GetColor(colorProperty);
            color.a = 0f;

            // SỬA: Gán màu bằng tên chính xác
            mRen.material.SetColor(colorProperty, color);
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.prepareCompleted += OnVideoPrepare;
        videoPlayer.Prepare();
    }

    void OnVideoPrepare(VideoPlayer vp)
    {
        if (mRen != null)
        {
            mRen.enabled = true;

            // SỬA: DOFade mặc định tìm _Color, nên với _TintColor nó sẽ không chạy hoặc lỗi.
            // Ta dùng DOColor hoặc chỉ định property cho DOFade (nếu bản DOTween mới hỗ trợ), 
            // nhưng an toàn nhất là dùng DOColor cho biến cụ thể và chỉnh Alpha của màu đích.

            Color targetColor = mRen.material.GetColor(colorProperty);
            targetColor.a = 1f; // Đích đến là Alpha = 1

            mRen.material.DOColor(targetColor, colorProperty, 1f);
        }
        vp.Play();
    }
}