using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetUpState : GuardBaseState
{
    public override int Priority => 85;
    public GetUpState(GuardAI guard) : base(guard) { }
    public override void OnEnter() { } 
    public override void OnUpdate() { } 
    public override void OnExit() { }
}
