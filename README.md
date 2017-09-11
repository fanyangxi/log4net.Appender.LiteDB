# log4net.Appender.LiteDB
A log4net appender for LiteDB (http://www.litedb.org/)

[![NuGet version](https://badge.fury.io/nu/log4net.appender.litedb.svg)](https://badge.fury.io/nu/log4net.appender.litedb)

## Getting started
Built with Visual Studio 2013, .Net framework 4.0

### General explaination:
You can use this appender to write logs to LiteDB.

### Configuration sample:
```xml
<appender name="LiteDbAppender" type="log4net.Appender.LiteDB.LiteAppender, log4net.Appender.LiteDB">
  <connectionString value="Logs\sample-logs.db"/>
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
```

## License
https://github.com/fanyangxi/log4net.Appender.LiteDB/blob/master/LICENSE
