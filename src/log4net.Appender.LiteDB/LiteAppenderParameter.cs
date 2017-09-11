using System;
using log4net.Layout;

namespace log4net.Appender.LiteDB
{
    /// <summary>
    /// Describes a single log field entry from the configuration file.
    /// :: AdoNetAppenderParameter
    /// </summary>
    public class LiteAppenderParameter
    {
        /// <summary>
        /// Gets or sets the parameter name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the log layout type that will format the final log entry
        /// </summary>
        public IRawLayout Layout { get; set; }

        /// <summary>
        /// Gets or sets the log format value
        /// </summary>
        public string Value { get; set; }
    }
}