using System.Collections;
using System.IO;
using UnityEngine;

public class BeatController : MonoBehaviour
{
    #region Singleton

    private static BeatController _beatControllerInstance;

    public static BeatController Instance
    {
        get
        {
            if (_beatControllerInstance == null) _beatControllerInstance = FindFirstObjectByType<BeatController>();
            return _beatControllerInstance;
        }
    }

    #endregion

    [Header("Targets")]
    [SerializeField] private Interactable[] interactablePrefabs;

    [Header("Beats")]
    [SerializeField] private Beat[] beats;

    [Header("Misc References")]
    [SerializeField] private TV tv;

    private float _timer;
    private int _currentIndex;
    private bool _isBeatStarted;
    public bool IsBeatInit { get; private set; }

    private const float SpawnDistance = 5f;
    public const float ScrollSpeed = 1f;
    public float CurrentScrollSpeed { get; private set; } = ScrollSpeed;

    private AudioSource _song;

    #region Unity Events

    private void Awake()
    {
        _song = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        // StartCoroutine(StartBeat());
    }

    private void Update()
    {
        if (!_isBeatStarted) return;
        if (_currentIndex > beats.Length - 1) return;

        _timer += Time.deltaTime;

        if (_timer >= beats[_currentIndex].time)
        {
            // Execute action
            var action = beats[_currentIndex].action;
            if (action != 0)
            {
                Instantiate(interactablePrefabs[action - 1], transform.position + Vector3.forward * SpawnDistance, Quaternion.identity);
            }

            _currentIndex++;
        }
    }

    #endregion

    public IEnumerator StartBeat()
    {
        _isBeatStarted = true;
        tv.SetMessage("Preparing song...");

        yield return new WaitForSeconds(SpawnDistance);
        IsBeatInit = true;
        tv.SetMessage("Vibing~");
        _song.Play();
    }

    public void PauseBeat()
    {
        _isBeatStarted = false;
        CurrentScrollSpeed = 0f;
        tv.SetMessage("Paused");

        _song.Pause();
    }

    public void ResumeBeat()
    {
        _isBeatStarted = true;
        CurrentScrollSpeed = ScrollSpeed;
        tv.SetMessage("Vibing~");
        
        _song.UnPause();
    }

    public void ToggleBeat()
    {
        if (_isBeatStarted) PauseBeat();
        else ResumeBeat();
    }

    // Execute in edit to fill beat arrays with timestamops
    private void PopulateBeats(string fileName)
    {
        var streamReader = new StreamReader($"{Application.dataPath}/Beats/{fileName}");
        var timeStrings = streamReader.ReadToEnd().Split("\n");
        streamReader.Close();

        beats = new Beat[timeStrings.Length];
        for (int i = 0; i < timeStrings.Length; i++) beats[i].time = System.Convert.ToSingle(timeStrings[i]);
    }
}
