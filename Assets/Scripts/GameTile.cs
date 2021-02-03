using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameTile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Text text;
    public bool opened = false;
    public bool hasBomb = false;
    public int bombsAroundIt = 0;
    public int x, y;

    GameMaster master;
    TileState state = TileState.untouched;
    public List<GameTile> neighbours = new List<GameTile>();

    /*
    bool mouseIsOver = false;

    void OnMouseEnter()
    {
        mouseIsOver = true;
    }

    void OnMouseExit()
    {
        mouseIsOver = false;
    }
    */

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

                GameTile tile = master.tiles[X, Y];
                neighbours.Add(tile);
            }
        }
    }

    public void Apply()
    {
        bombsAroundIt = 0;
        foreach (GameTile t in neighbours) if (t.hasBomb) bombsAroundIt++;
        if(bombsAroundIt > 0)
        {
            text.text = bombsAroundIt.ToString();
            text.color = ColorFromNumber(bombsAroundIt);
        }
    }

    Color ColorFromNumber(int number)
    {
        return master.numberColors[number];
    }

    void RightClick()
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

    void OpenNeighbours()
    {
        if (!opened) return;
        int countedBombs = 0;
        foreach (GameTile tile in neighbours) if (tile.state == TileState.flagged) countedBombs++;

        if (countedBombs != bombsAroundIt) return;

        foreach (GameTile tile in neighbours)if (tile.state == TileState.untouched) tile.Open();
    }

    void Open()
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

        if(bombsAroundIt == 0)
        {
            image.color = Color.grey;
            OpenNeighbours();
        }
    }

    public void Click()
    {
        if (!master.initialized) master.Initialize(this);

        if (state == TileState.flagged) return;

        if (!opened) Open();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left: Click();
                break;
            case PointerEventData.InputButton.Right: RightClick();
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!opened)
        {
            image.sprite = master.mouseOverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!opened)
        {
            switch (state)
            {
                case TileState.flagged:
                    image.sprite = master.flag;
                    break;
                case TileState.untouched:
                    image.sprite = master.defaultSprite;
                    break;
                case TileState.questioned:
                    image.sprite = master.questionMark;
                    break;
            }
        }
    }
}

public enum TileState { untouched, questioned, opened, flagged }
