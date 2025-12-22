using UnityEngine;

public class BossCannon : BulletBase
{
    [Tooltip("폭발 파티클")] public ParticleSystem ExplosionParticle;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))

            if (ExplosionParticle != null) 
            {
                ExplosionParticle.transform.position = transform.position;
                ExplosionParticle.Play();
            }               

            Despawn();

        if (other.transform.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerModel>(out var player))
                player.TakeDamage(_dmg);

            if (ExplosionParticle != null)
            {
                ExplosionParticle.transform.position = transform.position;
                ExplosionParticle.Play();
            }

            Despawn();
        }
    }
}
