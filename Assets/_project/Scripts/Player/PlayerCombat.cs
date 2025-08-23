using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configurazione")]
    [SerializeField] private Transform _hitboxOrigin;
    [SerializeField] private float _hitboxRadius = 0.8f;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private int _attackDamage = 15;
    [SerializeField] private GameObject _noisePrefab;
    [SerializeField] private float _comboResetTime = 1.0f;

    private PlayerController _playerController;
    private Animator _animator; 

    public int ComboStep { get; set; } = 0;
    public float LastAttackTime { get; set; } = -99f;
    public float ComboResetTime => _comboResetTime;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
    }

    public void CheckForHit()
    {
        if (_hitboxOrigin == null)
        {
            Debug.LogError("PlayerCombat: _hitboxOrigin non assegnato!", this.gameObject);
            return;
        }

        Collider[] hits = Physics.OverlapSphere(_hitboxOrigin.position, _hitboxRadius, _enemyLayer, QueryTriggerInteraction.Collide);

        if (hits.Length > 0)
        {
            Debug.Log($"<color=green>CheckForHit: Rilevati {hits.Length} nemici!</color>");
        }

        foreach (Collider hit in hits)
        {
            GuardAI guard = hit.GetComponentInParent<GuardAI>();
            if (guard != null)
            {
                int currentComboStep = _animator.GetInteger("AttackCombo");
                Debug.Log($"<color=cyan>PLAYER DEBUG: Colpendo '{guard.name}' con comboStep = {currentComboStep}</color>");
                guard.GetHit(currentComboStep, _attackDamage);
            }
        }
    }

    public void KnockOnWall()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 2f))
        {
            Instantiate(_noisePrefab, hit.point, Quaternion.identity);
            _playerController.Animator.SetTrigger("Knock");
            Debug.Log("Knock sul muro");
        }
    }

    public void OnAttackAnimationEnd()
    {
        _playerController.OnAttackFinished();
    }

    private void OnDrawGizmosSelected()
    {
        if (_hitboxOrigin == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_hitboxOrigin.position, _hitboxRadius);
    }
}