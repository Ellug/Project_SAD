using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private int _bossHp = 100;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 총알에 태그 별도로 할당 바랍니다.
        if (other.CompareTag("PlayerBullet"))
        {
            // TakeDamage(총알 공격력);
        }
    }

    public void TakeDamage(int dmg)
    {
        _bossHp -= dmg;
    }
}
