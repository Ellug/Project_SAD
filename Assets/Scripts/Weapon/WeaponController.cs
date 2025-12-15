using UnityEngine;

public class WeaponController : MonoBehaviour , IWeapon
{
    private WeaponData _weaponData;
    //특전까지 가져와야함
    //private WeaponPeck _weaponPeck;
    //불렛도 가져와야함(일단은 풀링 신경 x)
    //private PlayerBullet _playerBullet;

    public void Init(WeaponData weaponData)//, WeaponPeck weaponPeck, PlayerBullet playerBullet)
    {
        _weaponData = weaponData;
        //_weaponPeck = weaponPeck;
        //_playerBullet = playerBullet;
    }
    
    public void Attack()
    {
        //bullet에 공격력 주입
        //bullet에 이동속도 주입

        //여기서 공격 텀 주입
        //여기서 발사되는 갯수 주입(프리팹 만드는 수)
        //여기서 1회 발사될 때, 1개 초과면 Angle에 맞춰 이쁜각도로 나오게 값 대입
        //여기서 발사할 bullet 프리팹 지정
    }

    public float Damage()
    {
        return _weaponData.attack;
    }

    public void SpecialAttack()
    {
        //bullet에 공격력 주입
        //여기서 공격 텀 주입
        //여기서 격발 전까지의 시간딜레이 주입
        //여기서 발사되는 갯수 주입(프리팹 만드는 수)
        //여기서 1회 발사될 때, 1개 초과면 Angle에 맞춰 이쁜각도로 나오게 값 대입
        //bullet에 이동속도 주입
        //여기서 발사할 bullet 프리팹 지정
    }

    //불릿 정보 줄거.(구조체 활용해서 진행)
    private void Fireprojectile() 
    {
        Instantiate(_weaponData.projectilePrefab, transform.position, transform.rotation);
    }
}
