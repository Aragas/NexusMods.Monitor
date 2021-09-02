using MediatR;

using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Shared.Domain.SeedWork
{
    public abstract class Entity : IComparable<Entity>, IEquatable<Entity>
    {
        private uint _Id;
        public virtual uint Id { get => _Id; protected set => _Id = value; }

        public bool IsTransient => Id == default;

        private readonly List<INotification> _domainEvents = new();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);
        public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);
        public void ClearDomainEvents() => _domainEvents.Clear();


        public override int GetHashCode() => !IsTransient ? HashCode.Combine(Id) : base.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity) obj);
        }

        public bool Equals(Entity? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return !other.IsTransient && !IsTransient && other.Id == Id;
        }

        public int CompareTo(Entity? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return _Id.CompareTo(other._Id);
        }
    }
}