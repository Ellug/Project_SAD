using DamageNumbersPro;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    [Header("건드리지 말 것")]
    [SerializeField] private DamageNumber _dmgFont;

    public void TakeDamage(float dmg, bool isCounterable)
    {
        _dmgFont.Spawn(transform.position, dmg);
    }
}
