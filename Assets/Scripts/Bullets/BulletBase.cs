using UnityEngine;

public abstract class BulletBase : MonoBehaviour, IPoolable
{
     [SerializeField] protected float _speed = 20f;
     [SerializeField] protected float _maxDistance = 50f; // 0 하면 사거리 무한
     [SerializeField] protected float _dmg = 10f;

     private Vector3 _spawnPos;
     private PoolMember _poolMember;

     protected virtual void Awake()
     {
          _poolMember = GetComponent<PoolMember>();
     }

     public virtual void Init(float dmg, float speed, float maxDistance)
     {
          _dmg = dmg;
          _speed = speed;
          _maxDistance = maxDistance;

          _spawnPos = transform.position;
     }

     protected virtual void Update()
     {
          CheckDistance();
     }

     protected virtual void FixedUpdate()
     {
          MoveForward();          
     }

     // 정면 방향으로 전진
     protected virtual void MoveForward()
     {
          transform.Translate(Vector3.forward * (_speed * Time.fixedDeltaTime), Space.Self);
     }

     // 최대 사거리 체크 후 디스폰
     private void CheckDistance()
     {
          if (_maxDistance > 0f)
          {
               float sqr = (transform.position - _spawnPos).sqrMagnitude;
               float maxSqr = _maxDistance * _maxDistance;

               if (sqr >= maxSqr)
               {
                    Despawn();
                    return;
               }
          }
     }

     protected void Despawn()
     {
          if (_poolMember == null)
               _poolMember = GetComponent<PoolMember>();

          if (_poolMember != null)
               _poolMember.Despawn();
          else
               Destroy(gameObject);
     }

     // 특수탄환의 스폰, 디스폰시 추가 로직 세팅 용
     public virtual void OnSpawned()
     {
          _spawnPos = transform.position;
     }

     public virtual void OnDespawned() {}
}
