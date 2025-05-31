using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

namespace utils
{
    public static class Utils
    {
        //template function made by ChatGPT. Again, it's just an util, i know how to do it, but i don't want to waste time
        public static List<TKey> GetKeysByValue<TKey, TValue>(Dictionary<TKey, TValue> dict, TValue value)
        {
            List<TKey> keys = new List<TKey>();

            foreach (KeyValuePair<TKey, TValue> pair in dict)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, value)) //In case the types aren't supporting ==
                {
                    keys.Add(pair.Key);
                }
            }

            return keys;
        }
        public static Vector3Int getMousePositionOnTilemap(Tilemap tileMap)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
            mousePosition.z = 0; 
            return tileMap.WorldToCell(mousePosition); // Convert into tilemap coor
        }
    }
}

