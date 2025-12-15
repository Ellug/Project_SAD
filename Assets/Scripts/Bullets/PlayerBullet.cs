using UnityEngine;

public class PlayerBullet : BulletBase
{
    [SerializeField] private bool _counterAttack;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private float _attackPower = 10f;
    [SerializeField] private float _bulletRange = 10f;

    private Transform _createdPos;

    // TODO : 총알이 가져야할 정보는 무기로부터 오며, 이 Init 메서드를 통해서 할당
    public void Init()
    {
        _createdPos = transform;
    }

    void Update()
    {
        // 전진하다가 이동한 거리가 bulletRange를 넘어서면 파괴.
        transform.Translate(_bulletSpeed * Time.deltaTime * Vector3.forward);
        if (Vector3.Distance(_createdPos.position, transform.position) > _bulletRange)
        {
            Destroy(gameObject);
        }
    }
}
