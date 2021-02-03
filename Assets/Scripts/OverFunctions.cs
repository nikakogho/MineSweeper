using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverFunctions : MonoBehaviour {

    public string menuName = "MainMenu";
    public InputField field;

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuName);
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SubmitName()
    {
        string playerName = field.text;
        string mode = "";

        switch (PlayerPrefs.GetInt("Mode"))
        {
            case 1:
                mode = "Easy";
                break;
            case 2:
                mode = "Medium";
                break;
            case 3:
                mode = "Hard";
                break;
        }

        int index = PlayerPrefs.GetInt("Exists " + mode, 0) + 1;
        PlayerPrefs.SetInt("Exists " + mode, index);
        PlayerPrefs.SetString(mode + " Name " + index, playerName);
        PlayerPrefs.SetInt(mode + " Score " + index, (int)GameMaster.instance.timePast);
    }
}
