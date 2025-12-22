using System.Collections;
using UnityEngine;

public class PredictiveAim : MonoBehaviour
{
    private Coroutine coroutine;
    private Vector3 LastTransform;
    private GameObject Player;
    private Rigidbody PlayerRB;
    [Tooltip("마지막 플레이어 위치 갱신 간격")]public float _CheckSecond = 0.5f;
    [Tooltip("플레이어 제자리 체크 간격")] public float _StayCheck = 0.3f;
    [Tooltip("플레이어 제자리 체크 거리")] public float _MinRange = 0.1f;
    [Tooltip("플레이어 제자리 체크 거리")] public bool _PredictiveAimOn = true;
    private float stayTimer = 0;
    private bool isHit = false;
    
    [SerializeField] public LayerMask targetLayer;
    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        PlayerRB = Player.GetComponent<Rigidbody>();
        LastTransform = Player.transform.position;
        coroutine = StartCoroutine(PlayerLastTransform());
    }

    private void Update()
    {
        isHit = Physics.CheckSphere(LastTransform, _MinRange, targetLayer);
        if (isHit)
        {
            stayTimer += Time.deltaTime;
        }
        else
        {
            stayTimer = 0f;
        }
    }

    IEnumerator PlayerLastTransform() 
    {
        while (true) 
        {
            LastTransform = Player.transform.position;
            yield return new WaitForSeconds(_CheckSecond);
        }
    }
    public Vector3 PredictiveAimCalc(float ChaseOffset) 
    {
        if (_PredictiveAimOn) 
        {
            Vector3 predictedPosition = Player.transform.position + (PlayerRB.linearVelocity * ChaseOffset);
            if (stayTimer < 0.3f)
                return predictedPosition;
            else
                return Player.transform.position;
        }
        else
            return Player.transform.position;
    }

    private void OnDrawGizmos()
    {
        if (LastTransform == null) return;

        // 실제 로직과 동일한 체크 수행 (디버그용)
        bool isHit = Physics.CheckSphere(LastTransform, _MinRange, targetLayer);

        // 감지되면 초록색, 아니면 빨간색
        Gizmos.color = isHit ? Color.green : Color.red;

        // 원 그리기
        Gizmos.DrawWireSphere(LastTransform, _MinRange);
    }
}