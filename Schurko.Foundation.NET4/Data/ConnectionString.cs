using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Data
{
    public interface IConnectionString
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class ConnectionString : IConnectionString
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ConnectionString(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
