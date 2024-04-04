using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ColliderState
{
    Closed,
    Open,
    Colliding
}

public interface IHitboxResponder
{
    void CollidedWith(Collider collider);
}
public class HitBox : MonoBehaviour
{
    public LayerMask mask;
    public bool useSphere = false;
    public Vector3 hitboxSize = Vector3.one;
    public float radius = .5f;
    public Color inactiveColor;
    public Color collisionOpenColor;
    public Color collidingColor;

    private ColliderState _state= ColliderState.Closed;
    private IHitboxResponder _responder = null;
    private List<Collider> detectedColliders = new();

    public void HitboxUpdate()
    {
        Debug.Log("hitbox update");
        if(_state == ColliderState.Closed)
        {
            return;
        }
        Debug.Log("hitbox checking colliders");
        Collider[] colliders = useSphere ?  Physics.OverlapSphere(transform.position, radius) : Physics.OverlapBox(transform.position, hitboxSize, transform.rotation, mask);

        for(int i = 0; i < colliders.Length; i++)
        {
            Debug.Log("there is a collider");
            Collider collider = colliders[i];
            if (!detectedColliders.Contains(collider))
            {
                detectedColliders.Add(collider);
                _responder.CollidedWith(collider);
            }
        }

        _state = colliders.Length > 0 ? ColliderState.Colliding : ColliderState.Open;
    }

    public void UseResponder(IHitboxResponder responder)
    {
        _responder = responder;
    }

    private void OnDrawGizmos()
    {
        //Debug.Log("drawing gizmos");
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        if (useSphere) 
        {
            //Debug.Log("draw sphere");
            Gizmos.DrawWireSphere(Vector3.zero, radius);
        }
        else 
        {
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(hitboxSize.x * 2, hitboxSize.y * 2, hitboxSize.z * 2)); // Because size is halfExtents
        }
    }

    private Color CheckGizmoColor()
    {
        switch (_state)
        {
            case ColliderState.Closed:
                return inactiveColor;
            case ColliderState.Open:
                return collisionOpenColor;
            case ColliderState.Colliding:
                return collidingColor;
        }
        return Color.white;
    }

    public void StartCheckingCollision()
    {
        _state = ColliderState.Open;
    }

    public void StopCheckingCollision()
    {
        detectedColliders.Clear();
        _state = ColliderState.Closed;
    }
}
