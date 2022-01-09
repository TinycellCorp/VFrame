using System;
using VFrame.UI.View;
using UnityEngine;

namespace VFrame.UI.Tab
{
    public class TabChildOrder
    {
        private readonly Func<IView, int> _indexOf;

        public TabChildOrder(Func<IView, int> indexOf)
        {
            _indexOf = indexOf;
        }

        public bool TryGetDirection(IView from, IView to, out int direction)
        {
            direction = 0;
            var fromIndex = _indexOf(from);
            var toIndex = _indexOf(to);
            if (fromIndex == -1 || toIndex == -1) return false;

            direction = Mathf.Clamp(toIndex - fromIndex, -1, 1);
            return true;
        }
    }
}