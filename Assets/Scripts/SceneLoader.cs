using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        MainCameraController.Instance.Outro();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
