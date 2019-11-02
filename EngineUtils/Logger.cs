using System;
using System.Runtime.CompilerServices;
using Godot;

namespace GameOff_2019.EngineUtils {
    public class Logger {
        public static void Debug(string message, [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string caller = null) {
            var now = DateTime.Now;
            var logMessage = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond} {caller} at:{lineNumber} D/: {message}";
            Console.Out.WriteLine(logMessage);
            GD.Print(logMessage);
        }

        public static void Info(string message, [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string caller = null) {
            var now = DateTime.Now;
            var logMessage = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond} {caller} at:{lineNumber} I/: {message}";
            Console.Out.WriteLine(logMessage);
            GD.Print(logMessage);
        }

        public static void Warning(string message, [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string caller = null) {
            var now = DateTime.Now;
            var logMessage = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond} {caller} at:{lineNumber} W/: {message}";
            Console.Out.WriteLine(logMessage);
            GD.Print(logMessage);
        }

        public static void Error(string message, [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string caller = null) {
            var now = DateTime.Now;
            var logMessage = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond} {caller} at:{lineNumber} E/: {message}";
            Console.Error.WriteLine(logMessage);
            GD.PrintErr(logMessage);
        }
    }
}