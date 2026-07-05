using System;
using System.Reflection;
using Behaviour.Unit;
using Behaviour.Weapons;
using HarmonyLib;
using Source.Ability;
using Source.Dungeon;
using Source.Player;
using UnityEngine;

namespace VGBBoardAlways.Patches;

[HarmonyPatch]
internal static class BoardingPatches
{
    private static MethodInfo? _becameBoardable;

    private static MethodInfo BecameBoardableMi =>
        _becameBoardable ??= typeof(SpaceShip)
            .GetMethod("BecameBoardable", BindingFlags.NonPublic | BindingFlags.Instance)!;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpaceShip), "HandleBoardingCheck")]
    private static bool Prefix(
        SpaceShip __instance,
        DamageData damageData,
        ref bool __result)
    {
        if (Plugin.Instance.CfgEnabled.Value)
        {
            float maxHp = __instance.maxHullHP;
            if (maxHp > 0f)
            {
                float hpFraction = __instance.currentHullHP / maxHp;
                if (hpFraction < 0.4f &&
                    !(__instance is TargetableUnit { isDestroyed: true }))
                {
                    float empFraction = Mathf.Min(__instance.unitData.empCharge / maxHp, 1f);
                    float chance = MathF.Min(0.005f + empFraction * 0.02f, 1f);

                    BecameBoardableMi.Invoke(__instance, new object?[]
                    {
                        damageData,
                        chance,
                        hpFraction,
                        empFraction,
                    });

                    __result = true;
                    return false; // skip original
                }
            }
        }

        return true; // call original
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DungeonSimulation), "DamageFacility")]
    private static void DamageFacilityPrefix(ref float amount)
    {
        if (Plugin.Instance.CfgEnabled.Value)
        {
            amount *= Plugin.Instance.CfgIntegrityDamageMultiplier.Value;
        }
    }

    private static readonly FieldInfo _levelDefenderPowerMod = typeof(DungeonSimulation).GetField("levelDefenderPowerMod", BindingFlags.NonPublic | BindingFlags.Instance)!;
    private static readonly FieldInfo _levelDefenderHpMod = typeof(DungeonSimulation).GetField("levelDefenderHpMod", BindingFlags.NonPublic | BindingFlags.Instance)!;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DungeonSimulation), "ApplyLevelScaling")]
    private static void ApplyLevelScalingPostfix(DungeonSimulation __instance)
    {
        float mod = Plugin.Instance.CfgDifficultyModifier.Value;
        if (mod >= 0.99f || mod <= 0.01f)
            return;

        _levelDefenderPowerMod.SetValue(__instance, (float)_levelDefenderPowerMod.GetValue(__instance)! * mod);
        _levelDefenderHpMod.SetValue(__instance, (float)_levelDefenderHpMod.GetValue(__instance)! * mod);
    }

    private static float _preScuttleIntegrity;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DungeonSimulation), "TryScuttle")]
    private static void TryScuttlePrefix(DungeonSimulation __instance)
    {
        _preScuttleIntegrity = __instance.structureIntegrity;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DungeonSimulation), "TryScuttle")]
    private static void TryScuttlePostfix(DungeonSimulation __instance)
    {
        if (Plugin.Instance.CfgEnabled.Value && __instance.structureIntegrity < _preScuttleIntegrity)
        {
            float damage = _preScuttleIntegrity - __instance.structureIntegrity;
            float scaledDamage = damage * Plugin.Instance.CfgIntegrityDamageMultiplier.Value;
            __instance.structureIntegrity = _preScuttleIntegrity - scaledDamage;
        }
    }
}
