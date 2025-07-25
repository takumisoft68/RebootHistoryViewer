using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RebootHistoryViewer
{
    internal class RebootHistoryService
    {
        private const string LogName = "System";
        private const string MachineName = ".";

        // Events related to reboot:
        private static readonly HashSet<long> RebootEventIds = new() { 577, 20, 27 };

        public List<RebootInfo> QueryHistory(DateTime dateTimeAfter)
        {
            if (!EventLog.Exists(LogName, MachineName))
                return [];

            using var eventLog = new EventLog(LogName, MachineName);
            var entries = new EventLogEntry[eventLog.Entries.Count];
            eventLog.Entries.CopyTo(entries, 0);

            // Filtering entries based on the specified date and reboot related events
            var filteredEntries = entries
                .Where(e => e.TimeWritten >= dateTimeAfter && RebootEventIds.Contains(e.InstanceId))
                .OrderBy(e => e.TimeWritten)
                .ToList();

            var history = new List<RebootInfo>();
            EventLogEntry? shutdownEvent = null;
            EventLogEntry? goodEvent = null;
            EventLogEntry? bootEvent = null;

            foreach (var entry in filteredEntries)
            {
                switch (entry.InstanceId)
                {
                    case 577:
                        shutdownEvent = entry;
                        goodEvent = null;
                        bootEvent = null;
                        break;
                    case 20:
                        if (shutdownEvent != null &&
                            entry.ReplacementStrings.Length >= 2 &&
                            entry.ReplacementStrings[0] == "true" &&
                            entry.ReplacementStrings[1] == "true")
                        {
                            goodEvent = entry;
                        }
                        else
                        {
                            shutdownEvent = null;
                            goodEvent = null;
                            bootEvent = null;
                        }
                        break;
                    case 27:
                        if (shutdownEvent != null && goodEvent != null)
                        {
                            bootEvent = entry;
                            history.Add(new RebootInfo
                            {
                                ShutdownEvent = shutdownEvent,
                                GoodEvent = goodEvent,
                                BootEvent = bootEvent,
                                ShutdownAt = shutdownEvent.TimeWritten,
                                ShutdownEventRecordId = shutdownEvent.Index,
                                BootAt = bootEvent.TimeWritten,
                                BootEventRecordId = bootEvent.Index
                            });
                            shutdownEvent = null;
                            goodEvent = null;
                            bootEvent = null;
                        }
                        else
                        {
                            shutdownEvent = null;
                            goodEvent = null;
                            bootEvent = null;
                        }
                        break;
                }
            }

            return history;
        }
    }
}