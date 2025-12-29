using TMPro;
using UnityEngine;

public class PerksStatusUI : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private PlayerModel _player;

    [Header("Combat UI (Base / Final)")]
    [SerializeField] private TMP_Text _damageBaseText;
    [SerializeField] private TMP_Text _damageFinalText;

    [SerializeField] private TMP_Text _atkSpeedBaseText;
    [SerializeField] private TMP_Text _atkSpeedFinalText;

    [SerializeField] private TMP_Text _projCountBaseText;
    [SerializeField] private TMP_Text _projCountFinalText;

    [Header("Passive (Triggered Buff) Texts (Max 2)")]
    [SerializeField] private TMP_Text _passive1Text;
    [SerializeField] private TMP_Text _passive2Text;

    [Header("Options")]
    [SerializeField] private float _refreshInterval = 0.2f;

    private PlayerStatsContext _ctx;
    private float _acc;

    private void Awake()
    {
        if (_player == null)
            _player = FindFirstObjectByType<PlayerModel>();

        if (_player != null)
        {
            _ctx = _player.GetComponent<PlayerStatsContext>();
            if (_ctx != null)
                _ctx.OnChanged += RefreshNow;
        }

        RefreshNow();
    }

    private void OnDestroy()
    {
        if (_ctx != null)
            _ctx.OnChanged -= RefreshNow;
    }

    private void Update()
    {
        _acc += Time.unscaledDeltaTime;
        if (_refreshInterval <= 0f || _acc >= _refreshInterval)
        {
            _acc = 0f;
            RefreshNow();
        }
    }

    private void RefreshNow()
    {
        if (_player == null)
        {
            SetCombatEmpty();
            SetPassiveEmpty();
            return;
        }

        WeaponBase weapon = _player.CurrentWeapon;
        if (weapon == null || weapon.WeaponData == null)
        {
            SetCombatEmpty();
            SetPassiveEmpty();
            return;
        }

        // Combat
        WeaponRuntimeStats wBase = WeaponRuntimeStats.FromData(weapon.WeaponData);
        WeaponRuntimeStats wFinal = _player.FinalStats.Weapon;

        SetFloatPair(_damageBaseText, _damageFinalText, wBase.Attack, wFinal.Attack);
        SetFloatPair(_atkSpeedBaseText, _atkSpeedFinalText, wBase.AttackSpeed, wFinal.AttackSpeed);
        SetIntPair(_projCountBaseText, _projCountFinalText, wBase.ProjectileCount, wFinal.ProjectileCount);

        // Passive = TriggeredBuff (max 2)
        FillPassiveTexts(weapon.PerksTree);
    }

    private void FillPassiveTexts(PerksTree tree)
    {
        SetPassiveEmpty();
        if (tree == null) return;

        int count = 0;

        for (int stage = 0; stage < tree.StageCount; stage++)
        {
            int side = tree.GetSelectedSide(stage);
            if (side < 0) continue;

            var node = tree.GetNode(stage, side);
            if (node == null || node.buffs == null) continue;

            for (int i = 0; i < node.buffs.Length; i++)
            {
                var buff = node.buffs[i];
                if (buff == null) continue;

                string line = SingleLine(PerkText.BuildOneLine(buff));

                if (count == 0) SetText(_passive1Text, line);
                else if (count == 1) SetText(_passive2Text, line);
                else return; // 최대 2개

                count++;
            }
        }
    }

    // UI helpers
    private void SetCombatEmpty()
    {
        SetText(_damageBaseText, "-");
        SetText(_damageFinalText, "-");

        SetText(_atkSpeedBaseText, "-");
        SetText(_atkSpeedFinalText, "-");

        SetText(_projCountBaseText, "-");
        SetText(_projCountFinalText, "-");
    }

    private void SetPassiveEmpty()
    {
        SetText(_passive1Text, "");
        SetText(_passive2Text, "");
    }

    private void SetFloatPair(TMP_Text baseText, TMP_Text finalText, float b, float f)
    {
        SetText(baseText, Fmt(b));
        SetText(finalText, Fmt(f));
    }

    private void SetIntPair(TMP_Text baseText, TMP_Text finalText, int b, int f)
    {
        SetText(baseText, b.ToString());
        SetText(finalText, f.ToString());
    }

    private void SetText(TMP_Text t, string s)
    {
        if (t != null) t.text = s;
    }

    // formatting
    private static string SingleLine(string s)
    {
        if (string.IsNullOrEmpty(s)) return "-";

        s = s.Replace('\r', ' ')
             .Replace('\n', ' ')
             .Replace('\t', ' ');

        while (s.Contains("  "))
            s = s.Replace("  ", " ");

        return s.Trim();
    }

    private static string Fmt(float v) => v.ToString("0.##");
}
