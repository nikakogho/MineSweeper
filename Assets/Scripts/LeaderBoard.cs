using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public Text text;

	void OnEnable()
    {
        string EasyPart = "\nEasy :";
        string MediumPart = "\nMedium :";
        string HardPart = "\nHard :";
        int totalEasy = PlayerPrefs.GetInt("Exists Easy", 0);
        int totalMedium = PlayerPrefs.GetInt("Exists Medium", 0);
        int totalHard = PlayerPrefs.GetInt("Exists Hard", 0);
        totalEasy++;
        totalMedium++;
        totalHard++;
        for (int i = 1; i < totalEasy; i++)
        {
            EasyPart += "\n " + i + ". " + PlayerPrefs.GetString("Easy Name " + i) + " : " + PlayerPrefs.GetInt("Easy Score " + i);
        }
        for (int i = 1; i < totalMedium; i++)
        {
            MediumPart += "\n " + i + ". " + PlayerPrefs.GetString("Medium Name " + i) + " : " + PlayerPrefs.GetInt("Medium Score " + i);
        }
        for (int i = 1; i < totalHard; i++)
        {
            HardPart += "\n " + i + ". " + PlayerPrefs.GetString("Hard Name " + i) + " : " + PlayerPrefs.GetInt("Hard Score " + i);
        }

        text.text = "LeaderBoard : " + EasyPart + MediumPart + HardPart;
    }
}
