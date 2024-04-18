using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PiecesCreator))]
public class GameController : MonoBehaviour {

    private enum GameState {
        Init, Play, Finished
    }

    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private GameUIManager UIManager;

    private PiecesCreator pieceCreator;
    private Player whitePlayer;
    private Player blackPlayer;
    private Player activePlayer;

    private GameState state;

    private void Awake() {
        SetDependencies();
        CreatePlayers();
    }

    private void SetDependencies() {
        pieceCreator = GetComponent<PiecesCreator>();
    }

    private void CreatePlayers() {
        whitePlayer = new Player(TeamColor.White, board);
        blackPlayer = new Player(TeamColor.Black, board);
    }

    void Start() {
        StartNewGame();
    }

    private void StartNewGame() {
        UIManager.HideUI();
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
    }

    private void SetGameState(GameState state) {
        this.state = state;
    }

    public bool IsGameInProgress() {
        return state == GameState.Play;
    }

    private void CreatePiecesFromLayout(BoardLayout layout) {
        for (int i = 0; i < layout.GetPiecesCount(); i++) {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    private void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type) {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        Player currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    private void GenerateAllPossiblePlayerMoves(Player player) {
        player.GenerateAllPossibleMoves();
    }

    public bool IsTeamTurnActive(TeamColor team) {
        return activePlayer.team == team;
    }

    public void EndTurn() {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished()) {
            EndGame();
        }
        else {
            ChangeActiveTeam();
        }
    }

    private bool CheckIfGameIsFinished() {
        /* Check if any Black Pieces are in White's Home Row */
        for (int x = 0; x < Board.BOARD_SIZE; x++) {
            Piece piece = board.GetPieceOnSquare(new Vector2Int(x, 0));
            if (piece != null && piece.team == TeamColor.Black) {
                return true;
            }
        }

        /* Check if any White Pieces are in Blacks's Home Row */
        for (int x = 0; x < Board.BOARD_SIZE; x++) {
            Piece piece = board.GetPieceOnSquare(new Vector2Int(x, 7));
            if (piece != null && piece.team == TeamColor.White) {
                return true;
            }
        }

        /* Game is Not Over */
        return false;
    }

    private void EndGame() {
        SetGameState(GameState.Finished);
        Debug.Log("Game is Over!");
        UIManager.OnGameFinished(activePlayer.team.ToString());
    }

    public void RestartGame() {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces() {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void ChangeActiveTeam() {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    private Player GetOpponentToPlayer(Player player) {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    internal void OnPieceRemoved(Piece piece) {
        Player pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
