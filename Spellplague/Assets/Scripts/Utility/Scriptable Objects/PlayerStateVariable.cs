using UnityEngine;

namespace Spellplague.Utility
{
    public enum PlayerStance
    {
        Upright,
        Crouch,
        Jump
    }

    public enum PlayerMove
    {
        Still,
        Walk,
        Sprint
    }

    public enum PlayerSpecialState
    {
        None,
        Inspecting
    }

    public enum PlayerCombatState
    {
        None,
        Attacking,
        Hit
    }

    public enum InventoryState
    {
        Closed,
        Open
    }

    [CreateAssetMenu(fileName = "Player State Asset", menuName = "ScriptableObjects/Variables/New Player State Asset", order = 2)]
    public class PlayerStateVariable : ScriptableObject
    {
        private PlayerStance currentPlayerStance;
        public PlayerStance CurrentPlayerStance
        {
            get { return currentPlayerStance; }
            set
            {
                PlayerStateChangedEvent?.Invoke(value);
                currentPlayerStance = value;
            }
        }
        public delegate void PlayerStateChanged(PlayerStance value);
        public event PlayerStateChanged PlayerStateChangedEvent;

        public PlayerMove CurrentPlayerMoveState { get; set; }
        public PlayerSpecialState CurrentPlayerSpecialState { get; set; }
        public PlayerCombatState CurrentPlayerCombatState { get; set; }
        public InventoryState CurrentInventoryState { get; set; }

        private void OnEnable()
        {
            CurrentPlayerStance = PlayerStance.Upright;
            CurrentPlayerMoveState = PlayerMove.Still;
            CurrentPlayerSpecialState = PlayerSpecialState.None;
            CurrentPlayerCombatState = PlayerCombatState.None;
            CurrentInventoryState = InventoryState.Closed;
        }
    }
}