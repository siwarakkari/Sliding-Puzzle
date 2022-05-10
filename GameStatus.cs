using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
   public enum GameStat{
       
       start,
       start_pressed,
       play,
       inGameMenu,
       resume,
       win,
       replace_Puzzle,
   }
   public GameStat status;
 public GameStatus()
   {
       status=GameStat.start;
   }
     
}
