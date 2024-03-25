using UnityEngine;
using TMPro;

public class FeedbackText : MonoBehaviour
{
    private TMP_Text _text;

    #region Unity Events

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Destroy(gameObject, 1f);
    }

    #endregion

    public void SetMessage(string message)
    {
        _text.SetText(message);
    }

    public void SetColor(Color color)
    {
        _text.color = color;
    }

    public void SetSize(float size)
    {
        _text.fontSize = size;
    }
}
