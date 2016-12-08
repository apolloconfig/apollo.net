using System;
using Com.Ctrip.Framework.Apollo.Enums;

namespace Com.Ctrip.Framework.Apollo.Model
{
    /// <summary>
    /// Holds the information for a config change.
    /// </summary>
    public class ConfigChange
    {
        private string namespaceName;
        private string propertyName;
        private string oldValue;
        private string newValue;
        private PropertyChangeType changeType;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="namespace"> the namespace of the key </param>
        /// <param name="propertyName"> the key whose value is changed </param>
        /// <param name="oldValue"> the value before change </param>
        /// <param name="newValue"> the value after change </param>
        /// <param name="changeType"> the change type </param>
        public ConfigChange(string namespaceName, string propertyName, string oldValue, string newValue,
            PropertyChangeType changeType)
        {
            this.namespaceName = namespaceName;
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.changeType = changeType;
        }

        public string PropertyName
        {
            get
            {
                return propertyName;
            }
        }

        public string OldValue
        {
            get
            {
                return oldValue;
            }
            set
            {
                this.oldValue = value;
            }
        }

        public string NewValue
        {
            get
            {
                return newValue;
            }
            set
            {
                this.newValue = value;
            }
        }

        public PropertyChangeType ChangeType
        {
            get
            {
                return changeType;
            }
            set
            {
                this.changeType = value;
            }
        }

        public string Namespace
        {
            get
            {
                return namespaceName;
            }
        }

        public override string ToString()
        {
            return "ConfigChange{" +
                "namespace='" + namespaceName + '\'' +
                ", propertyName='" + propertyName + '\'' +
                ", oldValue='" + oldValue + '\'' +
                ", newValue='" + newValue + '\'' +
                ", changeType=" + changeType +
                '}';
        }
    }
}

