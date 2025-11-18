using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using Unity.VisualScripting;

public class Throw : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Transform attackPoint;
    public GameObject snowBall;
    [Header("Setting")]
    public int totalThrows;
    public float throwCooldown;
    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    //public float throwForce;
    //public float throwUpwardForce;
    [SerializeField] float jumpPower = 3f;
    [SerializeField] float duration = 0.3f;


    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;

    }
    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throwing();
        }
    }
    private void Throwing()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //Tạo Ray từ camera đi qua vị trí chuột trên màn hình

        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }
        Vector3 dir = (targetPoint - attackPoint.position).normalized;

        readyToThrow = false;

        GameObject g = Instantiate(snowBall, attackPoint.position, Quaternion.identity);// can obj pooling de toi uu

        //Rigidbody rbGameObj = g.GetComponent<Rigidbody>();

        Tween jumpTween = g.transform.DOJump(targetPoint, jumpPower, 1, duration).SetEase(Ease.OutQuad).OnComplete(() => { Destroy(g); });
                                                                        // hàm OnComplete ở đây sẽ đc gọi nếu quả bóng ném trượt khổi target

        g.GetComponent<SnowBall>().Setup(jumpTween);

        //rbGameObj.AddForce(forceAdd, ForceMode.Impulse);

        totalThrows--;
        Invoke(nameof(ResetThrow), throwCooldown);
        //Destroy(g, duration);
    }
    private void ResetThrow()
    {
        readyToThrow = true;
    }
   

}
