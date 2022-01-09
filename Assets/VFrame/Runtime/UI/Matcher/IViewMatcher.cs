using System;
using VFrame.UI.Context;

namespace VFrame.UI.Matcher
{
    public interface IViewMatcher<T>
    {
        Type ViewType { get; }
        T Resolve(ISystemContext context);
    }
}