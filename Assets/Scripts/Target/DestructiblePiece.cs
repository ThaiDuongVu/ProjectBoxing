using UnityEngine;

public class DestructiblePiece : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Collider _collider;

    #region Unity Events

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    #endregion

    public void SetKinematic(bool value)
    {
        _rigidbody.isKinematic = value;
    }

    public void SetColliderEnabled(bool value)
    {
        _collider.enabled = value;
    }

    public void AddForce(Vector3 direction, float force)
    {
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }

    public void AddTorque(Vector3 direction, float force)
    {
        _rigidbody.AddTorque(direction * force, ForceMode.Impulse);
    }

    public void SelfDestruct(float delay)
    {
        Destroy(gameObject, delay);
    }
}
