using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net.Appender;
using LiteDB;
using log4net.Util;
using System.Collections;
using log4net;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Sample.ConsoleApp
{
    class Program
    {
        private static ILoggerFactory loggerFactory;
        public static ILogger Log<T>() => loggerFactory.CreateLogger<T>();
        static void Main(string[] args)
        {
            // Get a logger instance

            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            loggerFactory = new LoggerFactory()
         .AddConsole()
          .AddLog4Net("log4net.config");



            var logger = loggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            
            Console.WriteLine("Appenders:");
            foreach (var appender in LogManager.GetRepository (MethodBase.GetCurrentMethod().DeclaringType.Assembly).GetAppenders())
            {
                Console.WriteLine("    {0}", appender.Name);

                var aa = appender as AppenderSkeleton;
                if (aa != null)
                {
                    //aa.ErrorHandler = new CustomLoggingErrorHandler((message, ex, eCode) =>
                    //{
                    //    Console.WriteLine("LOGGING FAILURE: {0}", message);
                    //});
                }
            }

            //Console.WriteLine("Writting log messages...");
            //for (int i = 0; i < 50; i++)
            //{
            //    var stubBigMessage = File.ReadAllText("Resources/stub-big-message.txt");
            //    //logger.Warn(string.Format("This is a warn log message @{0}", DateTime.UtcNow.ToLongTimeString()));
            //    logger.Error(string.Format("A error log @{0}, BIG: {1}", DateTime.UtcNow.ToLongTimeString(), stubBigMessage),
            //        new Exception("Level-3: Page \"failed\" to load",
            //            new Exception("Level-2: MyViewModel onbind error",
            //                new Exception("Level-1: Object null reference"))));
            //    System.Threading.Thread.Sleep(1000);
            //}

            //Console.WriteLine("Writting log messages...");
            // Write a few messages
            logger.LogDebug("This is a debug log message");
            logger.LogInformation("This is an info log message");
            logger.LogWarning("This is a warn log message");
            logger.LogError("This is an error log message");
            logger.LogCritical("This is a fatal log message");

            //// Write a message with exception
            //try
            //{
            //    throw new Exception();
            //}
            //catch (Exception ex)
            //{
            //    logger.Error("Our pretend exception detected!", ex);
            //}
            using (var db = new LiteDatabase(AppContext.BaseDirectory+ "\\sample-logs.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<BsonDocument>("logs");

                // Use LINQ to query documents
                var results = col.FindAll();
                results.ToList().ForEach(bd => Console.WriteLine(bd.ToString()));
            }
#if DEBUG
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
