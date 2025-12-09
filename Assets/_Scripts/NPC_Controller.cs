using Convai.Scripts;
using Convai.Scripts.Runtime.Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC_Controller : MonoBehaviour
{
    [Header("Ice Block Infont Of NPC")]
    [SerializeField] private List<GameObject> blockingIceCubes;

    [Header("Help Signal State NPC")]
    [SerializeField] GameObject helpStateNPC;
    [SerializeField] GameObject helpSignal;
    [SerializeField] GameObject hurryUpSignal;

    [Header("Happy Signal State NPC")]
    [SerializeField] GameObject happyStateNPC;
    [SerializeField] GameObject happySignal;

    [Header("Scene Transition")]
    [SerializeField] private SceneFader sceneFader;

    private Tween helpTween; // Lưu tween lại để quản lý
    private Tween hurryUpTimerTween;

    private bool isRescued = false;
    


    private void OnEnable()
    {
        GameEvent.OnIceBroken += TriggerHurryUp;// Đăng ký: Khi sự kiện OnIceBroken xảy ra -> Chạy hàm TriggerHurryUp
    }
    private void OnDisable()
    {
        // Hủy đăng ký: Rất quan trọng! Nếu quên dòng này sẽ gây lỗi khi reload scene
        GameEvent.OnIceBroken -= TriggerHurryUp;
    }
    private void Start()
    {
        
        this.ShowHelpState();
    }
    private void Update()
    {
        if (isRescued) return;
        
        blockingIceCubes.RemoveAll(item => item == null || !item.activeInHierarchy);
        if (blockingIceCubes.Count == 0)
        {
            this.PerformRescue();
        }
    }
    private void TriggerHurryUp()
    {
        if (isRescued) return;
        if (hurryUpTimerTween != null) hurryUpTimerTween.Kill();// Hủy đếm ngược nếu được cứu
        helpSignal.SetActive(false);
        hurryUpSignal.SetActive(true);

        // Effect rung lắc
        hurryUpSignal.transform.DOKill();
        hurryUpSignal.transform.localScale = Vector3.one;
        hurryUpSignal.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);

        hurryUpTimerTween = DOVirtual.DelayedCall(2.5f, () =>
        {
            if (!isRescued)
            {
                hurryUpSignal.SetActive(false);
                helpSignal.SetActive(true);
            }
        }).SetLink(gameObject);
    }
    private void ShowHelpState()
    {
        helpStateNPC.SetActive(true);
        happyStateNPC.SetActive(false);
        hurryUpSignal.SetActive(false);

        helpSignal.SetActive(true);
        happySignal.SetActive(false);

        // Lưu tween vào biến để quản lý, dùng SetLink để an toàn
        helpTween = helpSignal.transform.DOScale(1.1f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetLink(helpSignal); ;
    }
    private void PerformRescue()
    {
        isRescued = true;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopHelpSound();  // Tắt tiếng kêu cứu
            SoundManager.Instance.PlayHappySound(); // Bật tiếng reo hò
            SoundManager.Instance.StopBGMusic(); // tat bg music
        }

        if (helpTween != null) helpTween.Kill();
        helpStateNPC.SetActive(false);
        happyStateNPC.SetActive(true);
      

        helpSignal.SetActive(false);
        hurryUpSignal.SetActive(false);
        happySignal.SetActive(true);

        happySignal.transform.localScale = Vector3.zero;
        happySignal.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetLink(happySignal); // Thêm .SetLink(...) vào các lệnh DOTween. Điều này đảm bảo nếu NPC bị tắt hoặc destroy đột ngột, các tween này sẽ tự hủy theo, tránh gây lỗi đỏ console.;

        happyStateNPC.transform.DOJump(happyStateNPC.transform.position, 0.5f, 2, 1f);
        //PlayerPrefs.SetString("AI", SceneManager.GetActiveScene().name);
        //Invoke("LoadAIScene", 2f);
        //DOVirtual.DelayedCall(3.2f, ReloadScene).SetLink(gameObject);
        DOVirtual.DelayedCall(2f, () => {
            if (sceneFader != null)
            {
                sceneFader.FadeToScene("AI");
            }
            else 
            {
                SceneManager.LoadScene("AI");
            }
        }).SetLink(gameObject);

    }
    //public void LoadAIScene()
    //{
    //    SceneManager.LoadScene("AI");
    //}
}
