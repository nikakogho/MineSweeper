using UnityEngine;
using UnityEngine.UI;

public class CustomSelector : MonoBehaviour
{
    public Text tBombs, tX, tY;
    public Slider sBombs, sX, sY;
    GameStarter starter;

    int lastBombAmount = 10;
    int x = 10, y = 10;

    void Start()
    {
        starter = GameStarter.instance;
        SetBombValue();
        SetX();
        SetY();
    }

    public void SetBombValue()
    {
        int amount = (int)sBombs.value;

        if(amount > x * y)
        {
            sBombs.value = lastBombAmount;
            return;
        }

        starter.custom.bombs = amount;
        tBombs.text = amount.ToString();
        lastBombAmount = amount;
    }

    public void SetX()
    {
        int amount = (int)sX.value;
        starter.custom.x = amount;
        tX.text = amount.ToString();
        x = amount;
    }

    public void SetY()
    {
        int amount = (int)sY.value;
        starter.custom.y = amount;
        tY.text = amount.ToString();
        y = amount;
    }
}
