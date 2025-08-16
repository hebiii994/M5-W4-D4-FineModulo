using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float _comboResetTime = 1.0f;
    [SerializeField] private Transform _hitboxOrigin;
    [SerializeField] private float _hitboxRadius = 0.8f;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _noisePrefab;
    [SerializeField] private int _attackDamage = 15;

    private PlayerController _playerController;
    public int ComboStep { get; set; } = 0;
    public float LastAttackTime { get; set; } = -99f;

    public float ComboResetTime => _comboResetTime;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void CheckForHit()
    {
        Debug.Log("Checking for hit!");
        Collider[] hits = Physics.OverlapSphere(_hitboxOrigin.position, _hitboxRadius, _enemyLayer);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out GuardAI guard))
            {
                int currentComboStep = GetComponent<Animator>().GetInteger("AttackCombo");
                Debug.Log("<color=cyan>PLAYER DEBUG: Sto per colpire con comboStep = " + currentComboStep + "</color>");
                guard.GetHit(currentComboStep, _attackDamage);
                return;
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
}