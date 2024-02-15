using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    public ControllerType controllerType;

    [Header("References")]
    [SerializeField] private Transform rig;
    [SerializeField] private ParticleSystem explosionPrefab;

    private TargetPiece[] _pieces;
    public bool IsShattered { get; set; }

    #region Unity Events

    private void Awake()
    {
        _pieces = rig.GetComponentsInChildren<TargetPiece>();
    }

    #endregion

    public void Shatter(Vector3 direction, Vector3 impactPoint, float force)
    {
        foreach (var piece in _pieces)
        {
            var impactDirection = (piece.transform.position - impactPoint).normalized;
            var shatterDirection = (direction + impactDirection).normalized;

            piece.transform.SetParent(null, true);
            piece.SetKinematic(false);
            piece.SetColliderEnabled(true);
            piece.AddForce(shatterDirection, force);
            piece.AddTorque(shatterDirection, force);
            piece.SelfDestruct(3f);
        }

        Instantiate(explosionPrefab, impactPoint, Quaternion.identity);
        IsShattered = true;
    }
}
