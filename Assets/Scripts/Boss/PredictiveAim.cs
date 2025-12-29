using System.Collections;
using UnityEngine;

public class PredictiveAim : MonoBehaviour
{
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    [Tooltip("플레이어 리짓 바디")][SerializeField] private Rigidbody PlayerRB;
    [Tooltip("마지막 플레이어 위치 갱신 간격")] public float _CheckSecond = 0.5f;
    [Tooltip("플레이어 제자리 체크 간격")] public float _StayCheck = 0.3f;
    [Tooltip("플레이어 제자리 체크 거리")] public float _MinRange = 0.1f;
    [Tooltip("플레이어 예고 장판 여부")] public bool _PredictiveAimOn = true;
    
    private Coroutine _coroutine;
    private Vector3 _lastTransform;
    private float _stayTimer = 0;
    private bool _isHit = false;
    
    [SerializeField] public LayerMask targetLayer;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerRB = Player.GetComponent<Rigidbody>();
        _lastTransform = Player.transform.position;
        _coroutine = StartCoroutine(PlayerLastTransform());
    }

    private void Update()
    {
        _isHit = Physics.CheckSphere(_lastTransform, _MinRange, targetLayer);
        if (_isHit)
        {
            _stayTimer += Time.deltaTime;
        }
        else
        {
            _stayTimer = 0f;
        }
    }

    private void OnDestroy() 
    {
        StopCoroutine(_coroutine);
    }

    IEnumerator PlayerLastTransform() 
    {
        while (true) 
        {
            _lastTransform = Player.transform.position;
            yield return new WaitForSeconds(_CheckSecond);
        }
    }
    public Vector3 PredictiveAimCalc(float ChaseOffset) 
    {
        if (_PredictiveAimOn) 
        {
            Vector3 predictedPosition = Player.transform.position + (PlayerRB.linearVelocity * ChaseOffset);
            if (_stayTimer < 0.3f)
                return predictedPosition;
            else
                return Player.transform.position;
        }
        else
            return Player.transform.position;
    }
}