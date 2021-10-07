using MediatR;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NexusMods.Monitor.Shared.Domain.SeedWork
{
    public abstract record Entity(uint Id) : IComparable<Entity>
    {
        public bool IsTransient => Id == default;

        private readonly List<INotification> _domainEvents = new();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);
        public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);
        public void ClearDomainEvents() => _domainEvents.Clear();


        [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
        // https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode?view=net-5.0
        public override int GetHashCode() => !IsTransient ? HashCode.Combine(Id) : base.GetHashCode();

        public virtual bool Equals(Entity? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return !other.IsTransient && !IsTransient && other.Id == Id;
        }

        public int CompareTo(Entity? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Id.CompareTo(other.Id);
        }
    }
}