using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapParameters", menuName = "Game/MapParameters", order = 1)]
public class MapParameters : ScriptableObject
{
    public List<int> summoningTurns = new List<int> {1,2,4,5,6};
    public List<int> specialSummoningTurns = new List<int> {3,7};
    public int numberOfSummon=1;
}
