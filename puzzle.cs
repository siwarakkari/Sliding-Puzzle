 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzle : MonoBehaviour
{
   public Texture2D image ;
   public int blocksPerLigne =4;
   public int shuffleLength = 20 ;
   public float defauleMoveDuration = .2f;
   public float shuffleMoveDuration = .1f;
   public GameObject inGameMenu;
    public GameObject winGameMenu;
   public GameObject startButton;
   public GameObject swapButton;
   public Buttons GiveUpButton;
   public bool DeveloperMode;
   enum PuzzleState{ Solved , Shuffling , Inplay}; // scalaire enumeree indique l etat du puzzle
   
   PuzzleState state;
    Block emptyBlock ;
    Block[,] blocks;
    Block bloc;
    
    
   
   Queue<Block> inputs ; //inputs file de block 
   bool blockIsMoving;
   int shuffleMovesRemaining;
   Vector2Int prevShuffleOffset;
   [HideInInspector]
   public static GameStatus game_status=new GameStatus();
   [HideInInspector]
   public enum ReplaceElement
   {
       first,
       second,
       finished
   };
   //Replacing the puzzle 
   public static Vector3 pos1,pos2;//positing of the puzzle
   public static string element1_name,element2_name;
   public static ReplaceElement replace_element;
void Start()
    { 

        CreatePuzzle();
       
        inGameMenu.SetActive(false);
        winGameMenu.SetActive(false);
        startButton.SetActive(true);
        replace_element=ReplaceElement.first;
        swapButton.SetActive(true);
        element1_name=null;
        element2_name=null;
    }
    
void Update()
    {
    switch(game_status.status)
        {
            case GameStatus.GameStat.start_pressed:           
               game_status.status=GameStatus.GameStat.play;
                        break;
            case GameStatus.GameStat.play:
               // appel au methode startshufflemove qui permet de randomize les blocks 
                 // appel au methode startshufflemove qui permet de randomize les blocks 
                
                if( Input.GetKeyDown(KeyCode.Space) && state ==PuzzleState.Solved)// appel au methode startshufflemove qui permet de randomize les blocks 
                 {
                     StartShuffleMove();
                 }
                 if(GiveUpButton.clicked)
                 {
                         CreatePuzzle();
                         winGameMenu.SetActive(true);

                   }
                 
                 break;
            case GameStatus.GameStat.inGameMenu:
               inGameMenu.SetActive(true);
               startButton.SetActive(false);
                  break;      
            case GameStatus.GameStat.resume:
               inGameMenu.SetActive(false);
               game_status.status=GameStatus.GameStat.play;
                  break;     
            case GameStatus.GameStat.win:  
                if(DeveloperMode)
                {
                    SetPuzzleOneTheStartPosition(false);}
                 else
                 {
                    SetPuzzleOneTheStartPosition(GiveUpButton.clicked);
               }
                
               winGameMenu.SetActive(true);
                 break; 
            
            case GameStatus.GameStat.replace_Puzzle:
              Debug.Log("Element1" + element1_name);
              Debug.Log("Element2" + element2_name);
              Debug.Log("Element1 pos" + pos1);
              Debug.Log("Element2 pos" + pos2);          
             if(element1_name!=null && element2_name!=null)
             {
                 StartCoroutine(SwapPuzzle(1.0f));
                 swapButton.SetActive(false);
                 game_status.status=GameStatus.GameStat.play;
             }
              break;              
        }
 }
IEnumerator SwapPuzzle(float delay)
   {
       yield return StartCoroutine(Wait(delay));
          foreach (Block b in blocks)
      {
          if (b.GetComponent<Renderer>().name == element1_name)
          {
              StartCoroutine(MoveSwapPuzzle(pos2,b));
              b.GetComponent<Renderer>().material.color = Color.white;
          }
           if (b.GetComponent<Renderer>().name == element2_name)
          {
              StartCoroutine(MoveSwapPuzzle(pos1,b));
              b.GetComponent<Renderer>().material.color = Color.white;
          }
      } 
             game_status.status=GameStatus.GameStat.play;
   }
IEnumerator Wait( float durat) 
   {
       for(float timer=0 ;timer<durat ;timer+=Time.deltaTime)
       {
           yield return 0;
       }
   } 
IEnumerator MoveSwapPuzzle(Vector3 target ,Block b)
   {
       float accuracy =0.00001f;
       while(Vector3.Distance(b.transform.position,target)> accuracy)
       {
           b.transform.position= Vector3.MoveTowards(b.transform.position,target,6.0f*Time.deltaTime);
           yield return 0;
       }
   } 

void CreatePuzzle() // creation du puzzle
    {   blocks = new Block[blocksPerLigne,blocksPerLigne];    
         Texture2D[,] ImageSlices = ImageSlicer.GetSlices(image,blocksPerLigne);
        int m=0;
       for ( int y =0 ; y < blocksPerLigne ; y++)    //parcours des block 
       {
           for( int x =0; x < blocksPerLigne ;x++ )
           {
               
               GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Quad); // creation de la grille
                blockObject.name = blockObject.name + m ;
                m=m+1;
               blockObject.transform.position= -Vector2.one * (blocksPerLigne -1) * .5f + new Vector2(x ,y); // subdivision
               blockObject.transform.parent = transform;
               
               Block block = blockObject.AddComponent<Block>()  ;  //
               block.OnBlockPressed += PlayerMoveBlockInput; // deplacer le block vers l espace vide
               block.OnFinishedMoving+= OnBlockFinishedMoving;
               block.Init(new Vector2Int(x,y) , ImageSlices[x,y]); // l insertion de l image
               blocks[x,y]=block;
              
               if(y==0 && x== blocksPerLigne -1 ) // pour le block tout droit
               {
                   emptyBlock = block; // on le remplace par un espace vide
               }

           }
       }

     Camera.main.orthographicSize = blocksPerLigne * .55f ; // ajuster le puzzle dans l ecran
     inputs = new Queue<Block>(); // creation d une file de block
    }
