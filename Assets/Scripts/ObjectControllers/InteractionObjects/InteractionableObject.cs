using UnityEngine;

// 상호작용 가능한 오브젝트들은 모두 이 클래스를 상속받는다.
public abstract class InteractionableObject : MonoBehaviour
{
    [SerializeField] protected GameObject _interactionKey;

    private Camera _camera;
    private RectTransform _keyTransform;
    private Vector3 _keyPosOffset;

    private void Awake()
    {
        _camera = Camera.main;
        _keyPosOffset = new Vector3(75f, 50f, 0f);
    }

    // 감지 범위 내에 들어오면 상호작용 키를 활성화 한다.
    protected virtual void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag("Player") )
        {
            other.gameObject.GetComponent<PlayerController>()._interactionObject += OnInteract;
            _interactionKey.SetActive(true);
            _keyTransform = _interactionKey.GetComponent<RectTransform>();
            
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (_interactionKey.activeSelf)
        {
            _keyTransform.position = _camera.WorldToScreenPoint(transform.position) + _keyPosOffset;
        }
    }

    // 감지 범위에서 나가면 상호작용 관련 요소를 모두 OFF 한다.
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>()._interactionObject -= OnInteract;
            _interactionKey.SetActive(false);
        }
    }

    public abstract void OnInteract();
}
