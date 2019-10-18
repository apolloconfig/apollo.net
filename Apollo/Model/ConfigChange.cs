using Com.Ctrip.Framework.Apollo.Enums;

namespace Com.Ctrip.Framework.Apollo.Model
{
    /// <summary>
    /// Holds the information for a config change.
    /// </summary>
    public class ConfigChange
    {
        /// <summary>
        /// Constructor. </summary>
        /// <param name="config"> the config of the key </param>
        /// <param name="propertyName"> the key whose value is changed </param>
        /// <param name="oldValue"> the value before change </param>
        /// <param name="newValue"> the value after change </param>
        /// <param name="changeType"> the change type </param>
        public ConfigChange(IConfig config, string propertyName, string? oldValue, string? newValue,
            PropertyChangeType changeType)
        {
            Config = config;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
            ChangeType = changeType;
        }

        public string PropertyName { get; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public PropertyChangeType ChangeType { get; set; }

        public IConfig Config { get; }

        public override string ToString()
        {
            return "ConfigChange{" +
                "propertyName='" + PropertyName + '\'' +
                ", oldValue='" + OldValue + '\'' +
                ", newValue='" + NewValue + '\'' +
                ", changeType=" + ChangeType +
                '}';
        }
    }
}

