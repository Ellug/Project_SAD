using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PredictiveAim : MonoBehaviour
{
    private Coroutine coroutine;
    private Vector3 LastTransform;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    [Tooltip("플레이어 리짓 바디")][SerializeField] private Rigidbody PlayerRB;
    [Tooltip("마지막 플레이어 위치 갱신 간격")] public float _CheckSecond = 0.5f;
    [Tooltip("플레이어 제자리 체크 간격")] public float _StayCheck = 0.3f;
    [Tooltip("플레이어 제자리 체크 거리")] public float _MinRange = 0.1f;
    [Tooltip("플레이어 예고 장판 여부")] public bool _PredictiveAimOn = true;
    private float stayTimer = 0;
    private bool isHit = false;
    
    [SerializeField] public LayerMask targetLayer;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
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
}