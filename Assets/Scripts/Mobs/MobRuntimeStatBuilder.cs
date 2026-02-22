using Game.Mobs;

public static class MobRuntimeStatBuilder
{
    public static MobRuntimeStats Build(MobStatsSO baseStats, MobWaveStatsSO scaling, int round)
    {
        var r = new MobRuntimeStats();

        float hpMult = scaling.HpMult(round);
        float dmgMult = scaling.DamageMult(round);
        float spdMult = scaling.SpeedMult(round);

        // ✅ use YOUR MobStatsSO fields:
        r.maxHP     = baseStats.maxHealth * hpMult;
        r.damage    = baseStats.attackDamage * dmgMult;
        r.moveSpeed = baseStats.moveSpeed * spdMult;

        return r;
    }
}