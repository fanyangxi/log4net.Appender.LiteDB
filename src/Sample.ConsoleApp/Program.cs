using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net.Appender;
using LiteDB;

namespace Sample.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get a logger instance
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            Console.WriteLine("Appenders:");
            foreach (var appender in logger.Logger.Repository.GetAppenders())
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
            logger.Debug("This is a debug log message");
            logger.Info("This is an info log message");
            logger.Warn("This is a warn log message");
            logger.Error("This is an error log message");
            logger.Fatal("This is a fatal log message");

            //// Write a message with exception
            //try
            //{
            //    throw new Exception();
            //}
            //catch (Exception ex)
            //{
            //    logger.Error("Our pretend exception detected!", ex);
            //}
            using (var db = new LiteDatabase(@"Logs\sample-logs.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<BsonDocument>("logs");

                // Use LINQ to query documents
                var results = col.FindAll();
            }
#if DEBUG
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
