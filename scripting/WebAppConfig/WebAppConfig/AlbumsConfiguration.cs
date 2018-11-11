using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace WebAppConfig
{
    public class AlbumsConfiguration
    {
        public string VisibleAlbumsFilter { get; set; }

        public Func<Album, bool> VisibleAlbumsFilterLambda { get; set; }
    }
}
