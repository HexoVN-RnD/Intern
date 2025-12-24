using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AutoDespawnVideo : MonoBehaviour
{
    public VideoPlayer player;

    private void Start()
    {
        player = GetComponent<VideoPlayer>();

        player.loopPointReached += OnVideoEnd;
    }
    public void OnVideoEnd(VideoPlayer vp) 
    {
        Destroy(gameObject);
    }
}
