using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsulationPipe.Model
{
    public class PipeInsu : AddInsulation
    {
        public override ElementId elementId { get; }
        public override string ClassName { get; }
        public override string systemType { get; }
        public override string diameter { get; }
        public PipeInsu(Element el, BuiltInParameter systemtype, BuiltInParameter parameter) 
        {
            this.elementId = el.Id;
            this.ClassName = el.Category.Name;
            this.systemType = el.get_Parameter(systemtype).AsValueString();
            this.diameter= el.get_Parameter(parameter).AsValueString();
        } 
    }
}
