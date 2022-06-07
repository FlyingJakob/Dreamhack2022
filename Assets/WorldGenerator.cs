using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
   public GameObject[] asteroidPrefabs;
   public float randomSizeMin;
   public float randomSizeMax;
   public int radius;
   public int asteroidAmount;
   public bool generate;
   private void Awake()
   {
      if (!generate)
      {
         return;
      }
      for (int i = 0; i < asteroidAmount; i++)
      {
         GameObject prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
         Vector3 position = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius),
            Random.Range(-radius, radius));
         Vector3 rotation = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180),
            Random.Range(-180, 180));
         GameObject obj = Instantiate(prefab, position, Quaternion.Euler(rotation));
         obj.transform.localScale *=(Random.Range(randomSizeMin, randomSizeMax));
      }
   }
}
