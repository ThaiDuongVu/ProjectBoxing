using UnityEngine;

public class Head : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            var obstacle = other.transform.GetComponent<Destructible>();
            obstacle.Shatter(obstacle.transform.forward, other.GetContact(0).point, 5f);

            foreach (var glove in FindObjectsByType<Glove>(FindObjectsSortMode.None))
            {
                glove.Vibrate(0.25f, 0.25f);
            }
        }
    }
}
