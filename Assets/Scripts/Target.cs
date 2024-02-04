using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    public ControllerType controllerType;

    [Header("References")]
    [SerializeField] private Transform rig;
    [SerializeField] private ParticleSystem explosionPrefab;

    [SerializeField] private FeedbackText feedbackTextPrefab;
    [SerializeField] private Color textColor;

    private TargetPiece[] _pieces;
    private const float ShatterForce = 4f;

    #region Unity Events

    private void Awake()
    {
        _pieces = rig.GetComponentsInChildren<TargetPiece>();
    }

    // private IEnumerator Start()
    // {
    //     yield return new WaitForSeconds(5f);
    //     Shatter(Vector3.forward, transform.position);
    //     Destroy(transform.parent.gameObject);
    // }

    #endregion

    public void Shatter(Vector3 direction, Vector3 impactPoint)
    {
        foreach (var piece in _pieces)
        {
            var impactDirection = (piece.transform.position - impactPoint).normalized;
            var shatterDirection = (direction + impactDirection).normalized;

            piece.transform.SetParent(null, true);
            piece.SetKinematic(false);
            piece.SetColliderEnabled(true);
            piece.AddForce(shatterDirection, ShatterForce);
            piece.AddTorque(shatterDirection, ShatterForce);
            piece.SelfDestruct(1.5f);
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        var feedbackText = Instantiate(feedbackTextPrefab, transform.position, Quaternion.identity);
        feedbackText.SetColor(textColor);
    }
}
