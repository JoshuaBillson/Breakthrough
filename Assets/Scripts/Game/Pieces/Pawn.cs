using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

    public override List<Vector2Int> SelectAvaliableSquares()
    {
        availableMoves.Clear();

        /* Add Vertical Move */
        Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        Vector2Int nextCoords = occupiedSquare + direction;
        if (board.CheckIfCoordinatesAreOnBoard(nextCoords) && (board.GetPieceOnSquare(nextCoords) == null)) {
            TryToAddMove(nextCoords);
        }

        /* Add Diagonal Moves */
        Vector2Int diag_1 = new Vector2Int(-1, direction.y);
        Vector2Int diag_2 = new Vector2Int(1, direction.y);
        Vector2Int nextCoords_1 = occupiedSquare + diag_1;
        Vector2Int nextCoords_2 = occupiedSquare + diag_2;
        if (board.CheckIfCoordinatesAreOnBoard(nextCoords_1)) {
            Piece piece = board.GetPieceOnSquare(nextCoords_1);
            if (piece != null && !piece.IsFromSameTeam(this)) {
                TryToAddMove(nextCoords_1);
            } else if (piece == null) {
                TryToAddMove(nextCoords_1);
            }
        }
        if (board.CheckIfCoordinatesAreOnBoard(nextCoords_2)) {
            Piece piece = board.GetPieceOnSquare(nextCoords_2);
            if (piece != null && !piece.IsFromSameTeam(this)) {
                TryToAddMove(nextCoords_2);
            } else if (piece == null) {
                TryToAddMove(nextCoords_2);
            }
        }

        return availableMoves;
    }

    public override void MovePiece(Vector2Int coords) {
        base.MovePiece(coords);
    }
}
