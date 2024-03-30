using System.Collections;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [SerializeField] private Shield shield;

    private Transform _cameraTransform;
    private Glove _leftGlove;
    private Glove _rightGlove;

    #region Unity Events

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        var allGloves = FindObjectsByType<Glove>(FindObjectsSortMode.InstanceID);
        _leftGlove = allGloves[0];
        _rightGlove = allGloves[1];
    }

    private void Update()
    {
        if (IsBlockStance())
        {
            shield.gameObject.SetActive(true);

            var glovesAvgPosition = (_leftGlove.transform.position + _rightGlove.transform.position) / 2;
            shield.transform.position = glovesAvgPosition + _cameraTransform.forward * 0.25f;
            transform.forward = _cameraTransform.forward;
        }
        else
        {
            shield.gameObject.SetActive(false);
        }
    }

    #endregion

    // Check if glove positions/orientation is in block stance
    private bool IsBlockStance()
    {
        if (!_leftGlove || !_rightGlove) return false;

        // Check if gloves are closed
        if (!_leftGlove.IsClosed || !_rightGlove.IsClosed) return false;

        // Check gloves orientation
        if ((_leftGlove.transform.forward - _cameraTransform.up).magnitude > 0.4f) return false;
        if ((_rightGlove.transform.forward - _cameraTransform.up).magnitude > 0.4f) return false;
        if ((_leftGlove.transform.up + _cameraTransform.right).magnitude > 0.4f) return false;
        if ((_rightGlove.transform.up - _cameraTransform.right).magnitude > 0.4f) return false;

        // Check if gloves are close together
        if ((_leftGlove.transform.position - _rightGlove.transform.position).magnitude > 0.125f) return false;

        return true;
    }
}
