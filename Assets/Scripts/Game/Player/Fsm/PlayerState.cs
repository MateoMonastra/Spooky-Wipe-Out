using FSM;
using UnityEngine;

namespace Game.Player
{
    public class PlayerState : State
    {
        protected GameObject player;

        protected PlayerState(GameObject player)
        {
            this.player = player;
        }
    }
}