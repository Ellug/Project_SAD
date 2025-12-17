using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BurnDecalFade : MonoBehaviour
{
    [Tooltip("그을음 활성화 시간")] public float lifeTime = 1.5f;
    [Tooltip("그을음 제거 시간")] public float fadeDuration = 0.8f;

    DecalProjector decal;
    Material decalMat;
    float timer;

    void Awake()
    {
        decal = GetComponent<DecalProjector>();
        decalMat = Instantiate(decal.material);
        decal.material = decalMat;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifeTime)
        {
            float t = (timer - lifeTime) / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            decalMat.SetFloat("_BaseColorAlpha", alpha);

            if (t >= 1f)
                Destroy(gameObject);
        }
    }
}