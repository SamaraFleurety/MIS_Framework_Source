using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    /// <summary>
    /// string defName<br/>
    /// string label<br/>
    /// Int32 sortingOrder<br/>
    /// string textureFolder
    /// </summary>
    public class OperatorClassDef : Def
    {
        [Obsolete]
        public int sortingOrder = 0;
        public string textureFolder;
    }
}
