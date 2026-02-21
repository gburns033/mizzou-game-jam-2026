using UnityEngine;

namespace Game.Mobs.Targeting
{
    public interface ITargetProvider
    {
        Transform GetTarget();
    }
}