void PlayerMoveBlockInput (Block blockToMove) // methode pour deplacer les blocks
    {
        if(state== PuzzleState.Inplay)
        {
          inputs.Enqueue(blockToMove);
          MakeNextPlayerMove();
         }
    }
void MakeNextPlayerMove()
    {
     while(inputs.Count >0 && !blockIsMoving)
      {
          MoveBlock(inputs.Dequeue() , defauleMoveDuration);
      }


    }
    
void MoveBlock(Block blockToMove , float duration)
   {
    if(((blockToMove.coord - emptyBlock.coord).sqrMagnitude == 1)&&(game_status.status==GameStatus.GameStat.play)) // si le block est adjascent au block vide
    {
        blocks[blockToMove.coord.x, blockToMove.coord.y]=emptyBlock;
        blocks[emptyBlock.coord.x,emptyBlock.coord.y]=blockToMove;
        Vector2Int targetCoord = emptyBlock.coord ;    // deplacer le block vers l espace vide
        emptyBlock.coord = blockToMove.coord;          //
        blockToMove.coord = targetCoord;               //

        Vector2 targetPosition = emptyBlock.transform.position;
        emptyBlock.transform.position = blockToMove.transform.position;
        blockToMove.MoveToPosition (targetPosition , duration);
        blockIsMoving=true;

    }
   }
void OnBlockFinishedMoving()
  {
      blockIsMoving=false;
      CheckIfSolved();
      if (state == PuzzleState.Inplay)
      {
      MakeNextPlayerMove();
      }
      else if(state == PuzzleState.Shuffling)
      {
      if (shuffleMovesRemaining>0)
      {
          MakeNextShuffleMove();
      }
      else{
          state = PuzzleState.Inplay;
      }
     }
  }
void StartShuffleMove() // melenger aleatoiremengt les blocks
  {
      state = PuzzleState.Shuffling; // changer l etat du puzzle stat= shuffling
      shuffleMovesRemaining = shuffleLength;
      emptyBlock.gameObject.SetActive(false);//activer le block vide
      MakeNextShuffleMove(); // appel au methode qui va faire la continuetée du melange
  }
void MakeNextShuffleMove()// dans cette methode on va prendre les mesures necessaire pour ne pas avoir un desordre total et perdre la structure du puzzle
  {
     Vector2Int[] offsets = { new Vector2Int(1 , 0) , new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1)};
    int randomIndex = Random.Range(0, offsets.Length);
    for(int i=0 ; i<offsets.Length ; i++)
    {
        Vector2Int offset = offsets[(randomIndex + i )% offsets.Length];
        if (offset != prevShuffleOffset * -1)
        {
           Vector2Int moveBlockCoord = emptyBlock.coord + offset;
           if(moveBlockCoord.x >= 0 && moveBlockCoord.x < blocksPerLigne && moveBlockCoord.y >=0 && moveBlockCoord.y < blocksPerLigne ) 
              {
               MoveBlock(blocks [moveBlockCoord.x, moveBlockCoord.y] ,shuffleMoveDuration);
                shuffleMovesRemaining--;
                prevShuffleOffset = offset;
                break;
            
                }  
        }
    
     } 
  }   
void CheckIfSolved() // make sure that the puzzle is solved and complet the empty space 
  {
      foreach (Block block in blocks)
      {
          if (!block.IsAtStartingCoord())
          {
              return  ;
          }
             
      }
       state= PuzzleState.Solved;
      emptyBlock.gameObject.SetActive(true);
      game_status.status = GameStatus.GameStat.win;
     
  }

void SetPuzzleOneTheStartPosition(bool giveUp)
  {
     StartCoroutine(MoveIntoPosition(0.2f, giveUp));
     GiveUpButton.clicked=false;
     game_status.status=GameStatus.GameStat.start;

  }
IEnumerator MoveIntoPosition (float delayTime  ,bool giveUp)
 {
    for ( int y =0 ; y < blocksPerLigne ; y++)    //parcours des block 
       {
           for( int x =0; x < blocksPerLigne ; x++ )
           { 
               for (float timer =0 ; timer < delayTime ;timer+= Time.deltaTime)
               {
                   blocks[x,y].transform.localScale =new Vector3(blocks[x,y].scale_backup.x+blocks[x,y].scale_x,blocks[x,y].scale_backup.y+blocks[x,y].scale_y, blocks[x,y].transform.localScale.z);
                   yield return 0;
               }
            }
        }  
   winGameMenu.SetActive(true);
   if(!giveUp)
   {
     //save
   }
  
  }
}