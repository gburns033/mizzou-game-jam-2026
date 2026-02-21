using UnityEngine;

namespace Game.Mobs.Movement
{
    public interface IMovementStrategy
    {
        Vector2 GetDesiredVelocity();
    }
}