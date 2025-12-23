using System;

[Serializable]
public class TriggeredBuff
{
    public PerkTrigger trigger;
    public float duration = 3f;

    // 버프 동안 적용될 StatMod
    public StatMod[] mods;
}
