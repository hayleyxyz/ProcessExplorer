using System;
using System.Runtime.InteropServices;

namespace ProcessExplorer {
    internal static class ProcessUtils {

        [DllImport("ProcessUtils.dll")]
        internal static extern bool IsWow64ProcessById(uint processId, ref bool isWow64);

        internal static extern bool GetThreadContextById(uint threadId);

    }
}
