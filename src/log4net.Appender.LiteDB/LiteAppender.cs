using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using log4net.Core;
using log4net.Util;
using LiteDB;

namespace log4net.Appender.LiteDB
{
    /// <summary>
    /// The log4net appender. Saves log records to a mongo database.
    /// :: Thanks to AdoNetAppender
    /// </summary>
    /// <example>
    /// An example configuration to log to the above table:
    /// <code lang="XML" escaped="true">
    /// <appender name="LiteDbAppender" type="log4net.Appender.LiteDB.LiteAppender, log4net.Appender.LiteDB">
    ///   <connectionString value="Logs\sample-logs.db"/>
    ///   <collectionName value="logs"/>
    ///   <parameter>
    ///     <name value="timestamp"/>
    ///     <layout type="log4net.Layout.RawTimeStampLayout"/>
    ///   </parameter>
    ///   <parameter>
    ///     <name value="level"/>
    ///     <layout type="log4net.Layout.PatternLayout">
    ///       <conversionPattern value="%p"/>
    ///     </layout>
    ///   </parameter>
    ///   <parameter>
    ///     <name value="thread"/>
    ///     <layout type="log4net.Layout.PatternLayout">
    ///       <conversionPattern value="%t"/>
    ///     </layout>
    ///   </parameter>
    ///   <parameter>
    ///     <name value="logger"/>
    ///     <layout type="log4net.Layout.PatternLayout">
    ///       <conversionPattern value="%c"/>
    ///     </layout>
    ///   </parameter>
    ///   <parameter>
    ///     <name value="message"/>
    ///     <layout type="log4net.Layout.PatternLayout">
    ///       <conversionPattern value="%m"/>
    ///     </layout>
    ///   </parameter>
    ///   <parameter>
    ///     <name value="exception"/>
    ///     <layout type="log4net.Layout.ExceptionLayout">
    ///       <conversionPattern value="%ex{full}"/>
    ///     </layout>
    ///   </parameter>
    /// </appender>
    /// </code>
    /// </example>
    public class LiteAppender : BufferingAppenderSkeleton
    {
        /// <summary>The fully qualified type of this appender class.</summary>
        /// <remarks>
        /// Used by the internal logger to record the Type of the log message.
        /// </remarks>
        private static readonly Type declaringType = typeof(LiteAppender);

        /// <summary>
        /// Gets or sets Lite collection to write to. Initialised when the appender is activated
        /// </summary>
        private LiteDatabase databaseConnection;

        /// <summary>
        /// The list of log parameters to save. Initialised from the log4net configuration
        /// </summary>
        private List<LiteAppenderParameter> parameters;

        /// <summary>
        /// Gets or sets the path to the file that logging will be written to.
        /// </summary>
        /// <value>The path to the file that logging will be written to.</value>
        /// <remarks>
        /// <para>
        /// If the path is relative it is taken as relative from
        /// the application base directory.
        /// Sample: C:\my-lite-logs.db
        /// </para>
        /// </remarks>
        public virtual string File { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection in the database. Defaults to "logs"
        /// </summary>
        public string CollectionName { get; set; }

        public LiteAppender()
        {
            parameters = new List<LiteAppenderParameter>();
        }

        /// <summary>
        /// Adds an entry from the config to the list of fields to log
        /// </summary>
        /// <param name="parameter">The field to log</param>
        public void AddParameter(LiteAppenderParameter parameter)
        {
            parameters.Add(parameter);
        }

        /// <summary>
        /// Initialize the appender based on the options set
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// </remarks>
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            InitializeDatabaseConnection();
        }

        /// <summary>Override the parent method to close the database</summary>
        /// <remarks>
        /// <para>
        /// Closes the database command and database connection.
        /// </para>
        /// </remarks>
        protected override void OnClose()
        {
            base.OnClose();
            DiposeConnection();
        }

        /// <summary>
        /// Appends a logging event to Mongo
        /// </summary>
        /// <param name="loggingEvent">The logging event</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            var record = BuildBsonDocument(loggingEvent);
            var collection = GetCollection();
            collection.Insert(record);
        }

        /// <summary>
        /// Inserts the events into the database.
        /// </summary>
        /// <param name="events">The events to insert into the database.</param>
        /// <remarks>
        /// <para>
        /// Insert all the events specified in the <paramref name="events"/>
        /// array into the database.
        /// </para>
        /// </remarks>
        protected override void SendBuffer(Core.LoggingEvent[] events)
        {
            foreach (var logEvent in events)
            {
                Append(logEvent);
            }
        }

        /// <summary>
        /// Gets the Mongo collection that the logs will be written to. If one isn't specified 
        /// in the configuration then it defaults to 'logs'.
        /// </summary>
        /// <returns>The Mongo collection</returns>
        protected virtual LiteCollection<BsonDocument> GetCollection()
        {
            return databaseConnection.GetCollection(CollectionName ?? "logs");
        }

        /// <summary>
        /// Gets the Mongo database based on the connection string. IF the database name isn't 
        /// present in the connection string it defaults to 'log4net'.
        /// </summary>
        /// <returns>The Mongo database</returns>
        protected virtual LiteDatabase CreateDatabaseConnection()
        {
            var fullPath = SystemInfo.ConvertToFullPath(this.File);
            var db = new LiteDatabase(fullPath);
            return db;
        }

        /// <summary>
        /// Builds the BSON document to send to Mongo from the log4net LoggingEvent.
        /// </summary>
        /// <param name="log">The logging event</param>
        /// <returns>The BSON document</returns>
        private BsonDocument BuildBsonDocument(LoggingEvent log)
        {
            var doc = new BsonDocument();
            foreach (var parameter in parameters)
            {
                try
                {
                    if (parameter.Layout == null)
                    {
                        continue;
                    }

                    var value = parameter.Layout.Format(log);
                    var bsonValue = value as BsonValue ?? new BsonValue(value);
                    doc.Add(parameter.Name, bsonValue);
                }
                catch (Exception ex)
                {
                    this.ErrorHandler.Error("Exception while build bson document", ex);
                }
            }

            return doc;
        }

        /// <summary>Connects to the database.</summary>
        private void InitializeDatabaseConnection()
        {
            try
            {
                DiposeConnection();
                databaseConnection = CreateDatabaseConnection();
            }
            catch (Exception ex)
            {
                this.ErrorHandler.Error("Could not open lite database file [" + this.File + "].", ex);
                databaseConnection = (LiteDatabase)null;
            }
        }

        /// <summary>Cleanup the existing connection.</summary>
        /// <remarks>
        /// Calls the IDbConnection's <see cref="M:System.Data.IDbConnection.Close" /> method.
        /// </remarks>
        private void DiposeConnection()
        {
            if (databaseConnection == null)
                return;
            try
            {
                databaseConnection.Dispose();
            }
            catch (Exception ex)
            {
                LogLog.Warn(LiteAppender.declaringType, "Exception while disposing cached connection object", ex);
            }

            databaseConnection = (LiteDatabase)null;
        }
    }
}
