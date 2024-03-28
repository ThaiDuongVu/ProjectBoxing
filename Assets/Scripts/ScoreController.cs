using UnityEngine;

public class ScoreController : MonoBehaviour
{
    #region Singleton

    private static ScoreController _scoreControllerInstance;

    public static ScoreController Instance
    {
        get
        {
            if (_scoreControllerInstance == null) _scoreControllerInstance = FindFirstObjectByType<ScoreController>();
            return _scoreControllerInstance;
        }
    }

    #endregion

    private int _score;
    public int CurrentScore
    {
        get => _score;
        set
        {
            _score = value;
            tv.SetScoreText(_score);
        }
    }

    [SerializeField] private TV tv;
}
