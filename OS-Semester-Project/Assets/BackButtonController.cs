using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonController : MonoBehaviour
{
    // Method to go back to the main menu scene
    public void backToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
