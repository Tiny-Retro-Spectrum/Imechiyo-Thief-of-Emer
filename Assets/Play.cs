using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public void doPlay()
    {
        SceneManager.LoadScene("Intro");
    }
}
