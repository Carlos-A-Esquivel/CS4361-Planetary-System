using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToSystem : MonoBehaviour
{
    [SerializeField] private string systemSceneName = "SolarSystem";
    [SerializeField] private KeyCode returnKey = KeyCode.Backspace;

    void Update()
    {
        if (Input.GetKeyDown(returnKey))
        {
            SceneManager.LoadScene(systemSceneName);
        }
    }
}