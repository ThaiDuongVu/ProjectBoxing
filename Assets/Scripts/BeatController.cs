using System.Collections;
using System.IO;
using UnityEngine;

public class BeatController : MonoBehaviour
{
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

    }

    private void Update()
    {
        if (beats == null) return;
        if (!_isBeatStarted) return;
        if (_currentIndex > beats.Length - 1) return;

        _timer += Time.deltaTime;

        if (_timer >= beats[_currentIndex].time)
        {
            // TODO: Execute action
            _currentIndex++;
        }
    }

    #endregion

    private IEnumerator StartBeat()
    {
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
