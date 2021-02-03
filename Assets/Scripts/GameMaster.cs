using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public GridLayoutGroup group;
    public int bombs, x, y;
    public GameObject tilePrefab, aiTilePrefab;
    public GameTile[,] tiles;
    public AITile[,] aiTiles;
    public Sprite questionMark, flag, bomb, defaultSprite, unlockedSprite, mouseOverSprite;
    public AudioSource source;
    public AudioClip initializeSound, explosionSound;
    public float explosionDelta = 0.5f;
    public float loadDelta = 0.005f;
    public Text mines, timePastText;
    public int opened = 0, flagged = 0;
    public float timePast = 0;
    public GameObject winUI, loseUI;
    public GameObject aiObject;

    public Color[] numberColors = new Color[8];

    [HideInInspector]public bool over = false;

    void Awake()
    {
        Time.timeScale = 1;
        instance = this;

        bombs = PlayerPrefs.GetInt("bombs");
        x = PlayerPrefs.GetInt("x");
        y = PlayerPrefs.GetInt("y");

        group.constraintCount = x;
        int limitSize = Mathf.Min(Screen.width / x, Screen.height / y);
        group.cellSize = Vector2.one * limitSize;

        if (PlayerPrefs.GetInt("Mode") == 4)
            aiTiles = new AITile[x, y];
        else
            tiles = new GameTile[x, y];

        GenerateTiles();
    }

    void GenerateTiles()
    {
        int mode = PlayerPrefs.GetInt("Mode");

        for(int Y = 0; Y < y; Y++)
        {
            for(int X = 0; X < x; X++)
            {
                if(mode == 4)
                {
                    AITile aiTile = Instantiate(aiTilePrefab, transform.position, Quaternion.identity, transform).GetComponent<AITile>();

                    aiTile.x = X;
                    aiTile.y = Y;
                    aiTiles[X, Y] = aiTile;
                    continue;
                }

                GameTile tile = Instantiate(tilePrefab, transform.position, Quaternion.identity, transform).GetComponent<GameTile>();

                tile.x = X;
                tile.y = Y;
                tiles[X, Y] = tile;
            }
        }

        if(mode == 4)
        {
            foreach (AITile tile in aiTiles)
            {
                tile.Begin(this);
            }
        }
        else
        {
            foreach (GameTile tile in tiles)
            {
                tile.Begin(this);
            }
        }

        if(mode == 4)
        {
            StartCoroutine(StartAI());
        }
    }

    public float aiStartAfter = 1.5f;

    IEnumerator StartAI()
    {
        yield return new WaitForSeconds(aiStartAfter);
        aiObject.SetActive(true);
    }

    public bool initialized = false;

    public void Initialize(AITile tile)
    {
        if (initialized) { Debug.Log("Already init"); return; }
        source.clip = initializeSound;
        source.Play();
        initialized = true;
        List<Vector2> possiblePoints = new List<Vector2>();

        foreach (AITile t in aiTiles)
        {
            bool canUse = t != tile;

            if (canUse)
            {
                foreach (AITile t1 in tile.neighbours)
                {
                    if (t1 == t) { canUse = false; break; }
                }
            }

            if (canUse) possiblePoints.Add(new Vector2(t.x, t.y));
        }

        for (int i = 0; i < bombs; i++)
        {
            int index = Random.Range(0, possiblePoints.Count);

            Vector2 point = possiblePoints[index];
            possiblePoints.RemoveAt(index);

            aiTiles[(int)point.x, (int)point.y].hasBomb = true;
        }

        foreach (AITile t in aiTiles) t.Apply();
    }

    public void Initialize(GameTile tile)
    {
        if (initialized) { Debug.Log("Already init"); return; }
        source.clip = initializeSound;
        source.Play();
        initialized = true;
        List<Vector2> possiblePoints = new List<Vector2>();

        foreach(GameTile t in tiles)
        {
            bool canUse = t != tile;

            if (canUse)
            {
                foreach (GameTile t1 in tile.neighbours)
                {
                    if(t1 == t) { canUse = false; break; }
                }
            }

            if (canUse) possiblePoints.Add(new Vector2(t.x, t.y));
        }

        for(int i = 0; i < bombs; i++)
        {
            int index = Random.Range(0, possiblePoints.Count);

            Vector2 point = possiblePoints[index];
            possiblePoints.RemoveAt(index);

            tiles[(int)point.x, (int)point.y].hasBomb = true;
        }

        foreach (GameTile t in tiles) t.Apply();
    }

    public void Lose()
    {
        if (over) return;
        aiObject.SetActive(false);
        over = true;

        if (PlayerPrefs.GetInt("Mode") == 4)
            foreach (AITile tile in aiTiles)
            {
                if (tile.hasBomb)
                    tile.image.sprite = bomb;
            }
        else
            foreach (GameTile tile in tiles)
            {
                if (tile.hasBomb)
                    tile.image.sprite = bomb;
            }

        StartCoroutine(Boom());
    }

    void Win()
    {
        Time.timeScale = 0;
        winUI.SetActive(true);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (over)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StopAllCoroutines();
                EndBoom();
            }
        }
        else
        {
            mines.text = "Mines : " + (bombs - flagged);

            if (initialized)
            {
                timePast += Time.deltaTime;

                timePastText.text = "Time : " + ((int)timePast);
            }
        }
    }

    void FixedUpdate()
    {
        if (over)
        {
            return;
        }

        if(opened == x * y - bombs)
        {
            over = true;
            Win();
        }
    }

    bool skipBoom = false;

    void EndBoom()
    {
        loseUI.SetActive(true);
        gameObject.SetActive(false);
    }

    IEnumerator Boom()
    {
        source.clip = explosionSound;

        if(PlayerPrefs.GetInt("Mode") == 4)
        {
            foreach (AITile tile in aiTiles)
            {
                if (!tile.hasBomb) continue;
                source.Play();
                tile.image.color = new Color(0, 0, 0, 0.5f);
                yield return new WaitForSeconds(skipBoom ? 0 : explosionDelta);
            }
        }
        else
        {
            foreach (GameTile tile in tiles)
            {
                if (!tile.hasBomb) continue;
                source.Play();
                tile.image.color = new Color(0, 0, 0, 0.5f);
                yield return new WaitForSeconds(skipBoom ? 0 : explosionDelta);
            }
        }

        EndBoom();
    }
}
