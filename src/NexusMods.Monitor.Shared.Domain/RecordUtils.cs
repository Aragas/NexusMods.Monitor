using System.Runtime.Serialization;

namespace NexusMods.Monitor.Shared.Domain
{
    public static class RecordUtils
    {
        /// <summary>
        /// Shouldn't be used if the record derives from another class. Everything private-scoped will be left uninitialized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Default<T>() where T : class => FormatterServices.GetUninitializedObject(typeof(T)) as T ?? default!;
    }
}