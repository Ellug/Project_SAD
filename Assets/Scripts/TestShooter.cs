using UnityEngine;

public class TestShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float interval = 0.5f;
    public int damage = 10;

    float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= interval)
        {
            _timer = 0f;
            GameObject go = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Bullet bullet = go.GetComponent<Bullet>();
            bullet.Init(transform.forward, damage);
        }
    }
}

