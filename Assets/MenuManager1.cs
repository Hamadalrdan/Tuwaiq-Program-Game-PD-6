using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager1 : MonoBehaviour
{

    //Play BUtton function
    public void PlayButton_Pressed()
    {
        //SceneManager.LoadScene("Game");
        SceneManager.LoadScene(1);
    }

    //Exit Button Function
    public void ExitButton_Pressed()
    {
        Debug.Log("Exit Button Pressed!");

        //Exit Game.
        Application.Quit();
    }

    public void RestartButton_Pressed()
    {
        SceneManager.LoadScene("Game");
    }

    public void MenuButton_Pressed()
    {

        SceneManager.LoadScene("Menu");
    }
}

