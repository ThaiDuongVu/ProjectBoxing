using UnityEngine;
using TMPro;

public class TV : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetMessage(string message)
    {
        text.SetText(message);
    }

    public void SetSize(float size)
    {
        text.fontSize = size;
    }
}
