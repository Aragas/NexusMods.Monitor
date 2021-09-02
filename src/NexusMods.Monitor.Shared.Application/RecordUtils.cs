﻿using System.Runtime.Serialization;

namespace NexusMods.Monitor.Shared.Application
{
    public static class RecordUtils
    {
        public static T Default<T>() where T : class => FormatterServices.GetUninitializedObject(typeof(T)) as T ?? default!;
    }
}