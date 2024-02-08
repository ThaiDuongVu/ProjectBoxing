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

    private float _timer;
    private int _currentIndex;

    private AudioSource _song;
    private bool _isBeatStarted;

    #region Unity Events

    private void Awake()
    {
        _song = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        _song.Stop();
        // StartCoroutine(StartBeat());
    }

    private void Update()
    {
        if (beats == null) return;
        if (!_isBeatStarted) return;
        if (_currentIndex > beats.Length - 1) return;

        _timer += Time.deltaTime;

        if (_timer >= beats[_currentIndex].time)
        {
            // Execute action
            var action = beats[_currentIndex].action;
            if (action != 0)
            {
                Instantiate(interactablePrefabs[action - 1], new Vector3(0f, 1.75f, 4.5f), Quaternion.identity);
            }

            _currentIndex++;
        }
    }

    #endregion

    public IEnumerator StartBeat()
    {
        if (_isBeatStarted) yield break;

        _isBeatStarted = true;
        yield return new WaitForSeconds(5f);
        _song.Play();
    }

    private void PopulateBeats(string fileName)
    {
        var streamReader = new StreamReader($"{Application.dataPath}/Beats/{fileName}");
        var timeStrings = streamReader.ReadToEnd().Split("\n");
        streamReader.Close();

        beats = new Beat[timeStrings.Length];
        for (int i = 0; i < timeStrings.Length; i++) beats[i].time = System.Convert.ToSingle(timeStrings[i]);
    }
}
