using UnityEngine;

public class BossBullet : BulletBase
{
    [Header("탄환 속성")]
    [Tooltip("충돌 시 파괴 여부")] 
    // 장애물을 그냥 통과하는걸 의미하는지? 플레이어가 맞아도 사라지지 않는다는 것인지?
    // 장애물 통과라면 유저가 벽 뒤에 숨어있어도 갑자기 튀어나온 탄에 맞을텐데 의도된건지?
    // 플레이어가 맞아도 사라지지 않는다면 플레이어 캐릭터는 1명인데 통과하는게 어떤 의미인지?
    [SerializeField] private bool _isPierce = false;
    [Tooltip("유도탄 여부 (미구현)")]
    [SerializeField] private bool _isTracking = false;
    [SerializeField] private float _startSpeed = 3f;
    // 가속도가 있다는건 모든 총알이 일정한 속도로 날아가는게 아니라 점점 빨라지는 방식인지?
    [SerializeField] private float _acceleration = 1f;
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _knockbackPower = 5f;

    private float _currentSpeed;

    void Awake()
    {
        _currentSpeed = _startSpeed;
        Destroy(gameObject, _lifeTime);
    }

    void Update()
    {
        // 가속도에 델타 타임을 곱하여 현재 속도를 증가 시킴
        if (_currentSpeed < _maxSpeed)
        {
            _currentSpeed += _acceleration * Time.deltaTime;
        }
        transform.Translate(_currentSpeed * Time.deltaTime * Vector3.forward);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
