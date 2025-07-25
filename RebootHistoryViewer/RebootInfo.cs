using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebootHistoryViewer
{
    internal class RebootInfo
    {
        public required DateTime ShutdownAt { get; init; }
        public required DateTime BootAt { get; init; }
        public required long ShutdownEventRecordId { get; init; }
        public required long BootEventRecordId { get; init; }

        public required System.Diagnostics.EventLogEntry ShutdownEvent { get; init; }
        public required System.Diagnostics.EventLogEntry BootEvent { get; init; }
        public required System.Diagnostics.EventLogEntry GoodEvent { get; init; }
    }
}
