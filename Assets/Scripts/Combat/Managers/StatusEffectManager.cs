using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance;

    public void Init()
    {
        Instance = this;
    }

    public bool UnitHasStatuseffect(Unit unit, StatusEfectEnum statusEfect)
    {
        return unit.statusEffects.Find(x => x.statusEfectType == statusEfect) != null;
    }

    public void Blinded(Unit unit, DamageData data)
    {
        var blinded = UnitHasStatuseffect(unit,  StatusEfectEnum.Blinded);

        if (data.MagicalDamage)
            return;

        var succes = Random.Range(0f, 1f) > 0.5f;
        if (succes)
        {
            data.Damage = 0;
        }
    }

    public bool CantTakeTurn(Unit unit)
    {
        var incapacitated = UnitHasStatuseffect(unit, StatusEfectEnum.Incapacitated);
        var stunned = UnitHasStatuseffect(unit, StatusEfectEnum.Blinded);

        return incapacitated || stunned;
    }

    public List<DoTStatusEffect> GetDoTEffects(Unit unit)
    {
        return unit.statusEffects.OfType<DoTStatusEffect>().ToList();
    }
}
