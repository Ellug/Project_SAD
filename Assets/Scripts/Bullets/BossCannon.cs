using Unity.VisualScripting;
using UnityEngine;

public class BossCannon : BulletBase
{
    [Tooltip("폭발 파티클")] public ParticleSystem _ExplosionParticle;
    private ParticleSystem Explosion;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))

            if (_ExplosionParticle != null) 
            {
                Explosion = Instantiate(_ExplosionParticle, transform.position, transform.rotation);
                var main = Explosion.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                Explosion.Play();
            }               

            Despawn();

        if (other.transform.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerModel>(out var player))
                player.TakeDamage(_dmg);

            if (_ExplosionParticle != null)
            {
                Explosion = Instantiate(_ExplosionParticle, transform.position, transform.rotation);
                var main = Explosion.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                Explosion.Play();
            }

            Despawn();
        }
    }
}
