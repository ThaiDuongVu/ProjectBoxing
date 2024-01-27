using UnityEngine;
using UnityEngine.InputSystem;

public class Glove : MonoBehaviour
{
    public ControllerType controllerType;

    private Animator _animator;
    private static readonly int CloseAnimationBool = Animator.StringToHash("isClosed");
    public bool IsClosed { get; private set; }

    private Vector3 _lastPosition;
    private Vector3 _currentVelocity;
    private const float VelocityScale = 10f;

    private XRInputManager _inputManager;

    #region Unity Events

    private void OnEnable()
    {
        _inputManager = new XRInputManager();

        // Handle glove open/close input
        switch (controllerType)
        {
            case ControllerType.Left:
                _inputManager.XRILeftHandInteraction.Select.performed += SelectOnPerformed;
                _inputManager.XRILeftHandInteraction.Select.canceled += SelectOnCanceled;
                break;

            case ControllerType.Right:
                _inputManager.XRIRightHandInteraction.Select.performed += SelectOnPerformed;
                _inputManager.XRIRightHandInteraction.Select.canceled += SelectOnCanceled;
                break;

            default:
                break;
        }


        _inputManager.Enable();
    }

    private void OnDisable()
    {
        _inputManager.Disable();
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        _currentVelocity = (_lastPosition - transform.position) * VelocityScale;
        _lastPosition = transform.position;
    }

    #endregion

    #region Input Handlers

    private void SelectOnPerformed(InputAction.CallbackContext context)
    {
        Close();
    }

    private void SelectOnCanceled(InputAction.CallbackContext context)
    {
        Open();
    }

    #endregion

    #region Open & Close Fist

    private void Open()
    {
        IsClosed = false;
        _animator.SetBool(CloseAnimationBool, false);
    }

    private void Close()
    {
        IsClosed = true;
        _animator.SetBool(CloseAnimationBool, true);
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            var target = other.GetComponent<Target>();
            target.Shatter(_currentVelocity.normalized, other.ClosestPoint(transform.position));
            Destroy(target.gameObject);
        }
    }
}
