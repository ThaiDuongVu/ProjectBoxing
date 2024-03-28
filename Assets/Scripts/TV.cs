using UnityEngine;
using TMPro;
using System.Collections;

public class TV : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text scoreText;

    public void SetText(string message)
    {
        text.SetText(message);
    }

    public IEnumerator SetTempMessage(string message1, string message2, float delay)
    {
        SetText(message1);
        yield return new WaitForSeconds(delay);
        SetText(message2);
    }

    public void SetComboText(int combo)
    {
        comboText.SetText($"x{combo}");
    }

    public void SetComboTextScale(Vector3 scale)
    {
        comboText.transform.localScale = scale;
    }

    public void SetScoreText(int score)
    {
        scoreText.SetText($"{score}");
    }
}
