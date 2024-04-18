using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
	public TeamColor team { get; set; }
	public Board board { get; set; }
	public List<Piece> activePieces { get; private set; }

	public Player(TeamColor team, Board board) {
		activePieces = new List<Piece>();
		this.board = board;
		this.team = team;
	}

	public void AddPiece(Piece piece) {
		if (!activePieces.Contains(piece))
			activePieces.Add(piece);
	}

	public void RemovePiece(Piece piece) {
		if (activePieces.Contains(piece))
			activePieces.Remove(piece);
	}

	public void GenerateAllPossibleMoves() {
		foreach (var piece in activePieces) {
			if (board.HasPiece(piece))
				piece.SelectAvaliableSquares();
		}
	}
	public Piece[] GetPieceAtackingOppositePiceOfType<T>() where T : Piece {
		return activePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
	}

	public Piece[] GetPiecesOfType<T>() where T : Piece {
		return activePieces.Where(p => p is T).ToArray();
	}

	internal void OnGameRestarted() {
		activePieces.Clear();
	}
}
