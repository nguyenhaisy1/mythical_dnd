using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Kernel;
using Game.Foundation.Identity;

namespace Game.Infrastructure.Diagnostics
{
    public sealed class RuleEvaluationTrace
    {
        public ulong BeforeRevision => Input.Revision.Value;
        public string Issuer { get; }
        public string CommandType => Command.CommandType;
        public int PayloadVersion => Command.PayloadVersion;
        public WorldSnapshot Input { get; }
        public IKernelCommand Command { get; }
        public RuleResult Result { get; }
        internal RuleEvaluationTrace(WorldSnapshot input, EntityId<Actor> issuer, IKernelCommand command, RuleResult result)
        { Input = input; Issuer = issuer.ToString(); Command = command; Result = result; }
    }
    /// <summary>Records actual compiled handler evaluation, not commit outcome. No trace exists for commands rejected before rules.</summary>
    public sealed class RuleInspectionHandler : IRuleHandler
    {
        private readonly IRuleHandler inner;
        private readonly int capacity;
        private readonly List<RuleEvaluationTrace> traces = new List<RuleEvaluationTrace>();
        private readonly object sync = new object();
        public string CommandType => inner.CommandType;
        public int PayloadVersion => inner.PayloadVersion;
        public Type PayloadType => inner.PayloadType;
        public long DroppedCount { get; private set; }
        public IReadOnlyList<RuleEvaluationTrace> Traces { get { lock (sync) return Array.AsReadOnly(traces.ToArray()); } }
        public RuleInspectionHandler(IRuleHandler inner, int capacity = 1000)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            if (inner.PayloadType != typeof(TransferResource)) throw new ArgumentException("Only immutable transfer payloads are supported", nameof(inner));
            if (capacity < 1 || capacity > 100000) throw new ArgumentOutOfRangeException(nameof(capacity));
            this.capacity = capacity;
        }
        public RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command)
        {
            if (world == null || issuer == null || command == null) throw new ArgumentNullException();
            if (!(command is TransferResource)) throw new ArgumentException("Unsupported trace payload", nameof(command));
            var result = inner.Evaluate(world, issuer, command) ?? throw new InvalidOperationException("Handler returned null");
            lock (sync)
            {
                if (traces.Count < capacity) traces.Add(new RuleEvaluationTrace(world, issuer, command, result));
                else if (DroppedCount < long.MaxValue) DroppedCount++;
            }
            return result;
        }
    }
}
