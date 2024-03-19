using UnityEngine;

public class Interactable : MonoBehaviour
{
    #region Unity Events

    private void Update()
    {
        Scroll();

        if (transform.position.z <= -2.5f) Destroy(gameObject);
    }

    #endregion

    private void Scroll()
    {
        transform.Translate(BeatController.Instance.CurrentScrollSpeed * Time.deltaTime * -Vector3.forward);
    }
}
