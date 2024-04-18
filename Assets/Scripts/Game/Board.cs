using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelector))]
public class Board : MonoBehaviour {

    public const int BOARD_SIZE = 8;

    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;

    private Piece[,] grid;
    private Piece selectedPiece;
    private GameController gameController;
    private SquareSelector squareSelector;

    private void Awake() {
        CreateGrid();
    }

    public void SetDependencies(GameController gameController) {
        squareSelector = GetComponent<SquareSelector>();
        this.gameController = gameController;
    }

    private void CreateGrid() {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public Piece GetPieceOnSquare(Vector2Int coords) {
        if (CheckIfCoordinatesAreOnBoard(coords))
            return grid[coords.x, coords.y];
        return null;
    }

    public bool CheckIfCoordinatesAreOnBoard(Vector2Int coords) {
        bool on_board = !(coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE);
        return on_board;
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition) {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE / 2;
        return new Vector2Int(x, y);
    }

    public void OnSquareSelected(Vector3 inputPosition) {
        /* Return if Game is Not in Progress */
        if (!gameController.IsGameInProgress()) {
            return;
        }

        /* Select a Piece Based on the Chosen Square */
        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);
        if (selectedPiece) {
            if (piece != null && selectedPiece == piece) { // Deselect piece on double click.
                selectedPiece.DropPiece();
                DeselectPiece();
            }
            else if (piece != null && selectedPiece != piece && gameController.IsTeamTurnActive(piece.team)) // Select new piece if on same team.
                SelectPiece(piece);
            else if (selectedPiece.CanMoveTo(coords)) // Move selected piece to chosen square if reachable from current position.
                OnSelectedPieceMoved(coords, selectedPiece);
        }
        else {
            if (piece != null && gameController.IsTeamTurnActive(piece.team)) // Select piece if on same team and no piece is already chosen.
                SelectPiece(piece);
        }
    }

    private void SelectPiece(Piece piece) {
        selectedPiece = piece;
        selectedPiece.LiftPiece();
        List<Vector2Int> selection = selectedPiece.availableMoves;
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection) {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++) {
            Vector3 position = CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
            squaresData.Add(position, isSquareFree);
        }
        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectPiece() {
        selectedPiece = null;
        squareSelector.ClearSelection();
    }

    private void OnSelectedPieceMoved(Vector2Int coords, Piece piece) {
        TryToTakeOppositePiece(coords);
        UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
        selectedPiece.MovePiece(coords);
        DeselectPiece();
        EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece) {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
    }

    private void EndTurn() {
        gameController.EndTurn();
    }

    internal Vector3 CalculatePositionFromCoords(Vector2Int coords) {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    public bool HasPiece(Piece piece) {
        for (int i = 0; i < BOARD_SIZE; i++) {
            for (int j = 0; j < BOARD_SIZE; j++) {
                if (grid[i, j] == piece)
                    return true;
            }
        }
        return false;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece) {
        if (CheckIfCoordinatesAreOnBoard(coords))
            grid[coords.x, coords.y] = piece;
    }

    private void TryToTakeOppositePiece(Vector2Int coords) {
        Piece piece = GetPieceOnSquare(coords);
        if (piece && !selectedPiece.IsFromSameTeam(piece)) {
            TakePiece(piece);
        }
    }

    private void TakePiece(Piece piece) {
        if (piece) {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            gameController.OnPieceRemoved(piece);
            Destroy(piece.gameObject);
        }
    }

    public void OnGameRestarted() {
        selectedPiece = null;
        CreateGrid();
    }
}
