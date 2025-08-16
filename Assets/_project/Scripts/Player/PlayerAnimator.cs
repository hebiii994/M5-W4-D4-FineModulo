using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _animationSmoothFactor = 10f;
    [SerializeField] private PlayerController _playerController;

    void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        if (_playerController == null)
        {
            _playerController = GetComponent<PlayerController>();
        }

    }

    void Update()
    {
        float targetSpeed = _agent.velocity.magnitude;

        float currentAnimatorSpeed = _animator.GetFloat("Speed");
        float smoothedSpeed = Mathf.Lerp(currentAnimatorSpeed, targetSpeed, Time.deltaTime * _animationSmoothFactor);


        _animator.SetFloat("Speed", smoothedSpeed);

        bool isAgainstWall = _playerController.CurrentState is WallPressState;
        _animator.SetBool("isAgainstWall", isAgainstWall);

        if (isAgainstWall)
        {
            float wallMoveDirection = Input.GetAxis("Horizontal");
            _animator.SetFloat("WallMoveDirection", wallMoveDirection);
        }
        else
        {
            _animator.SetFloat("WallMoveDirection", 0f);
        }
    }
}
