using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Web
{
    /// <summary>
    /// Fake IWebHostEnvironment for .NET Core 2.x
    /// </summary>
    public interface IWebHostEnvironment : IHostEnvironment
    {
        /// <summary>
        /// Gets or sets an <see cref="T:Microsoft.Extensions.FileProviders.IFileProvider" /> 
        /// pointing at <see cref="P:Microsoft.AspNetCore.Hosting.IWebHostEnvironment.WebRootPath" />.
        /// </summary>
        IFileProvider WebRootFileProvider { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the web-servable application content files.
        /// </summary>
        string WebRootPath { get; set; }
    }
}
