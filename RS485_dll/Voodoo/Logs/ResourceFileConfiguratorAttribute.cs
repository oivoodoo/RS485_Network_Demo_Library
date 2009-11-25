using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using log4net.Config;
using log4net.Repository;

namespace Voodoo.Libraries.Logs
{
    public class ResourceFileConfiguratorAttribute : ConfiguratorAttribute
    {
        private readonly string resourceName;


        public ResourceFileConfiguratorAttribute(string resourceName) : base(0)
        {
            this.resourceName = resourceName;
        }

        ///<summary>
        ///
        ///            Configures the <see cref="T:log4net.Repository.ILoggerRepository" /> for the specified assembly.
        ///            
        ///</summary>
        ///
        ///<param name="sourceAssembly">The assembly that this attribute was defined on.</param>
        ///<param name="targetRepository">The repository to configure.</param>
        ///<remarks>
        ///
        ///<para>
        ///
        ///            Abstract method implemented by a subclass. When this method is called
        ///            the subclass should configure the <paramref name="targetRepository" />.
        ///            
        ///</para>
        ///
        ///</remarks>
        ///
        public override void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository)
        {
            XmlConfigurator.Configure(
                        Assembly.GetAssembly(typeof(Logger)).GetManifestResourceStream(
                            resourceName));
        }
    }
}
