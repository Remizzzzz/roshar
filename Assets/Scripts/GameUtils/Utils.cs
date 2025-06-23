using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

namespace utils
{
    public static class Utils
    {
        /**template function made by ChatGPT. Again, it's just an util, i know how to do it, but i don't want to waste time
        This class contains utility functions that can be used throughout the development of the game.
        */
        public static List<TKey> GetKeysByValue<TKey, TValue>(Dictionary<TKey, TValue> dict, TValue value)
        {/// Returns a list of keys that have the specified value in the dictionary.
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
        { /// Returns the position of the mouse on the tilemap in tile coordinates.
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
            mousePosition.z = 0; 
            return tileMap.WorldToCell(mousePosition); // Convert into tilemap coor
        }
        public static int launchD4(int nbOfLaunches=1){ /// Simulates rolling a D4 dice <nbOfLaunches> times and returns the total.
            int total = 0;
            for (int i=0; i<nbOfLaunches;i++) {
                total+=UnityEngine.Random.Range(1,5);
            }
            return total;
        }

        public static bool isInstance<T>(object obj){ /// Checks if the object is an instance of type T.
            return obj is T;
        }
    }
}

