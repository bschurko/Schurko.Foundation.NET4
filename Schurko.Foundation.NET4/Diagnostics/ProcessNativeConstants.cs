using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schurko.Foundation.Diagnostics
{
    internal static class ProcessNativeConstants
    {
        public const int SE_PRIVILEGE_ENABLED = 0x02;
        public const int ERROR_NO_TOKEN = 1008;
        public const int RPC_S_INVALID_BINDING = 1702;
        public const uint WM_QUIT = 0x12;
        public const uint WM_CLOSE = 0x10;
        public const uint WM_DESTROY = 0x02;
    }
}
