using UnityEngine;

public enum ShotgunPerkMode
{
    None,
    Slug,
    Tripple
}

public class Shotgun : WeaponBase
{
    private int _dynOwnerId;

    private ShotgunPerkMode CurrentMode
    {
        get
        {
            if (_statsContext == null) return ShotgunPerkMode.None;
            return (ShotgunPerkMode)_statsContext.Current.Weapon.ShotgunMode; // 0 1 2
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _dynOwnerId = GetInstanceID(); // int 키
    }

    private void OnDisable()
    {
        Clear();
    }

    // 공격 시도
    public override bool TryAttack()
    {
        var mode = CurrentMode;

        switch (mode)
        {
            case ShotgunPerkMode.Slug:
                FireSlug();
                break;

            case ShotgunPerkMode.Tripple:
                FireTriple();
                break;

            default:
                FireDefault();
                break;
        }

        _statsContext?.NotifyAttackSlow();
        return true;
    }

    private void FireDefault()
    {
        // 기본 산탄 발사
        FireProjectile(false);
    }

    private void FireSlug()
    {
        // 탄퍼짐 없는 슬러그턴 발사
        // 노드에서 사거리 증가, 탄퍼짐 감소 옵션 적용
        FireProjectile(false);
    }

    private void FireTriple()
    {
        // 탄환 갯수2개 추가
        // TODO: 화상 피해 탄환 추가 필요
        FireProjectile(false);
    }

    private void Clear()
    {
        if (_statsContext != null)
            _statsContext.SetDynamicMods(_dynOwnerId, null);
    }
}
