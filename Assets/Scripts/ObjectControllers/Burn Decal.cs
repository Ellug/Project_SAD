using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BurnDecal : MonoBehaviour, IPoolable
{
    private DecalProjector burnDecal;
    private PoolMember _poolMember;

    [Tooltip("그을음 지속 시간")] public float _fadeTime = 1f;

    private float Timer;

    void Awake()
    {
        burnDecal = GetComponent<DecalProjector>();
        _poolMember = GetComponent<PoolMember>();
    }

    void Update()
    {
        if (Timer < _fadeTime)
        {
            Timer += Time.deltaTime;
            float normalizedTime = Timer / _fadeTime;
            burnDecal.fadeFactor = Mathf.Clamp01(1f - normalizedTime);
        }
    }

    private void FadeDecal() 
    {
        PoolManager.Instance.Despawn(gameObject);
    }

    public void OnSpawned()
    {
        Timer = 0f;

        if (burnDecal != null)
        {
            burnDecal.fadeFactor = 1f;
        }

        Invoke("FadeDecal", _fadeTime);
    }

    public void OnDespawned()
    {
    }
}
