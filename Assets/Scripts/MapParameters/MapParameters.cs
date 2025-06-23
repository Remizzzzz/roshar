using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapParameters", menuName = "Game/MapParameters", order = 1)]
public class MapParameters : ScriptableObject
{
    /** MapParameters is a ScriptableObject that contains the parameters of the map.
     * It is used to store the parameters of the map in a single place, so they can be easily modified.
     */
    public List<int> summoningTurns = new List<int> {1,2,3,4,5,6,7};
    public List<int> specialSummoningTurns = new List<int> {1,2,3,4,5,6,7};
    public int numberOfSummon=1; /// The number of units to summon each turn

    public List<string> unlockedTroops = new List<string>();
}
