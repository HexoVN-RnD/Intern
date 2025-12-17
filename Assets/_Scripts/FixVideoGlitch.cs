using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FixVideoGlitch : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private Renderer mRen;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        mRen = GetComponent<Renderer>();

        if (mRen != null)
        {
            mRen.enabled = false;
            // Set Alpha (độ trong suốt) về 0 để chuẩn bị fade in
            Material mat = mRen.material;
            Color color = mat.color;
            color.a = 0f;
            mat.color = color;
        }
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;

        videoPlayer.prepareCompleted += OnVideoPrepare;// Đăng ký sự kiện: "Khi nào chuẩn bị xong thì gọi hàm OnVideoPrepared"

        videoPlayer.Prepare();// Bắt đầu nạp video
    }
    void OnVideoPrepare(VideoPlayer vp)
    {
        if (mRen != null)
        {
            mRen.enabled = true; mRen.material.DOFade(1f, 1f);
        }
        vp.Play();
    }
}
//Lệnh videoPlayer.prepareCompleted giống như một người gác cổng, nó đảm bảo chỉ khi nào video đã lên nòng thì mới cho phép hiển thị tấm Quad ra.