using System;
using System.Collections.Generic;

namespace VFrame.Scene
{
    public class AddressablePrefabVariantReservable : IPreloadPrefabVariantReservable
    {
        public IReadOnlyList<string> BundleKeys { get; }
        public Type PoolType { get; }
        public AddressablePrefabVariantReservable(IReadOnlyList<string> keys, Type type) => (BundleKeys, PoolType) = (keys, type);
    }
}