using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCInputManager : MonoBehaviour
{
    [Header("--- OSC Settings ---")]
    [SerializeField] private OSCReceiver oscReceiver;
    [SerializeField] private string oscAddress = "/hit";
    [SerializeField] private int listenPort = 7000;
    [Header("--- Calibration ---")]// can chinh
    [SerializeField] private bool flipX = false;
    [SerializeField] private bool flipY = false;
    [Header("--- Gameplay Setup ---")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject snowballPrefab;
    [SerializeField] private LayerMask iceLayerMask;

    private void Start()
    {
        if (oscReceiver != null) return;
        oscReceiver = gameObject.AddComponent<OSCReceiver>();
        oscReceiver.LocalPort = listenPort;
        oscReceiver.Bind(oscAddress, OnOSCReceived);
    }
    private void OnOSCReceived(OSCMessage message)
    {
        if (message.Values.Count < 2) return;
        float x = message.Values[0].FloatValue;
        float y = message.Values[1].FloatValue;
        Debug.Log($"Unity Nhận: X={x} | Y={y}");
        if (flipX) x = 1f - x;
        if (flipY) y = 1f - y;

        Vector3 viewportPos = new Vector3(x, y, mainCam.nearClipPlane);
        Ray ray = mainCam.ViewportPointToRay(viewportPos);//ViewportPointToRay fix cung man hinh unity nhan toa do tu (0,0)->(1,1)
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, iceLayerMask))
        {
            TriggerInstantHit(hit);
        }
    }
    private void TriggerInstantHit(RaycastHit hit)
    {
        if (snowballPrefab == null) return;

        GameObject ball = PoolManager.Instance.GetFromPool(snowballPrefab);
        ball.transform.position = hit.point;
        ball.transform.rotation = Quaternion.identity;

        Renderer r = ball.GetComponent<Renderer>();
        if (r != null) r.enabled = false;

        Collider c = ball.GetComponent<Collider>();
        if (c != null) c.enabled = false;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        SnowBall snowBall = ball.GetComponent<SnowBall>();
        if (snowBall != null)
        {
            ball.SetActive(true);
            snowBall.ForceCollision(hit);
        }
        else 
        {
            Destroy(ball);
        } 
    }
}
