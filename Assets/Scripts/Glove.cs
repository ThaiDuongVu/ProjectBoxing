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
    public Vector3 CurrentDirection => _currentVelocity.normalized;
    public float CurrentSpeed => _currentVelocity.magnitude;
    private const float MinPunchSpeed = 0.2f;
    public bool MinSpeedReached => CurrentSpeed >= MinPunchSpeed;

    private bool _collisionBuffer = true;

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
    }

    private void FixedUpdate()
    {
        _currentVelocity = (transform.position - _lastPosition) * VelocityScale;
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

    public void Vibrate(float intensity, float duration)
    {
        if (!_controller) return;
        _controller.SendHapticImpulse(intensity, duration);
    }

    private void EnableCollisionBuffer()
    {
        _collisionBuffer = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        var contactPoint = other.GetContact(0).point;

        if (other.transform.CompareTag("Target"))
        {
            var target = other.transform.GetComponent<Destructible>();
            var targetDirection = target.transform.forward;

            // Check if target is already shattered
            if (target.IsShattered) return;
            // Check if glove/target type match
            if (target.controllerType != controllerType) return;
            // Check if fist is closed
            if (!IsClosed) return;
            // Check if glove is fast enough
            if (!MinSpeedReached) return;
            // Check if glove direction match target's
            if ((CurrentDirection - targetDirection).magnitude > 0.4f) return;

            // TODO: Update combo based on optimum time
            ComboController.Instance.AddCombo();
            Debug.Log(target.AbsoluteOptimumTime);
            var addedScore = 100 * ComboController.Instance.CurrentCombo;
            ScoreController.Instance.CurrentScore += 100 * ComboController.Instance.CurrentCombo;

            target.Shatter(-_currentVelocity.normalized, contactPoint, CurrentSpeed * 10f);
            Destroy(target.transform.parent.gameObject);
            Vibrate(CurrentSpeed / 2f, CurrentSpeed / 2f);

            var feedbackText = Instantiate(feedbackTextPrefab, contactPoint, Quaternion.identity);
            feedbackText.SetText($"+{100 * ComboController.Instance.CurrentCombo}");
            feedbackText.SetColor(textColor);
            feedbackText.SetSize(CurrentSpeed / MinPunchSpeed / 2f);
        }
        // If the gloves are smashed together then start/pause the song
        else if (other.transform.CompareTag("Glove"))
        {
            // Prevent spamming
            if (_collisionBuffer)
            {
                var otherGlove = other.transform.GetComponent<Glove>();

                // Check if both gloves are closed
                if (!IsClosed || !otherGlove.IsClosed) return;
                // Check speed of both gloves
                if (!MinSpeedReached || !otherGlove.MinSpeedReached) return;
                // Prevent accidental triggers
                if (controllerType == ControllerType.Left && CurrentDirection.x < 0.75f && transform.forward.x < 0.75f) return;
                if (controllerType == ControllerType.Right && CurrentDirection.x > -0.75f && transform.forward.x > -0.75f) return;

                Instantiate(explosionPrefab, contactPoint, Quaternion.identity);
                Vibrate(CurrentSpeed, CurrentSpeed);

                // Only run this code once
                if (controllerType == ControllerType.Left)
                {
                    // Start/pause/resume the song
                    if (BeatController.Instance.IsBeatInit) BeatController.Instance.ToggleBeat();
                    else BeatController.Instance.StartCoroutine(BeatController.Instance.StartBeat());
                }

                _collisionBuffer = false;
                Invoke(nameof(EnableCollisionBuffer), 0.1f);
            }
        }
    }
}
