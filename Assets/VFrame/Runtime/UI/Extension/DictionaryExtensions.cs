using System.Collections.Generic;

namespace VFrame.UI.Extension
{
    public static class DictionaryExtensions
    {
        public static bool SafeRemove<TKey, TValue>(this Dictionary<TKey, TValue> collection, TKey key)
        {
            if (collection.ContainsKey(key))
            {
                return collection.Remove(key);
            }

            return false;
        }
    }
}