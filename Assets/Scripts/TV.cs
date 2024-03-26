using UnityEngine;
using TMPro;
using System.Collections;

public class TV : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text comboText;

    public void SetMessage(string message)
    {
        text.SetText(message);
    }

    public IEnumerator SetTempMessage(string message1, string message2, float delay)
    {
        SetMessage(message1);
        yield return new WaitForSeconds(delay);
        SetMessage(message2);
    }

    public void SetCombo(int combo)
    {
        comboText.SetText($"x{combo}");
    }

    public void SetComboTextScale(Vector3 scale)
    {
        comboText.transform.localScale = scale;
    }

    public void SetSize(float size)
    {
        text.fontSize = size;
    }
}
