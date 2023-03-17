using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsulationPipe.Model
{
    public abstract class AddInsulation
    {
        public abstract ElementId elementId { get; }
        public abstract string ClassName { get; }
        public abstract string systemType { get; }
        public abstract string diameter { get; }

    }
}
