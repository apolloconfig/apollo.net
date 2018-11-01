using Com.Ctrip.Framework.Apollo.Enums;

namespace Com.Ctrip.Framework.Apollo.Model
{
    /// <summary>
    /// Holds the information for a config change.
    /// </summary>
    public class ConfigChange
    {
        private readonly string _namespaceName;
        private readonly string _propertyName;
        private string _oldValue;
        private string _newValue;
        private PropertyChangeType _changeType;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="namespaceName"> the namespace of the key </param>
        /// <param name="propertyName"> the key whose value is changed </param>
        /// <param name="oldValue"> the value before change </param>
        /// <param name="newValue"> the value after change </param>
        /// <param name="changeType"> the change type </param>
        public ConfigChange(string namespaceName, string propertyName, string oldValue, string newValue,
            PropertyChangeType changeType)
        {
            _namespaceName = namespaceName;
            _propertyName = propertyName;
            _oldValue = oldValue;
            _newValue = newValue;
            _changeType = changeType;
        }

        public string PropertyName => _propertyName;

        public string OldValue
        {
            get => _oldValue;
            set => _oldValue = value;
        }

        public string NewValue
        {
            get => _newValue;
            set => _newValue = value;
        }

        public PropertyChangeType ChangeType
        {
            get => _changeType;
            set => _changeType = value;
        }

        public string Namespace => _namespaceName;

        public override string ToString()
        {
            return "ConfigChange{" +
                "namespace='" + _namespaceName + '\'' +
                ", propertyName='" + _propertyName + '\'' +
                ", oldValue='" + _oldValue + '\'' +
                ", newValue='" + _newValue + '\'' +
                ", changeType=" + _changeType +
                '}';
        }
    }
}

