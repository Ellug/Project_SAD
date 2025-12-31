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
    public TriggeredBuff[] buffs;
    public string Description
    {
        get
        {
            string modsText  = PerkText.Build(mods);
            string buffsText = PerkText.Build(buffs);

            if (string.IsNullOrEmpty(modsText))
                return buffsText;

            if (string.IsNullOrEmpty(buffsText))
                return modsText;

            return $"{modsText}\n\n{buffsText}";
        }
    }
}
