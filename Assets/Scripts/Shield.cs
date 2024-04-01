using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private FeedbackText feedbackTextPrefab;
    [SerializeField] private Color textColor;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Blockable"))
        {
            var contactPoint = other.GetContact(0).point;
            var blockable = other.transform.GetComponent<Destructible>();

            blockable.Shatter(transform.forward, contactPoint, 5f);
            foreach (var glove in FindObjectsByType<Glove>(FindObjectsSortMode.None)) glove.Vibrate(0.25f, 0.25f);

            ComboController.Instance.AddCombo();
            var addedScore = 100 * ComboController.Instance.CurrentCombo;
            ScoreController.Instance.CurrentScore += addedScore;

            // Provide feedback text
            var feedbackText = Instantiate(feedbackTextPrefab, contactPoint, Quaternion.identity);
            feedbackText.SetText($"+{addedScore}");
            feedbackText.SetColor(textColor);
        }
    }
}
