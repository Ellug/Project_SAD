using System;

[Serializable]
public struct StatMod
{
    public StatId stat;
    public ModOp op;
    public float value;
}

[Serializable]
public class PerksNode
{
    public StatMod[] mods;
    public string Description => PerkText.Build(mods);
}
