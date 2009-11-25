using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Voodoo.Libraries.Logs
{
    public class LogFolderConverter : log4net.Util.PatternConverter
    {
        ///<summary>
        ///
        ///            Evaluate this pattern converter and write the output to a writer.
        ///            
        ///</summary>
        ///
        ///<param name="writer"><see cref="T:System.IO.TextWriter" /> that will receive the formatted result.</param>
        ///<param name="state">The state object on which the pattern converter should be executed.</param>
        ///<remarks>
        ///
        ///<para>
        ///
        ///            Derived pattern converters must override this method in order to
        ///            convert conversion specifiers in the appropriate way.
        ///            
        ///</para>
        ///
        ///</remarks>
        ///
        protected override void Convert(TextWriter writer, object state)
        {
            writer.Write(Path.Combine(LogConfiguration.UserApplicationFolder, "RS485.log"));
        }
    }
}
