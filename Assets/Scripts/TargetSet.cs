using UnityEngine;

public class TargetSet : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;

    #region Unity Events

    private void Update()
    {
        Scroll();

        if (transform.position.z <= -2f) Destroy(gameObject);
    }

    #endregion

    private void Scroll()
    {
        transform.Translate(-Vector3.forward * scrollSpeed * Time.deltaTime);
    }
}
