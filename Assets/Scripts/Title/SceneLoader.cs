using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string nextSceneName = "MainGame"; // 遷移先のシーン名

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
