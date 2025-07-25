using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RebootHistoryViewer
{
    internal class RebootHistoryService
    {
        private readonly string LogName = "System";
        private readonly string MachineName = ".";

        public List<RebootInfo> QueryHistory(DateTime dateTimeAfter)
        {
            if (!System.Diagnostics.EventLog.Exists(LogName, MachineName))
                return new List<RebootInfo>();

            // Collect all event log entries from the System log
            using var eventLog = new System.Diagnostics.EventLog(LogName, MachineName);
            var systemEventEntries = new EventLogEntry[eventLog.Entries.Count];
            eventLog.Entries.CopyTo(systemEventEntries, 0);

            // Filter entries that are after the specified date and match reboot-related event IDs
            var rebootEventEntries = systemEventEntries.Where(v =>
            {
                if (v.TimeWritten < dateTimeAfter)
                    return false;

                return v.InstanceId switch
                {
                    577 => true,
                    20 => true,
                    27 => true,
                    _ => false
                };
            });

            var history = new List<RebootInfo>();
            var rebootEventSet = new EventLogEntry?[3];
            rebootEventSet[0] = null;
            rebootEventSet[1] = null;
            rebootEventSet[2] = null;

            foreach (var entry in rebootEventEntries)
            {
                if (rebootEventSet[0] != null && rebootEventSet[1] != null && rebootEventSet[2] != null)
                {
                    var rebootInfo = new RebootInfo()
                    {
                        ShutdownEvent = rebootEventSet[0]!,
                        GoodEvent = rebootEventSet[1]!,
                        BootEvent = rebootEventSet[2]!,

                        ShutdownAt = rebootEventSet[0]!.TimeWritten,
                        ShutdownEventRecordId = rebootEventSet[0]!.Index,
                        BootAt = rebootEventSet[2]!.TimeWritten,
                        BootEventRecordId = rebootEventSet[2]!.Index,
                    };

                    history.Add(rebootInfo);

                    rebootEventSet[0] = null;
                    rebootEventSet[1] = null;
                    rebootEventSet[2] = null;
                }

                if (rebootEventSet[0] == null)
                {
                    if(entry.InstanceId == 577)
                    {
                        rebootEventSet[0] = entry;
                        continue;
                    }

                    rebootEventSet[0] = null;
                    rebootEventSet[1] = null;
                    rebootEventSet[2] = null;
                }
                else if (rebootEventSet[1]  == null)
                {
                    if (entry.InstanceId == 20 && entry.ReplacementStrings[0] == "true" && entry.ReplacementStrings[1] == "true")
                    {
                        rebootEventSet[1] = entry;
                        continue;
                    }

                    rebootEventSet[0] = null;
                    rebootEventSet[1] = null;
                    rebootEventSet[2] = null;
                }
                else if (rebootEventSet[2]  == null)
                {
                    if (entry.InstanceId == 27)
                    {
                        rebootEventSet[2] = entry;
                        continue;
                    }

                    rebootEventSet[0] = null;
                    rebootEventSet[1] = null;
                    rebootEventSet[2] = null;
                }
            }

            return history;
        }

        private EventLogEntryCollection GetEventLogEntryCollection()
        {
            using var eventLog = new System.Diagnostics.EventLog(LogName, MachineName);
            return eventLog.Entries;
        }
    }
}
