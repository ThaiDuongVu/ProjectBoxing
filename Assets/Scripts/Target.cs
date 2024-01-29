using UnityEngine;

public class Target : MonoBehaviour
{
    public ControllerType controllerType;

    [Header("References")]
    [SerializeField] private Transform rig;
    [SerializeField] private ParticleSystem explosionPrefab;

    private TargetPiece[] _pieces;
    private const float ShatterForce = 4f;

    #region Unity Events

    private void Awake()
    {
        _pieces = rig.GetComponentsInChildren<TargetPiece>();
    }

    private void Start()
    {

    }

    #endregion

    public void Shatter(Vector3 direction, Vector3 impactPoint)
    {
        foreach (var piece in _pieces)
        {
            var impactDirection = (piece.transform.position - impactPoint).normalized;
            var shatterDirection = (direction + impactDirection).normalized;

            piece.transform.SetParent(null);
            piece.SetKinematic(false);
            piece.SetColliderEnabled(true);
            piece.AddForce(shatterDirection, ShatterForce);
            piece.AddTorque(shatterDirection, ShatterForce);
            piece.SelfDestruct(3f);
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}