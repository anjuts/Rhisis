﻿using System;
using System.Linq.Expressions;
using Rhisis.Core.IO;
using Rhisis.Core.Network.Packets.World;
using Rhisis.World.Core.Systems;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Interfaces;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Events.Statistics;

namespace Rhisis.World.Systems
{
    [System]
    public class StatisticsSystem : NotifiableSystemBase
    {

        /// <summary>
        /// Gets the <see cref="InventorySystem"/> match filter.
        /// </summary>
        protected override Expression<Func<IEntity, bool>> Filter => x => x.Type == WorldEntityType.Player;

        public StatisticsSystem(IContext context) :
            base(context)
        {
        }

        /// <summary>
        /// Executes the <see cref="StatisticsSystem"/> logic.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="e"></param>
        public override void Execute(IEntity entity, EventArgs e)
        {
            if (!(e is StatisticsEventArgs statisticsEvent))
                return;

            var playerEntity = entity as IPlayerEntity;

            Logger.Debug("Execute statistics action: {0}", statisticsEvent.ActionType.ToString());

            switch (statisticsEvent.ActionType)
            {
                case StatisticsActionType.ModifyStatus:
                    this.ModifyStatus(playerEntity, statisticsEvent.Arguments);
                    break;
                case StatisticsActionType.Unknown:
                    // Nothing to do.
                    break;
                default:
                    Logger.Warning("Unknown statistics action type: {0} for player {1} ",
                        statisticsEvent.ActionType.ToString(), entity.ObjectComponent.Name);
                    break;
            }

            WorldPacketFactory.SendUpdateState(playerEntity);
        }

        private void ModifyStatus(IPlayerEntity player, object[] args)
        {
            Logger.Debug("Modify sttus");
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (args.Length < 0)
                throw new ArgumentException("Statistics event arguments cannot be empty.", nameof(args));
            if (!(args[0] is ModifyStatusPacket msPacket))
                throw new ArgumentException("Statistics event arguments can only be a ModifyStatusPacket.",
                    nameof(args));

            var total = msPacket.StrenghtCount + msPacket.StaminaCount + msPacket.DexterityCount +
                        msPacket.IntelligenceCount;

            var statsPoints = player.StatisticsComponent.StatPoints;
            if (statsPoints <= 0 || total > statsPoints)
            {
                Logger.Error("No statspoints available, but trying to upgrade {0}.", player.ObjectComponent.Name);
                return;
            }

            if (msPacket.StrenghtCount > statsPoints || msPacket.StaminaCount > statsPoints ||
                msPacket.DexterityCount > statsPoints || msPacket.IntelligenceCount > statsPoints || total <= 0 ||
                total > ushort.MaxValue)
            {
                Logger.Error("Invalid upgrade request due to bad total calculation (trying to dupe) {0}.",
                    player.ObjectComponent.Name);
                return;
            }

            player.StatisticsComponent.Strenght += msPacket.StrenghtCount;
            player.StatisticsComponent.Stamina += msPacket.StaminaCount;
            player.StatisticsComponent.Dexterity += msPacket.DexterityCount;
            player.StatisticsComponent.Intelligence += msPacket.IntelligenceCount;
            player.StatisticsComponent.StatPoints -= (ushort) total;
        }
    }
}
