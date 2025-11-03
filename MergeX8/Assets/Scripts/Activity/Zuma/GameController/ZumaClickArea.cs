using System;
using UnityEngine;
using UnityEngine.UI;

public class ZumaClickArea:MonoBehaviour
{
    public ZumaGameController Game;
    public ClickArea Area;
    public Image Image;
        
    private void Awake()
    {
        Image = gameObject.AddComponent<Image>();
        Image.raycastTarget = true;
        Image.color = new Color(0, 0, 0, 0);
        Area = gameObject.AddComponent<ClickArea>();
    }

    public void BindGame(ZumaGameController game)
    {
        Game = game;
        Area.OnUp(Game.OnClick);
        Area.OnDown(Game.OnDrag);
        Area.OnDrag(Game.OnDrag);
    }
}