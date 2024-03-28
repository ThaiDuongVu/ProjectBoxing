using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] private Transform main;
    [SerializeField] private UnityEvent onPress;

    private bool _pressEventExecuted;

    private void Update()
    {
        if (main.localPosition.y <= 0.03125f && !_pressEventExecuted)
        {
            OnButtonPressed();
            _pressEventExecuted = true;
        }

        if (main.localPosition.y >= 0.0625f && _pressEventExecuted)
        {
            _pressEventExecuted = false;
        }
    }

    private void OnButtonPressed()
    {
        onPress.Invoke();
    }
}
