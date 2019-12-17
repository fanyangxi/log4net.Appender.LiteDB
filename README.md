# log4net.Appender.LiteDB
A log4net appender for LiteDB (http://www.litedb.org/)

[![NuGet version](https://badge.fury.io/nu/log4net.appender.litedb.svg)](https://badge.fury.io/nu/log4net.appender.litedb)

## Getting started
Built with Visual Studio 2017, .Net framework 4.0

### General explaination:
You can use this appender to write logs to LiteDB.

### log4net configuration file sample:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="LiteDb" type="log4net.Appender.LiteDB.LiteAppender, log4net.Appender.LiteDB">
    <file value="Logs\sample-logs.db"/>
	<!-- set FileMaxSizeInMB to value larger that 10 to enable rotate db file by size automatically -->
	<fileMaxSizeInMB value="100" />
    <collectionName value="logs"/>
    <parameter>
      <name value="timestamp"/>
      <layout type="log4net.Layout.RawTimeStampLayout"/>
    </parameter>
    <parameter>
      <name value="level"/>
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%p"/>
      </layout>
    </parameter>
    <parameter>
      <name value="thread"/>
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%t"/>
      </layout>
    </parameter>
    <parameter>
      <name value="logger"/>
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%c"/>
      </layout>
    </parameter>
    <parameter>
      <name value="message"/>
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%m"/>
      </layout>
    </parameter>
    <parameter>
      <name value="exception"/>
      <layout type="log4net.Layout.ExceptionLayout">
        <conversionPattern value="%ex{full}"/>
      </layout>
    </parameter>
  </appender>
  <root>
    <level value="ALL" />
    <appender-ref ref="LiteDb" />
  </root>
</log4net>
```

## License
https://github.com/fanyangxi/log4net.Appender.LiteDB/blob/master/LICENSE
