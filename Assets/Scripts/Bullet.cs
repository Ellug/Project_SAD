using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 16f;
    public float lifeTime = 3f;
    public int damage = 10;

    Vector3 _direction;
    float _timer;

    void OnTriggerEnter(Collider other)
    {
        // 일단은 뭔가에 닿기만 하면 탄환이 사라지게
        Destroy(gameObject);
    }


    public void Init(Vector3 direction, int damage)
    { 
      _direction = direction.normalized;
        this.damage = damage;
        _timer = 0f;

    }    
    void Start()
    {
        if (_direction == Vector3.zero)      
            _direction = transform.forward;
        
    }

    
    void Update()
    {
        transform.position += _direction * speed * Time.deltaTime;
        

        _timer += Time.deltaTime;
        if (_timer >= lifeTime)
        {
            Destroy(gameObject);
        }



    }
    
}
