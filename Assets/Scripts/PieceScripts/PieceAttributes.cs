///This class will be used to store the attributes of a piece
/// An assignment could be to analyze the code and put all pieces attributes (like isFluct, isSpecial, etc.) in this class
using UnityEngine;
using System.Collections.Generic;

class PieceAttributes : MonoBehaviour
{
    /// This class will holds the attributes of a piece.
    public string pieceName;
    internal bool isMinion=false;
    internal GameObject summoner;

    public void setSummoner(GameObject summoner)
    {
        /** * This method sets the summoner of the piece if its a minion.
         * It is used to keep track of the piece that summoned this piece.
         */
        this.summoner = new();
        if (!isMinion) return; // If the piece is not a minion, do nothing
        this.summoner = summoner;
    }
}