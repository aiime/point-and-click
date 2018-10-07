using System;

namespace Service
{
    public static class InvokeWithNullCheck
    {
        public static void SafeInvoke(this Action action)
        {
            if (action != null) action.Invoke();
        }
    }
}