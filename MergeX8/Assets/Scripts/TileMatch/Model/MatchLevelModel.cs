using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus.Config.TileMatch;
using DragonU3DSDK.Storage;
using SomeWhereTileMatch;
using TileMatch.Game;
using UnityEngine;

public class TileMatchLevelModel : Singleton<TileMatchLevelModel>
{

   private int _collectCoin = 0;
   public int CollectCoin
   {
      get
      {
         return _collectCoin;
      }
      set { _collectCoin = value; }
   }
}
   