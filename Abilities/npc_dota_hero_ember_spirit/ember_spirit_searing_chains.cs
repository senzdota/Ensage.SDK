﻿// <copyright file="ember_spirit_searing_chains.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_ember_spirit
{
    using System.Linq;

    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;

    public class ember_spirit_searing_chains : ActiveAbility, IAreaOfEffectAbility, IHasDot
    {
        public ember_spirit_searing_chains(Ability ability)
            : base(ability)
        {
        }

        public float Duration
        {
            get
            {
                var duration = this.Ability.GetAbilitySpecialData("duration");

                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_ember_spirit_2);
                if (talent?.Level > 0)
                {
                    duration += talent.GetAbilitySpecialData("value");
                }

                return duration;
            }
        }

        public bool HasInitialDamage { get; } = false;

        public float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("radius");
            }
        }

        public float RawTickDamage
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("chains_damage");
            }
        }

        public string TargetModifierName { get; } = "modifier_ember_spirit_searing_chains";

        public float TickRate
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("tick_interval");
            }
        }

        public override bool CanHit(params Unit[] targets)
        {
            if (!targets.Any())
            {
                return true;
            }

            return this.Owner.Distance2D(targets.First()) < this.Radius;
        }

        public float GetTickDamage(params Unit[] targets)
        {
            var damage = this.RawTickDamage;
            var amplify = this.Ability.SpellAmplification();
            var maxTargets = (int)this.Ability.GetAbilitySpecialData("unit_count");

            var totalDamage = 0.0f;
            foreach (var target in targets.Take(maxTargets))
            {
                var reduction = this.Ability.GetDamageReduction(target, this.DamageType);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage;
        }

        public float GetTotalDamage(params Unit[] targets)
        {
            return this.GetTickDamage(targets) * (this.Duration / this.TickRate);
        }
    }
}