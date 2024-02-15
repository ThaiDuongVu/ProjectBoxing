using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;

    #region Unity Events

    private void Update()
    {
        Scroll();

        if (transform.position.z <= -2.5f) Destroy(gameObject);
    }

    #endregion

    private void Scroll()
    {
        transform.Translate(scrollSpeed * Time.deltaTime * -Vector3.forward);
    }
}
