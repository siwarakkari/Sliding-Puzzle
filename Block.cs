using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Block : MonoBehaviour
{
    public event System.Action<Block> OnBlockPressed; // defenir un event 
    public event System.Action OnFinishedMoving;
    public Vector2Int coord ;
    public  Vector2Int startingCoord;
    [HideInInspector]
    public Vector3 scale_backup;
     [HideInInspector]
    public float scale_x = 0.99f;
    [HideInInspector]
    public float scale_y = 0.99f;
    [HideInInspector]
    public Vector3 winPosition;
     [HideInInspector]
     public bool replace=false;
 
 void Start()
    {
        scale_backup=transform.localScale;
        winPosition= transform.position;
        

    }
    public void Init(Vector2Int startingCoord,Texture2D image)
    {
        this.startingCoord = startingCoord;
        coord = startingCoord;
       GetComponent<MeshRenderer>().material = Resources.Load<Material>("Block");
       GetComponent<MeshRenderer>().material.mainTexture = image;

   }
public void MoveToPosition(Vector2 target, float duration)
    {
        StartCoroutine(AnimateMove (target,duration));
    }
void OnMouseDown() // programmer la click pour deplacer les blocks
    {
        
         if (OnBlockPressed != null)
        {
            OnBlockPressed(this);
        }
         if(puzzle.game_status.status== GameStatus.GameStat.replace_Puzzle)
        {
            replace = true; 
            if( puzzle.replace_element ==puzzle.ReplaceElement.second)
            {
                gameObject.GetComponent<Renderer> ().material.color = Color.green;
                puzzle.pos2 = transform.position;
                puzzle.element2_name = gameObject.name;
                puzzle.replace_element=puzzle.ReplaceElement.finished;

            }
            if( puzzle.replace_element ==puzzle.ReplaceElement.first)
            {
                gameObject.GetComponent<Renderer> ().material.color = Color.green;
                puzzle.pos1 = transform.position;
                puzzle.element1_name = gameObject.name;
                puzzle.replace_element=puzzle.ReplaceElement.second;

            }
        }
    }
IEnumerator AnimateMove(Vector2 target , float duration ) // move blocks smoothly 
    {
        Vector2 InitilPos = transform.position;
        float percent = 0;
        while(percent<1)
        {
            percent+= Time.deltaTime /duration;
            transform.position = Vector2.Lerp(InitilPos,target,percent);
            yield return  null;
        }
        if (OnFinishedMoving != null)
        {
            OnFinishedMoving();
        }


    }

  public bool IsAtStartingCoord()
  {
      return coord== startingCoord;
  }



}





