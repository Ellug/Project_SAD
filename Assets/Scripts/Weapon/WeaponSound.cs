using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    private WeaponBase _weapon;

    public void Bind(WeaponBase weapon)
    {
        if (_weapon != null)
        {
            _weapon.OnFire -= PlayFire;
            _weapon.OnReload -= PlayReload;
            _weapon.OnSPFire -= PlaySPFire;
        }

        _weapon = weapon;

        _weapon.OnFire += PlayFire;
        _weapon.OnReload += PlayReload;
        _weapon.OnSPFire += PlaySPFire;
    }

    void PlayFire(WeaponRuntimeStats stats)
    {
        audioSource.PlayOneShot(stats.FireClip, 0.5f);
    }

    void PlayReload(WeaponRuntimeStats stats)
    {
        audioSource.PlayOneShot(stats.ReloadClip);
    }
    void PlaySPFire(WeaponRuntimeStats stats)
    {
        audioSource.PlayOneShot(stats.SPFireClip);
    }
}
