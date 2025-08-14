using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _noisePrefab;


    [SerializeField] private float _comboResetTime = 1.0f;

    private int _comboStep = 0;
    private bool _isAttacking = false;
    public bool IsAttacking => _isAttacking;
    private bool _queuedAttack = false;
    private float _lastAttackTime = -99f;

    //hitbox variables
    [SerializeField] private Transform _hitboxOrigin;
    [SerializeField] private float _hitboxRadius = 0.8f;
    [SerializeField] private LayerMask _enemyLayer;

    void Update()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            _isAttacking = false;
        }

        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1))
        {
            if (_isAttacking)
            {
                _queuedAttack = true;
            }
            else
            {
                Attack();
            }
        }

    }

    private void Attack()
    {
        float timeSinceLast = Time.time - _lastAttackTime;

        if (timeSinceLast > _comboResetTime)
        {
            _comboStep = 1;
        }
        else
        {
            _comboStep++;
            if (_comboStep > 3) _comboStep = 1;
        }

        _isAttacking = true;
        _queuedAttack = false;

        _playerController.StopMovement();
        _playerController.CanMove = false;

        _animator.SetInteger("AttackCombo", _comboStep);
        _animator.SetTrigger("Attack");

        Debug.Log($"Attacco partito. Step: {_comboStep} (tempo dall'ultimo: {timeSinceLast:F2}s)");
    }

    public void OnAttackEnd()
    {
        _isAttacking = false;
        _lastAttackTime = Time.time;

        if (_queuedAttack)
        {
            Attack();
        }
        else
        {
            _playerController.CanMove = true;
        }
    }

    public void AllowMovementMidAttack()
    {
        if (_isAttacking)
            _playerController.CanMove = true;
    }
    public void CheckForHit()
    {
        Debug.Log("Checking for hit!");
        Collider[] hits = Physics.OverlapSphere(_hitboxOrigin.position, _hitboxRadius, _enemyLayer);

        foreach (Collider hit in hits)
        {

            if (hit.TryGetComponent(out GuardAI guard))
            {
                guard.LastKnownPlayerPosition = transform.position;
                guard.GetHit(_comboStep);
                return;
            }
        }
    }

    public void KnockOnWall()
    {

        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 2f))
        {
            Instantiate(_noisePrefab, hit.point, Quaternion.identity);

            _animator.SetTrigger("Knock");
            Debug.Log("Knock sul muro");
        }
    }

}