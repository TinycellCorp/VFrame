using System;
using System.Collections.Generic;

namespace VFrame.Scene
{
    public interface IPreloadPrefabVariantReservable
    {
        public IReadOnlyList<string> BundleKeys { get; }
        public Type PoolType { get; }
    }
}