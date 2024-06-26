using UnityEngine;

public class ComboController : MonoBehaviour
{
    #region Singleton

    private static ComboController _comboControllerInstance;

    public static ComboController Instance
    {
        get
        {
            if (_comboControllerInstance == null) _comboControllerInstance = FindFirstObjectByType<ComboController>();
            return _comboControllerInstance;
        }
    }

    #endregion

    [SerializeField] private TV tv;

    public int CurrentCombo { get; private set; }
    private float _comboTimer;
    private const float ComboDecreaseFactor = 0.5f;

    #region Unity Events

    private void Start()
    {
        CurrentCombo = 1;
    }

    private void Update()
    {
        tv.SetComboText(CurrentCombo);

        if (CurrentCombo <= 1) 
        {
            tv.SetComboTextScale(Vector3.one);
            return;
        }

        if (_comboTimer > 0f)
        {
            _comboTimer -= Time.deltaTime * ComboDecreaseFactor;
        }
        else
        {
            CurrentCombo--;
            _comboTimer = 1f;
        }
        tv.SetComboTextScale(new Vector3(_comboTimer + 0.5f, _comboTimer + 0.5f, 1f));
    }

    #endregion

    public void AddCombo(int amount = 1)
    {
        CurrentCombo += 1;
        _comboTimer = 1f;
    }
}
