using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour {

    public string nameOfLevel;
    public void loadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfLevel);
    }

        
}
