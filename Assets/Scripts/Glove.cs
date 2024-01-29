using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Glove : MonoBehaviour
{
    public ControllerType controllerType;

    private Animator _animator;
    private static readonly int CloseAnimationBool = Animator.StringToHash("isClosed");
    public bool IsClosed { get; private set; }

    private bool _indexFingerClosed;
    private bool _middleFingerClosed;

    private Vector3 _lastPosition;
    private Vector3 _currentVelocity;
    private const float VelocityScale = 10f;

    private XRBaseController _controller;
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
                _inputManager.XRILeftHandInteraction.Activate.performed += ActivateOnPerformed;
                _inputManager.XRILeftHandInteraction.Activate.canceled += ActivateOnCanceled;
                break;

            case ControllerType.Right:
                _inputManager.XRIRightHandInteraction.Select.performed += SelectOnPerformed;
                _inputManager.XRIRightHandInteraction.Select.canceled += SelectOnCanceled;
                _inputManager.XRIRightHandInteraction.Activate.performed += ActivateOnPerformed;
                _inputManager.XRIRightHandInteraction.Activate.canceled += ActivateOnCanceled;
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
        _controller = GetComponentInParent<XRBaseController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        // Handle fist open/close
        if (_middleFingerClosed && _indexFingerClosed) Close();
        else Open();

        _currentVelocity = (_lastPosition - transform.position) * VelocityScale;
        _lastPosition = transform.position;
    }

    #endregion

    #region Input Handlers

    private void SelectOnPerformed(InputAction.CallbackContext context)
    {
        _middleFingerClosed = true;
    }

    private void SelectOnCanceled(InputAction.CallbackContext context)
    {
        _middleFingerClosed = false;
    }

    private void ActivateOnPerformed(InputAction.CallbackContext context)
    {
        _indexFingerClosed = true;
    }

    private void ActivateOnCanceled(InputAction.CallbackContext context)
    {
        _indexFingerClosed = false;
    }

    #endregion

    #region Open & Close Fist

    private void Open()
    {
        if (!IsClosed) return;

        IsClosed = false;
        _animator.SetBool(CloseAnimationBool, false);
    }

    private void Close()
    {
        if (IsClosed) return;

        IsClosed = true;
        _animator.SetBool(CloseAnimationBool, true);
    }

    #endregion

    private void Vibrate(float intensity, float duration)
    {
        if (!_controller) return;
        _controller.SendHapticImpulse(intensity, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            var target = other.GetComponent<Target>();
            if (target.controllerType != controllerType) return;
            if (!IsClosed) return;

            Debug.Log(_currentVelocity.magnitude);

            target.Shatter(_currentVelocity.normalized, other.ClosestPoint(transform.position));
            Destroy(target.gameObject);
            Vibrate(0.25f, 0.25f);
        }
    }
}