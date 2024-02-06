using System.Collections;
using System.IO;
using UnityEngine;

public class BeatController2 : MonoBehaviour
{
    [SerializeField] private FeedbackText feedbackTextPrefab;
    [SerializeField] private TargetSet[] targetSetPrefabs;
    private int _temp = 0;

    private float _timer;
    private float[] _beatTimes;
    private int _currentIndex;

    #region Unity Events

    private IEnumerator Start()
    {
        var streamReader = new StreamReader($"{Application.dataPath}/Beats/last_christmas.txt");
        var timeStrings = streamReader.ReadToEnd().Split("\n");
        streamReader.Close();

        _beatTimes = new float[timeStrings.Length];
        for (int i = 0; i < timeStrings.Length; i++)
        {
            _beatTimes[i] = System.Convert.ToSingle(timeStrings[i]);
        }

        yield return new WaitForSeconds(5f);

        // _beatStart = true;
        GetComponent<AudioSource>().Play();
        Debug.Log("Play");
    }

    private void Update()
    {
        if (_beatTimes == null) return;
        if (_currentIndex > _beatTimes.Length - 1) return;

        _timer += Time.deltaTime;
        if (_timer >= _beatTimes[_currentIndex])
        {
            // Debug.Log($"BEAT! {_beatTimes[_currentIndex]}");
            // Instantiate(feedbackTextPrefab, new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(1.5f, 2f), 0f), Quaternion.identity).SetColor(Color.red);

            if (_currentIndex % 2 == 0)
            {
                Instantiate(targetSetPrefabs[_temp], new Vector3(0f, 1.75f, 4.5f), Quaternion.identity);
                _temp = _temp == 0 ? 1 : 0;
            }
            _currentIndex++;
        }
    }

    #endregion
}
