using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace Planty.EngineUtils {
    public class Logger {
        public static void Debug(string message, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string caller = null) {
            var callerClassName = caller.Split("/").Last().Split(".").First();
            var limitedCallerClassName = callerClassName.Length <= 17 ? callerClassName : callerClassName.Substring(0, 17);
            var callerClassNameWithLineNumber = $"{limitedCallerClassName,-17}:{lineNumber,-3}";
            var now = DateTime.Now;
            var formattedTime = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}";
            var logMessage = $"{formattedTime,-22} {callerClassNameWithLineNumber,-20} D/: {message}";
            Console.Out.WriteLine(logMessage);
            GD.Print(logMessage);
        }

        public static void Info(string message, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string caller = null) {
            var callerClassName = caller.Split("/").Last().Split(".").First();
            var limitedCallerClassName = callerClassName.Length <= 17 ? callerClassName : callerClassName.Substring(0, 17);
            var callerClassNameWithLineNumber = $"{limitedCallerClassName,-17}:{lineNumber,-3}";
            var now = DateTime.Now;
            var formattedTime = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}";
            var logMessage = $"{formattedTime,-22} {callerClassNameWithLineNumber,-20} I/: {message}";
            Console.Out.WriteLine(logMessage);
            GD.Print(logMessage);
        }

        public static void Warning(string message, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string caller = null) {
            var callerClassName = caller.Split("/").Last().Split(".").First();
            var limitedCallerClassName = callerClassName.Length <= 17 ? callerClassName : callerClassName.Substring(0, 17);
            var callerClassNameWithLineNumber = $"{limitedCallerClassName,-17}:{lineNumber,-3}";
            var now = DateTime.Now;
            var formattedTime = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}";
            var logMessage = $"{formattedTime,-22} {callerClassNameWithLineNumber,-20} W/: {message}";
            Console.Out.WriteLine(logMessage);
            GD.Print(logMessage);
        }

        public static void Error(string message, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string caller = null) {
            var callerClassName = caller.Split("/").Last().Split(".").First();
            var limitedCallerClassName = callerClassName.Length <= 17 ? callerClassName : callerClassName.Substring(0, 17);
            var callerClassNameWithLineNumber = $"{limitedCallerClassName,-17}:{lineNumber,-3}";
            var now = DateTime.Now;
            var formattedTime = $"{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}";
            var logMessage = $"{formattedTime,-22} {callerClassNameWithLineNumber,-20} E/: {message}";
            Console.Error.WriteLine(logMessage);
            GD.PrintErr(logMessage);
        }
    }
}