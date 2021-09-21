using System.Runtime.Serialization;

namespace NexusMods.Monitor.Shared.Common
{
    public static class RecordUtils
    {
        /// <summary>
        /// Shouldn't be used if the record derives from another record. Everything private-scoped will be left uninitialized.
        /// </summary>
        public static T Default<T>() where T : class => FormatterServices.GetUninitializedObject(typeof(T)) as T ?? default!;
    }
}