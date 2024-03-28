using UnityEngine;

public class Head : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private FeedbackText feedbackTextPrefab;
    [SerializeField] private Color textColor;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            var contactPoint = other.GetContact(0).point;
            var obstacle = other.transform.GetComponent<Destructible>();
            obstacle.Shatter(obstacle.transform.forward, contactPoint, 5f);

            // Deduct score
            ScoreController.Instance.CurrentScore -= 100;

            foreach (var glove in FindObjectsByType<Glove>(FindObjectsSortMode.None))
            {
                glove.Vibrate(0.25f, 0.25f);
            }

            // Provide feedback text
            var feedbackText = Instantiate(feedbackTextPrefab, contactPoint, Quaternion.identity);
            feedbackText.SetText("-100");
            feedbackText.SetColor(textColor);
        }
    }
}
