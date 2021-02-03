using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour {
    public GameMaster master;
    AITile[,] tiles;
    int x, y;
    public float actionDelta = 1;
    public float waitForMoveTime = 6;

    void OnEnable()
    {
        tiles = master.aiTiles;
        x = master.x;
        y = master.y;

        StartCoroutine(Act());
    }

    IEnumerator Act()
    {
        tiles[Random.Range(0, x), Random.Range(0, y)].Click();
        yield return new WaitForSeconds(actionDelta * 3);

        float lastActTime = Time.time;

        while (!master.over)
        {
            foreach (AITile tile in tiles)
            {
                if (tile.opened)
                {
                    if (tile.OpenNeighbours())
                    {
                        yield return new WaitForSeconds(actionDelta / 2);
                        lastActTime = Time.time;
                    }

                    List<AITile> unopenedTiles = new List<AITile>();

                    foreach (AITile t in tile.neighbours)
                    {
                        if (!t.opened)
                        {
                            unopenedTiles.Add(t);
                        }
                    }

                    if (tile.bombsAroundIt == unopenedTiles.Count)
                    {
                        foreach (AITile t in unopenedTiles)
                        {
                            if (t.state != TileState.flagged)
                            {
                                t.RightClick();
                                lastActTime = Time.time;
                            }
                        }
                    }

                    foreach (AITile t in tile.neighbours)
                    {
                        if (!t.opened) continue;

                        List<AITile> mutualNeighbours = tile.GetMutualNeighbors(t);

                        int mutualFrees = 0, mutualFlagged = 0;
                        foreach (AITile aiTile in mutualNeighbours)
                        {
                            if(aiTile.state == TileState.flagged)
                            {
                                mutualFlagged++;
                            }
                            else if (!aiTile.opened)
                            {
                                mutualFrees++;
                            }
                        }

                        if (mutualFrees == 0) continue;

                        int tFlaggeds = 0;
                        foreach (AITile aiTile in t.neighbours)
                        {
                            if (aiTile.state == TileState.flagged)
                            {
                                tFlaggeds++;
                            }
                        }

                        int tileOnlyFlaggeds = 0;
                        foreach(AITile aiTile in tile.neighbours)
                        {
                            if (aiTile != t && !mutualNeighbours.Contains(aiTile) && aiTile.state == TileState.flagged) tileOnlyFlaggeds++;
                        }

                        if (t.bombsAroundIt - tFlaggeds <= mutualFrees && t.GetFreeNeighbours == mutualFrees)
                        {
                            if (tile.bombsAroundIt - tileOnlyFlaggeds <= t.bombsAroundIt - tFlaggeds)
                            {
                                foreach (AITile t2 in tile.neighbours)
                                {
                                    if (!t2.opened && !mutualNeighbours.Contains(t2) && t2.state != TileState.flagged)
                                    {
                                        t2.Click();
                                        lastActTime = Time.time;
                                    }
                                }
                            }
                        }

                        int mutualNecessaryFlaggeds = t.bombsAroundIt - (tFlaggeds - mutualFlagged);
                        
                        if(t.GetFreeNeighbours == mutualFrees && tile.bombsAroundIt - mutualNecessaryFlaggeds == tile.GetUnopenedNeighbours - mutualNeighbours.Count)
                        {
                            foreach(AITile t2 in tile.neighbours)
                            {
                                if(!t2.opened && t2.state != TileState.flagged && !mutualNeighbours.Contains(t2))
                                {
                                    t2.RightClick();
                                    Debug.Log("Tile : " + tile.x + " " + tile.y + " Neighbor : " + t.x + " " + t.y + " Result : " + t2.x + " " + t2.y);
                                    lastActTime = Time.time;
                                }
                            }
                        }
                    }
                }
            }

            if (Time.time - lastActTime > waitForMoveTime)
            {
                List<AITile> unopenedTiles = new List<AITile>();

                foreach (AITile t in tiles)
                {
                    if (!t.opened && t.state != TileState.flagged)
                    {
                        unopenedTiles.Add(t);
                    }
                }

                unopenedTiles[Random.Range(0, unopenedTiles.Count)].Click();
                lastActTime = Time.time;
            }

            yield return new WaitForSeconds(actionDelta);
        }
    }
}
