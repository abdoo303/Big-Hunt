using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene transitions and application control for the main menu.
/// Provides methods for starting the game and exiting the application.
/// Note: Class name has a typo - should be "SceneManager" but conflicts with Unity's SceneManager.
/// </summary>
public class SceneManger : MonoBehaviour
{
    /// <summary>
    /// Starts the game by loading Level1.
    /// Typically called from a Play button in the main menu.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Exits the application.
    /// Typically called from an Exit/Quit button in the main menu.
    /// Note: Has no effect in the Unity Editor.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
