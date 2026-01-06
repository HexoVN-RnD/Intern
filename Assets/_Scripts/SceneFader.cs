using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup blackPanel;
    [SerializeField] private float fadeDuration = 4f;
    [SerializeField] private bool fadeOnStart = true;
    private void Start()
    {
        if (fadeOnStart)
        {
            blackPanel.alpha = 1;
            blackPanel.blocksRaycasts = true;

            blackPanel.DOFade(0, fadeDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                blackPanel.blocksRaycasts = false;
            });
        }
    }
    public void FadeToScene(string sceneName, float overrideDuration = -1f)
    {
        blackPanel.blocksRaycasts = true; // Chặn click
        blackPanel.DOKill();

        float duration = overrideDuration > 0 ? overrideDuration : fadeDuration;

        // Fade từ 0 lên 1 (Đen dần)
        blackPanel.DOFade(1, fadeDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            // Khi đã đen kịt thì mới Load Scene
            SceneManager.LoadScene(sceneName);
        });
    }
}
