
using System;

namespace VFrame.Scene
{
    public interface IPreloadPrefabReservable
    {
        public string BundleKey { get; }
        public Type PoolType { get; }
    }
    
    public class AddressablePrefabReservable : IPreloadPrefabReservable
    {
        public string BundleKey { get; }
        public Type PoolType { get; }
        public AddressablePrefabReservable(string key, Type type) => (BundleKey, PoolType) = (key, type);
    }
}