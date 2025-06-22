using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapParameters", menuName = "Game/MapParameters", order = 1)]
public class MapParameters : ScriptableObject
{
    public List<int> summoningTurns = new List<int> {1,2,3,4,5,6,7};
    public List<int> specialSummoningTurns = new List<int> {1,2,3,4,5,6,7};
    public int numberOfSummon=1;

    public List<string> unlockedTroops = new List<string>();
}
