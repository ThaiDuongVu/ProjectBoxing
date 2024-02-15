using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Glove : MonoBehaviour
{
    public ControllerType controllerType;

    [Header("Particle References")]
    [SerializeField] private ParticleSystem explosionPrefab;

    [Header("Text References")]
    [SerializeField] private FeedbackText feedbackTextPrefab;
    [SerializeField] private Color textColor;

    private Animator _animator;
    private static readonly int CloseFingersAnimationBool = Animator.StringToHash("isClosedFingers");
    private static readonly int CloseThumbsAnimationBool = Animator.StringToHash("isClosedThumbs");
    public bool IsClosed { get; private set; }

    private bool _fingersClosed;
    private bool _thumbClosed;

    private Vector3 _lastPosition;
    private Vector3 _currentVelocity;
    private const float VelocityScale = 10f;
    private const float MinPunchVelocity = 0.1f;

    private XRBaseController _controller;
    private XRInputManager _inputManager;

    #region Unity Events

    private void OnEnable()
    {
        _inputManager = new XRInputManager();

        // Handle glove open/close input
        switch (controllerType)
        {
            // For the left controller
            case ControllerType.Left:
                _inputManager.XRILeftHandInteraction.Select.performed += SelectOnPerformed;
                _inputManager.XRILeftHandInteraction.Select.canceled += SelectOnCanceled;
                _inputManager.XRILeftHandInteraction.Activate.performed += ActivateOnPerformed;
                _inputManager.XRILeftHandInteraction.Activate.canceled += ActivateOnCanceled;
                break;

            // For the right controller
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
        // Handle glove open/close
        _animator.SetBool(CloseThumbsAnimationBool, _thumbClosed);
        _animator.SetBool(CloseFingersAnimationBool, _fingersClosed);
        IsClosed = _thumbClosed && _fingersClosed;

        _currentVelocity = (_lastPosition - transform.position) * VelocityScale;
        _lastPosition = transform.position;
    }

    #endregion

    #region Input Handlers

    private void SelectOnPerformed(InputAction.CallbackContext context)
    {
        _thumbClosed = true;
    }

    private void SelectOnCanceled(InputAction.CallbackContext context)
    {
        _thumbClosed = false;
    }

    private void ActivateOnPerformed(InputAction.CallbackContext context)
    {
        _fingersClosed = true;
    }

    private void ActivateOnCanceled(InputAction.CallbackContext context)
    {
        _fingersClosed = false;
    }

    #endregion

    #region Open & Close Glove

    private void Open()
    {
        if (!IsClosed) return;

        IsClosed = false;
    }

    private void Close()
    {
        if (IsClosed) return;

        IsClosed = true;
    }

    #endregion

    private void Vibrate(float intensity, float duration)
    {
        if (!_controller) return;
        _controller.SendHapticImpulse(intensity, duration);
    }

    private void OnCollisionEnter(Collision other)
    {
        var contactPoint = other.GetContact(0).point;
        var velocityMagnitude = _currentVelocity.magnitude;

        if (other.transform.CompareTag("Target"))
        {
            var target = other.transform.GetComponent<Target>();

            // Check if target is already shattered
            if (target.IsShattered) return;
            // Check if glove/target type match
            if (target.controllerType != controllerType) return;
            // Check if fist is closed
            if (!IsClosed) return;
            // Check if glove is fast enough
            if (velocityMagnitude < MinPunchVelocity) return;

            target.Shatter(-_currentVelocity.normalized, contactPoint, velocityMagnitude * 10f);
            Destroy(target.transform.parent.gameObject);
            Vibrate(velocityMagnitude / 2f, velocityMagnitude / 2f);

            var feedbackText = Instantiate(feedbackTextPrefab, contactPoint, Quaternion.identity);
            feedbackText.SetColor(textColor);
            feedbackText.SetSize(velocityMagnitude / MinPunchVelocity * 0.5f);
        }
        // If the gloves are smashed together then start the beat
        else if (other.transform.CompareTag("Glove"))
        {
            var otherGlove = other.transform.GetComponent<Glove>();

            // Check if fist is closed
            if (!IsClosed) return;
            // Check speed of glove
            if (velocityMagnitude < MinPunchVelocity) return;

            Instantiate(explosionPrefab, contactPoint, Quaternion.identity);
            StartCoroutine(BeatController.Instance.StartBeat());
            Vibrate(velocityMagnitude, velocityMagnitude);
        }
    }
}
