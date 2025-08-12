using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _animationSmoothFactor = 10f;

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

    }

    void Update()
    {
        float targetSpeed = _agent.velocity.magnitude;

        float currentAnimatorSpeed = _animator.GetFloat("Speed");
        float smoothedSpeed = Mathf.Lerp(currentAnimatorSpeed, targetSpeed, Time.deltaTime * _animationSmoothFactor);


        _animator.SetFloat("Speed", smoothedSpeed);
    }
}
