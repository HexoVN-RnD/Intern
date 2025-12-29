using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBreakManager : MonoBehaviour
{
    public static Action OnAnyHit;

    [Header("--- Setup References ---")]
    [SerializeField] private Transform stage1Root;
    [SerializeField] private Transform stage2Root;
    [SerializeField] private Transform stage3Root;

    [Header("--- Physics Settings ---")]
    [SerializeField] private float explosionForce = 50f;
    [SerializeField] private float disappearDelay = 2.0f;

    [Header("--- MANUAL MAPPING (Kéo thả ở đây) ---")]
    [SerializeField] public List<IceCluster> shardClusters = new List<IceCluster>();// Danh sách này sẽ hiện lên Inspector để chỉnh sửa

    //[Header("--- VFX Settings ---")]
    //// --- [MỚI] Thêm biến chứa Prefab Video ---
    //[SerializeField] private GameObject iceDustVideoPrefab;
    //[SerializeField] private float videoScale = 1.5f;
    [Header("--- VFX Settings ---")]
    // SỬA: Đổi từ VideoPlayer sang GameObject thường (vì Particle là GameObject)
    [SerializeField] private GameObject iceHitVFXPrefab;

    [SerializeField] private float neighborRadius = 2f;
    [SerializeField] private NPC_Controller nPC_Controller;

    private Dictionary<GameObject, GameObject> map1_to_2 = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, List<GameObject>> map2_to_3 = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> map3_lookup = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, GameObject> map3_to_1 = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> map2_to_1 = new Dictionary<GameObject, GameObject>();


    private void Start()
    {
        BuildRuntimeData();// Khi game bắt đầu, chuyển dữ liệu từ List (trên Inspector) vào Dictionary (trong Ram)
    }

    private void BuildRuntimeData()
    {
        //Ẩn các stage vỡ
        foreach (Transform t in stage2Root)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in stage3Root)
        {
            t.gameObject.SetActive(false);
        }
        //Map Stage 1 -> Stage 2 (Tự động theo tên - cái này ít khi sai nên để tự động)
        foreach (Transform s1 in stage1Root)
        {
            Transform s2 = stage2Root.Find(s1.name);
            if (s2 != null)
            {
                map1_to_2.Add(s1.gameObject, s2.gameObject); map2_to_1.Add(s2.gameObject, s1.gameObject);
            }
        }
        //Map Stage 2 -> Stage 3 (DỰA TRÊN LIST  ĐÃ CHỈNH Ở INSPECTOR)
        foreach (var cluster in shardClusters)
        {
            if (cluster.crackedPart == null) continue;
            //Map xuôi: Từ nứt -> Vụn
            if (!map2_to_3.ContainsKey(cluster.crackedPart))
            {
                map2_to_3.Add(cluster.crackedPart, cluster.shards);
            }
            GameObject originStage1 = null;
            //Tìm ra (Stage 1) của nhóm vụn này
            if (map2_to_1.ContainsKey(cluster.crackedPart))
            {
                originStage1 = map2_to_1[cluster.crackedPart];
            }
            //Map ngược: Từ vụn -> Nhóm
            foreach (var shard in cluster.shards)
            {
                if (shard == null) continue;

                if (!map3_lookup.ContainsKey(shard))
                {
                    map3_lookup.Add(shard, cluster.shards);
                }
                // [MỚI] Lưu map: Mảnh vụn này thuộc về Ông tổ Stage 1 nào
                if (originStage1 != null && !map3_to_1.ContainsKey(shard))
                {
                    map3_to_1.Add(shard, originStage1);
                }
                // Setup vật lý luôn để đảm bảo không quên
                Rigidbody rb = shard.GetComponent<Rigidbody>();
                if (rb == null) rb = shard.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                if (shard.GetComponent<Collider>() == null)
                {
                    MeshCollider mc = shard.AddComponent<MeshCollider>();
                    mc.convex = true;
                }
            }
        }
    }
    public void ProcessHit(GameObject hitObj, Vector3 hitPoint)
    {
        OnAnyHit?.Invoke();
        if (map1_to_2.ContainsKey(hitObj))
        {
            //GameObject s2 = map1_to_2[hitObj];
            //hitObj.SetActive(false);
            //s2.SetActive(true);
            //SoundManager.Instance.PlaySnowBallHitSound();

            GameObject stage2Obj = map1_to_2[hitObj];
            if (map2_to_3.ContainsKey(stage2Obj))
            {
                List<GameObject> shards = map2_to_3[stage2Obj];
                hitObj.gameObject.SetActive(false);
                stage2Obj.SetActive(false);

                foreach (var s in shards)
                {
                    s.gameObject.SetActive(true);
                }
                if (iceHitVFXPrefab != null)
                {
                    // Sinh ra Particle tại điểm va chạm (hitPoint) thì sẽ chuẩn hơn là tâm object
                    // Tuy nhiên, dùng hitObj.transform.position an toàn hơn nếu hitPoint bị lệch
                    //GameObject vfx = Instantiate(iceHitVFXPrefab, hitObj.transform.position, Quaternion.identity);
                    GameObject vfx = PoolManager.Instance.GetFromPool(iceHitVFXPrefab);
                    vfx.transform.position = hitObj.transform.position;
                    // Mẹo: Hướng vụn băng nổ ra phía ngoài (theo hướng pháp tuyến của mảnh băng)
                    vfx.transform.rotation = hitObj.transform.rotation;
                }
                SoundManager.Instance.PlayCrackIce();
            }
            else
            {
                // Phòng hờ: Nếu không tìm thấy Stage 3 thì chuyển sang Stage 2 thôi
                hitObj.SetActive(false);
                stage2Obj.SetActive(true);
            }
            // 2. XỬ LÝ HÀNG XÓM (Neighbors) -> Sang Stage 2 (Nứt)
            Collider[] neighbors = Physics.OverlapSphere(hitPoint, neighborRadius);
            foreach (var col in neighbors)
            {
                GameObject neighborObj = col.gameObject;
                // Chỉ xử lý nếu là mảnh Stage 1 và KHÔNG PHẢI mảnh vừa ném trúng
                if (neighborObj != hitObj && map1_to_2.ContainsKey(neighborObj))
                {
                    GameObject neighborStage2 = map1_to_2[neighborObj];
                    neighborObj.SetActive(false);
                    neighborStage2.SetActive(true);
                }
            }
        }
        else if (map2_to_3.ContainsKey(hitObj))
        {
            List<GameObject> shards = map2_to_3[hitObj];
            if (iceHitVFXPrefab != null)
            {
                // Sinh ra Particle tại điểm va chạm (hitPoint) thì sẽ chuẩn hơn là tâm object
                // Tuy nhiên, dùng hitObj.transform.position an toàn hơn nếu hitPoint bị lệch
                GameObject vfx = Instantiate(iceHitVFXPrefab, hitObj.transform.position, Quaternion.identity);

                // Mẹo: Hướng vụn băng nổ ra phía ngoài (theo hướng pháp tuyến của mảnh băng)
                vfx.transform.rotation = hitObj.transform.rotation;
            }
            hitObj.SetActive(false);
            foreach (var s in shards)
            {
                s.SetActive(true);
            }
            SoundManager.Instance.PlayCrackIce();
        }
        else if (map3_lookup.ContainsKey(hitObj))
        {
            List<GameObject> siblings = map3_lookup[hitObj];
            foreach (var s in siblings)
            {
                if (s == null) continue;
                Rigidbody rb = s.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explosionForce, hitPoint, 2f);
                }
            }

            if (map3_to_1.ContainsKey(hitObj))
            {
                GameObject originStage1 = map3_to_1[hitObj];
                if (nPC_Controller != null)
                {
                    nPC_Controller.ReportBrokenIce(originStage1);
                }
            }
            GameEvent.OnIceBroken?.Invoke();
            SoundManager.Instance.PlayBreakIce();
            DOVirtual.DelayedCall(disappearDelay, () =>
            {
                foreach (var s in siblings) if (s != null) s.SetActive(false);
            });
            if (CamShake.Instance != null) 
            {
                CamShake.Instance.Shake(0.3f, 0.15f);
            }
        }
    }
    public List<GameObject> GetActiveVisualsFromStage1(GameObject stage1Obj)
    {
        List<GameObject> result = new List<GameObject>();
        if (stage1Obj == null) return result;
        // 1. Nếu Stage 1 đang hiện -> Trả về nó
        if (stage1Obj.activeInHierarchy)
        {
            result.Add(stage1Obj);
            return result;
        }
        // 2. Nếu đã nứt sang Stage 2
        if (map1_to_2.ContainsKey(stage1Obj))
        {
            GameObject stage2Obj = map1_to_2[stage1Obj];
            if (stage2Obj != null && stage2Obj.activeInHierarchy) 
            {
                result.Add(stage2Obj);
                return result;
            }
            // 3. Nếu đã vỡ sang Stage 3 (Vụn)
            if (map2_to_3.ContainsKey(stage2Obj)) 
            {
                List<GameObject> shards = map2_to_3[stage2Obj];
                foreach (var s in shards) 
                {
                    // Chỉ lấy những mảnh còn đang active (chưa rơi mất)
                    if (s != null && s.activeInHierarchy) 
                    {
                        result.Add(s);
                    }    
                }
            }
        }
        return result;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, neighborRadius);
    }
}
