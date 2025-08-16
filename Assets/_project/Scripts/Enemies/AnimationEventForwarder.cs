using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    [SerializeField] private GuardAI _guardAI;
    private void Awake()
    {
        if (_guardAI == null)
        {
            _guardAI = GetComponentInParent<GuardAI>();
        }
    }

    public void DealDamage()
    {
        if (_guardAI != null)
        {
            _guardAI.DealDamage();
        }
    }
}
