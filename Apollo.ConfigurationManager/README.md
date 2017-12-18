## ApolloConfigurationBuilder说明
``` xml
<configuration>
    <configBuilders>
        <builders>
            <add name="ApolloConfigBuilder1" type="Com.Ctrip.Framework.Apollo.AppSettingsSectionBuilder, Com.Ctrip.Framework.Apollo.ConfigurationManager" namespace="TEST1.test" />
        </builders>
    </configBuilders>
</configuration>

```
* namespace为可选值，该值对应apollo中的namespace。
* keyPrefix为可选值，默认值等于namespace值，如果有值则会使用:合并key，如果不想设置值请将值设置成空字符串。    
## ConnectionStringsSectionBuilder使用说明
``` xml
<configuration>
    <configBuilders>
        <builders>
            <add name="ConnectionStringsSectionBuilder1" type="Com.Ctrip.Framework.Apollo.ConnectionStringsSectionBuilder, Com.Ctrip.Framework.Apollo.ConfigurationManager" namespace="TEST1.test" defaultProviderName="MySql.Data.MySqlClient" />
        </builders>
    </configBuilders>
</configuration>

```
* namespace为可选值，该值对应apollo中的namespace。
* defaultProviderName为可选值，默认值为System.Data.SqlClient,，对应ConnectionString的ProviderName。    
#### key/value格式要求
* key必须以ConnectionStrings:开始
* value如果是一个json，{"ConnectionString":"required","ProviderName":"optional"}，则必需有ConnectionString属性，否则整个value做为一个ConnectionString。