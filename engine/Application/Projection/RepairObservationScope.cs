using System;
using Game.Application.CommandPipeline;
using Game.Foundation.Identity;

namespace Game.Application.Projection
{
    /// <summary>Trusted fixture composition grants direct observations per field. Ownership alone grants nothing.</summary>
    public sealed class RepairObservationScope
    {
        internal ExecutionPrincipal Principal { get; }
        internal EntityId<Item> BoatId { get; }
        internal EntityId<Lot> MaterialLotId { get; }
        internal EntityId<Item> ToolId { get; }
        internal EntityId<Settlement> Worksite { get; }
        internal string ActorLabel { get; }
        internal string LocationLabel { get; }
        internal RepairObservationFields Fields { get; }
        public RepairObservationScope(ExecutionPrincipal principal, EntityId<Item> boatId, EntityId<Lot> materialLotId,
            EntityId<Item> toolId, EntityId<Settlement> worksite, string actorLabel, string locationLabel,
            RepairObservationFields fields)
        {
            Principal = principal ?? throw new ArgumentNullException(nameof(principal));
            BoatId = boatId ?? throw new ArgumentNullException(nameof(boatId));
            MaterialLotId = materialLotId ?? throw new ArgumentNullException(nameof(materialLotId));
            ToolId = toolId ?? throw new ArgumentNullException(nameof(toolId));
            Worksite = worksite ?? throw new ArgumentNullException(nameof(worksite));
            if (principal.Source != CommandSource.Player || string.IsNullOrWhiteSpace(actorLabel) || string.IsNullOrWhiteSpace(locationLabel) ||
                actorLabel.Length > 128 || locationLabel.Length > 128 || (fields & ~RepairObservationFields.All) != 0 ||
                boatId.Value.World != principal.ActorId.Value.World || materialLotId.Value.World != principal.ActorId.Value.World ||
                toolId.Value.World != principal.ActorId.Value.World || worksite.Value.World != principal.ActorId.Value.World || boatId.Equals(toolId))
                throw new ArgumentException("Invalid repair observation scope");
            ActorLabel = actorLabel; LocationLabel = locationLabel; Fields = fields;
        }
        internal bool Grants(RepairObservationFields field) => (Fields & field) == field;
    }
}
