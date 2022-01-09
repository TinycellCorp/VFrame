using System;
using UnityEngine;

namespace VFrame.UI.Extension
{
    public static class AsyncOperationExtensions
    {
        public static void Active(this AsyncOperation operation)
        {
            if (operation.isDone)
            {
                operation.allowSceneActivation = true;
            }
            else
            {
                throw new Exception("not isDone Operation");
            }
        }
    }
}