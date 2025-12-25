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
        }

        _weapon = weapon;

        _weapon.OnFire += PlayFire;
        _weapon.OnReload += PlayReload;
    }
    public void UnBind(WeaponBase weapon)
    {
        weapon.OnFire -= PlayFire;
        weapon.OnReload -= PlayReload;
    }

    void PlayFire(WeaponRuntimeStats stats)
    {
        audioSource.PlayOneShot(stats.FireClip);
    }

    void PlayReload(WeaponRuntimeStats stats)
    {
        audioSource.PlayOneShot(stats.ReloadClip);
    }
}
