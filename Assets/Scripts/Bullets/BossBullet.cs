using UnityEngine;

public class BossBullet : BulletBase
{
    [Header("탄환 속성")]
    [Tooltip("충돌 시 파괴 여부")] 
    // 장애물을 그냥 통과하는걸 의미하는지? 플레이어가 맞아도 사라지지 않는다는 것인지?
    // 장애물 통과라면 유저가 벽 뒤에 숨어있어도 갑자기 튀어나온 탄에 맞을텐데 의도된건지?
    // 플레이어가 맞아도 사라지지 않는다면 플레이어 캐릭터는 1명인데 통과하는게 어떤 의미인지?
    // 우선 관통 여부는 고려하지 마시오.
    // [SerializeField] private bool _isPierce = false;
    //[Tooltip("유도탄 여부 (미구현)")]
    //[SerializeField] private bool _isTracking = false;
    //[SerializeField] private float _startSpeed = 3f;
    // 일직선 탄환형, 유도형 미사일형으로 나눠지고, 일직선 탄환은 일정속도로 날라감
    // 유도형 미사일은 플레이어 방향으로 곡선형으로 날아가며 가속을 받음. 가속도 필드는 이 때 쓰임.
    // 유도형 미사일은 별도의 패턴 클래스로 분리하고 여긴 일직선 탄환 패턴만 고려함.
    // [SerializeField] private float _acceleration = 1f;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _damage = 10f;
    // 넉백에 대한 내용도 좀 더 상세한 설명 필요.
    // [SerializeField] private float _knockbackPower = 5f;

    void Awake()
    {
        Destroy(gameObject, _lifeTime);
    }

    void Update()
    {
        transform.Translate(_bulletSpeed * Time.deltaTime * Vector3.forward);
    }

    void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Obstacle"))
        // {
        //     Destroy(gameObject);
        // }

        if (other.transform.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerModel>(out var player))
                player.TakeDamage(_damage);

            Destroy(gameObject);
        }
    }
}
