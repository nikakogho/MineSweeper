using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class AITile : MonoBehaviour {
    public Image image;
    public Text text;
    public bool opened = false;
    public bool hasBomb = false;
    public int bombsAroundIt = 0;
    public int x, y;

    GameMaster master;
    public TileState state = TileState.untouched;
    public List<AITile> neighbours = new List<AITile>();
    
    public List<AITile> GetMutualNeighbors(AITile tile)
    {
        List<AITile> mutuals = new List<AITile>();

        foreach(AITile t1 in tile.neighbours)
        {
            foreach(AITile t2 in neighbours)
            {
                if(t1 == t2)
                {
                    mutuals.Add(t1);
                    break;
                }
            }
        }

        return mutuals;
    }

    public int GetUnopenedNeighbours
    {
        get
        {
            int amount = 0;
            foreach (AITile t in neighbours) if (!t.opened) amount++;

            return amount;
        }
    }

    public int GetFreeNeighbours
    {
        get
        {
            int amount = 0;

            foreach (AITile tile in neighbours) if (!tile.opened && tile.state != TileState.flagged) amount++;

            return amount;
        }
    }

    void Start()
    {
        text.enabled = false;
        text.text = "";
    }

    public void Begin(GameMaster master)
    {
        this.master = master;
        for (int X = x - 1; X <= x + 1 && X < master.x; X++)
        {
            if (X < 0) continue;
            for (int Y = y - 1; Y <= y + 1 && Y < master.y; Y++)
            {
                if (Y < 0) continue;

                if (X == x && Y == y) continue;

                AITile tile = master.aiTiles[X, Y];
                neighbours.Add(tile);
            }
        }
    }

    public void Apply()
    {
        bombsAroundIt = 0;
        foreach (AITile t in neighbours) if (t.hasBomb) bombsAroundIt++;
        if (bombsAroundIt > 0)
        {
            text.text = bombsAroundIt.ToString();
            text.color = ColorFromNumber(bombsAroundIt);
        }
    }

    Color ColorFromNumber(int number)
    {
        return master.numberColors[number];
    }

    public void RightClick()
    {
        if (opened)
        {
            OpenNeighbours();

            return;
        }

        switch (state)
        {
            case TileState.untouched:
                state = TileState.flagged;
                master.flagged++;
                image.sprite = master.flag;
                break;
            case TileState.flagged:
                master.flagged--;
                state = TileState.questioned;
                image.sprite = master.questionMark;
                break;
            case TileState.questioned:
                state = TileState.untouched;
                image.sprite = master.defaultSprite;
                break;
        }
    }

    public bool OpenNeighbours()
    {
        if (!opened) return false;
        int countedBombs = 0;
        foreach (AITile tile in neighbours) if (tile.state == TileState.flagged) countedBombs++;

        if (countedBombs != bombsAroundIt) return false;

        bool actuallyOpenedSomeone = false;

        foreach (AITile tile in neighbours)
        {
            if (tile.state == TileState.untouched)
            {
                tile.Open();
                actuallyOpenedSomeone = true;
            }
        }

        return actuallyOpenedSomeone;
    }

    public void Open()
    {
        if (opened) return;
        opened = true;

        if (hasBomb)
        {
            master.Lose();
            return;
        }

        master.opened++;

        text.enabled = true;
        state = TileState.opened;
        image.sprite = master.unlockedSprite;

        if (bombsAroundIt == 0)
        {
            image.color = Color.grey;
        }
        else
            text.text = bombsAroundIt.ToString();
        
        text.enabled = true;

        OpenNeighbours();
    }

    public void Click()
    {
        if (!master.initialized) master.Initialize(this);

        if (state == TileState.flagged) return;

        if (!opened) Open();
    }
}
