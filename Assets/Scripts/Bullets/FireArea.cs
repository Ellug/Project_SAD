using UnityEngine;
using System.Collections;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FireArea : MonoBehaviour, IPoolable
{
    private PoolMember _poolMember;
    [Tooltip("화염장판 판정 범위")][SerializeField] float _FireAreaRange;
    [Tooltip("화염장판 지속 시간")][SerializeField] float _FireAreaLifeTime;
    [Tooltip("화염장판 데미지")][SerializeField] float _Dmg;
    [Tooltip("데미지 딜레이")][SerializeField] float _DmgDelay;
    [Tooltip("화상 지속시간")][SerializeField] float _BurnDebuffTime;
    [Tooltip("화상 데미지")][SerializeField] float _BurnDmg;
    [Tooltip("화상 틱")][SerializeField] float _TickInterval;
    [Tooltip("타겟 레이어")][SerializeField] LayerMask Layer;
    private ParticleSystem _FireArea;
    private GameObject Player;
    private bool CheckDelay = true;
    private Coroutine DelayCoroutine;

    private void Start()
    {
        Invoke("DespawnFireArea", _FireAreaLifeTime);
    }

    private void Update()
    {
        bool TargetHit = Physics.CheckSphere(transform.position, _FireAreaRange, Layer);
        if (TargetHit) 
        {
            Player.TryGetComponent<PlayerModel>(out var player);
            if (CheckDelay) 
            {
                player.TakeDamage(_Dmg);
                player.BurnDebuff(_BurnDmg, _BurnDebuffTime, _TickInterval);
                CheckDelay = false;
                DelayCoroutine = StartCoroutine(DmgDelayTime());                
            }     
        }
    }
    public void SetTarget(GameObject player) 
    {
        Player = player;
    }

    private void DespawnFireArea() 
    {
        if (DelayCoroutine != null)
            StopCoroutine(DelayCoroutine);
        PoolManager.Instance.Despawn(gameObject);
    }
    IEnumerator DmgDelayTime()
    {
        yield return new WaitForSeconds(_DmgDelay);
        CheckDelay = true;
    }

    //판정 범위 체크용 함수 씬에서 확인할 것
    private void OnDrawGizmos()
    {
        if (transform.position == null) return;

        // 실제 로직과 동일한 체크 수행 (디버그용)
        bool isHit = Physics.CheckSphere(transform.position, _FireAreaRange, Layer);

        // 감지되면 초록색, 아니면 빨간색
        Gizmos.color = isHit ? Color.green : Color.red;

        // 원 그리기
        Gizmos.DrawWireSphere(transform.position, _FireAreaRange);
    }


    public void OnSpawned()
    {
    }

    public void OnDespawned()
    {
    }
}
