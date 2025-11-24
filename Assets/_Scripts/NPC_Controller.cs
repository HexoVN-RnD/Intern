using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Globalization;

public class NPC_Controller : MonoBehaviour
{
    [Header("Ice Block Infont Of NPC")]
    [SerializeField] private List<GameObject> blockingIceCubes;

    [Header("Help Signal State NPC")]
    [SerializeField] GameObject helpStateNPC;
    [SerializeField] GameObject helpSignal;

    [Header("Happy Signal State NPC")]
    [SerializeField] GameObject happyStateNPC;
    [SerializeField] GameObject happySignal;
    private Tween helpTween; // Lưu tween lại để quản lý

    private bool isRescued = false;

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
    private void ShowHelpState()
    {
        helpStateNPC.SetActive(true);
        happyStateNPC.SetActive(false);

        helpSignal.SetActive(true);
        happySignal.SetActive(false);

        // Lưu tween vào biến để quản lý, dùng SetLink để an toàn
        helpTween = helpSignal.transform.DOScale(1.1f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetLink(helpSignal); ;
    }
    private void PerformRescue()
    {
        isRescued = true;
        if (helpTween != null) helpTween.Kill();
        helpStateNPC.SetActive(false);
        happyStateNPC.SetActive(true);

        helpSignal.SetActive(false);
        happySignal.SetActive(true);

        happySignal.transform.localScale = Vector3.zero;
        happySignal.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetLink(happySignal); // Thêm .SetLink(...) vào các lệnh DOTween. Điều này đảm bảo nếu NPC bị tắt hoặc destroy đột ngột, các tween này sẽ tự hủy theo, tránh gây lỗi đỏ console.;

        happyStateNPC.transform.DOJump(happyStateNPC.transform.position, 0.5f, 2, 1f);
    }
}
