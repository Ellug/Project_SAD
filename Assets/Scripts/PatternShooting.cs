using System.Collections;
using UnityEngine;

public class PatternShooting : PatternBase
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _shootInterval;
    [SerializeField] private int _shootBulletNumber;

    private Transform _targetPosition;
    private WaitForSeconds _delay;

    void Awake()
    {
        _delay = new WaitForSeconds(_shootInterval);
    }

    protected override void PatternLogic()
    {
        StartCoroutine(ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        for (int i = 0; i < _shootBulletNumber; i++)
        {
            Instantiate(_bulletPrefab, transform.position, transform.rotation);
            yield return _delay;
        }
    }
}
