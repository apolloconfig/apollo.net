using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Ctrip.Framework.Apollo.Core.Ioc.LightInject;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc.Extensions.Annotation
{
    /// <summary>
    /// An <see cref="IFieldDependencySelector"/> that uses the <see cref="InjectAttribute"/>
    /// to determine which fields to inject dependencies.
    /// </summary>
    internal class AnnotatedFieldDependencySelector : FieldDependencySelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotatedFieldDependencySelector"/> class.
        /// </summary>
        /// <param name="fieldSelector">The <see cref="IFieldSelector"/> that is 
        /// responsible for selecting a list of injectable fields.</param>
        public AnnotatedFieldDependencySelector(IFieldSelector fieldSelector)
            : base(fieldSelector)
        { }

        /// <summary>
        /// Selects the field dependencies for the given <paramref name="type"/> 
        /// that is annotated with the <see cref="InjectAttribute"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the field dependencies.</param>
        /// <returns>A list of <see cref="FieldDependency"/> instances that represents the field
        /// dependencies for the given <paramref name="type"/>.</returns>
        public override IEnumerable<FieldDependency> Execute(Type type)
        {
            var fields = FieldSelector.Execute(type).Where(f => f.IsDefined(typeof(InjectAttribute), true)).ToArray();
            foreach (var fieldInfo in fields)
            {
                var injectAttribute = (InjectAttribute)fieldInfo.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
                if (injectAttribute != null)
                {
                    yield return
                        new FieldDependency
                        {
                            Field = fieldInfo,
                            ServiceName = injectAttribute.ServiceName,
                            ServiceType = fieldInfo.FieldType,
                            IsRequired = true
                        };
                }
            }
        }
    }
}
