using UnityEngine;

public class BossBullet : BulletBase
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
            Despawn();

        if (other.transform.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerModel>(out var player))
                player.TakeDamage(_dmg);

            Despawn();
        }
    }
}
