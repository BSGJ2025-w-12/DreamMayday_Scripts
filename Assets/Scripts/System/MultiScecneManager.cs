using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiScecneManager : MonoBehaviour
{
    private Scene Scene1;
    private Scene Scene2;
    private Scene Scene3;
    void Start()
    {
        SceneManager.LoadSceneAsync("Dream0", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Dream2", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("HIime", LoadSceneMode.Additive);
    }
}

