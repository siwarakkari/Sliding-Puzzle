using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSlicer : MonoBehaviour
{
 public static Texture2D[,] GetSlices(Texture2D image , int blocksPerLigne) // get slices (quad) and apply graphic to them
    {
        int imageSize = Mathf.Min(image.width, image.height);
         int blockSize = imageSize / blocksPerLigne;
         Texture2D[,] blocks = new Texture2D[blocksPerLigne, blocksPerLigne];
         for(int y =0; y<blocksPerLigne ; y++)
         {
             for(int x =0; x<blocksPerLigne ; x++)
             {
                 Texture2D block =new Texture2D(blockSize,blockSize);
                 block.wrapMode = TextureWrapMode.Clamp;
                 block.SetPixels(image.GetPixels(x * blockSize, y * blockSize ,blockSize ,blockSize));
                 block.Apply();
                 blocks[x,y]= block;
             }
         }
         return blocks;
    }
}