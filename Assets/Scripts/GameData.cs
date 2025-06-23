using UnityEngine;
using System.Collections.Generic;


public static class GameData
{
    /**This class is used to store the game data.
     * It is a static class, so it can be accessed from anywhere in the code and anytime in the game by the game managers.
     * It contains the following data:
     * - isUnlocked: a list of strings that contains the names of the troops that are unlocked
     * - fluctScore: the score of the Honor forces
     * - fusScore: the score of the Odium forces
     * - winScore: the score needed to win the game
     */
    public static List<string> isUnlocked = new List<string>();
    public static int fluctScore = 0;
    public static int fusScore = 0;
    public static int winScore = 2;
}