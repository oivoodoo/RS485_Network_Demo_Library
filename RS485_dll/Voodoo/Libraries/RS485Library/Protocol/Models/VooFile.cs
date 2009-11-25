using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Models
{
    public class VooFile
    {
        private String source;
        private String destination;
        private String from;
        private String to;
        private Int64 size;

        // TODO: Create swap file for replacing with crap memory collector.
        // For information about with crap contact with voodoo.
        private String data = String.Empty;

        public VooFile(String source, String destination, 
            String from, String to, Int64 size) : this(source, destination, from, to)
        {
            this.size = size;
        }

        public VooFile(String source, String destination,
            String from, String to)
        {
            this.source = source;
            this.destination = destination;
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Collect file.
        /// </summary>
        /// <returns>if file is downloaded return true else false.</returns>
        public bool Collect(String data)
        {
            this.data += data;
            // TODO: This part of code is unstable.
            Console.WriteLine("VooFile: from='{0}', to='{1}', size='{2}', commint data='{3}'", from, to, size, data);
            if (this.data.Length == size)
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(to, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    writer.Write(this.data);
                }
                return true;
            }
            return false;
        }

        public bool IsParent(String source, String destination)
        {
            return this.source == source && this.destination == destination;
        }

        public void ReadToEnd()
        {
            using (StreamReader reader = new StreamReader(new FileStream(from, FileMode.OpenOrCreate, FileAccess.Read)))
            {
                this.data = reader.ReadToEnd();
            }
        }

        public String From
        {
            get 
            { 
                return from; 
            }
        }

        public String To
        {
            get
            {
                return to;
            }
        }

        public Int64 Size
        {
            get
            {
                return size;
            }
        }

        public String Data
        {
            get
            {
                return data;
            }
        }
    }
}
