using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour {

    public static GameStarter instance;
    public string sceneName = "playground";

    public LevelStats easy, normal, hard, ai;
    [HideInInspector] public LevelStats custom;

    void Awake()
    {
        Time.timeScale = 1;
        instance = this;
    }

    public void AI()
    {
        PlayerPrefs.SetInt("Mode", 4);
        Play(ai);
    }

    public void Easy()
    {
        PlayerPrefs.SetInt("Mode", 1);
        Play(easy);
    }

    public void Medium()
    {
        PlayerPrefs.SetInt("Mode", 2);
        Play(normal);
    }

    public void Hard()
    {
        PlayerPrefs.SetInt("Mode", 3);
        Play(hard);
    }

    public void Custom()
    {
        PlayerPrefs.SetInt("Mode", 0);
        Play(custom);
    }

    void Play(LevelStats stats)
    {
        stats.Save();
        SceneManager.LoadScene(sceneName);
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Exit()
    {
        Application.Quit();
    }

    [System.Serializable]
    public struct LevelStats
    {
        public int bombs;
        public int x;
        public int y;

        public void Save()
        {
            PlayerPrefs.SetInt("bombs", bombs);
            PlayerPrefs.SetInt("x", x);
            PlayerPrefs.SetInt("y", y);
        }
    }
}
