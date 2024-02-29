using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void Enter();
    public abstract void Tick();
    public abstract void TickFixed();
    public abstract void Exit();
    public abstract void EvaluateCollision(Collision collision);
    public abstract void EvaluateSubmergence(Collider other);
}
