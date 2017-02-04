﻿using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Aegir.Util
{
    public class DebugUtil
    {
        private static readonly ILog defaultLog = LogManager.GetLogger(typeof(DebugUtil));

        public static void LogWithLocation(string logData,
            bool shortenCallerFilepath = true,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (shortenCallerFilepath)
            {
                FileInfo fileInfo = new FileInfo(sourceFilePath);
                sourceFilePath = fileInfo.Name;
            }
            Debug.WriteLine("[" + sourceFilePath + ":" + sourceLineNumber + "@" + memberName + "]" + logData);
        }

    }

    public class PerfStopwatch
    {
        private ILog log;
        private string description;
        private Stopwatch stopwatch;

        private PerfStopwatch(string description, ILog log)
        {
            this.log = log;
            this.description = description;
            stopwatch = Stopwatch.StartNew();
        }
        public void Stop()
        {
            stopwatch.Stop();
            log.Debug($"[ {description} ] used {stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public static PerfStopwatch StartNew(string description, ILog log)
        {
            return new PerfStopwatch(description, log);
        }

    }
}