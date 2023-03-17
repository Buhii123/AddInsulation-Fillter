using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InsulationPipe.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InsulationPipe
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class InsulationPipeCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication UIAPP = commandData.Application;
            UIDocument UIDOC = UIAPP.ActiveUIDocument;
            Document DOC = UIDOC.Document;
            try {
                if (DOC.IsFamilyDocument)
                {
                    TaskDialog.Show("Warrning", "Không thể dùng Tool trên Family!");
                    return Result.Cancelled;
                }
                ControlViewMain Mainwindow = new ControlViewMain(UIAPP);
                Mainwindow.MainView.ShowDialog();
            }
            catch {
                MessageBox.Show("Fail!\n");
                return Result.Cancelled;    
            }
           

            return Result.Succeeded;
        }
    }
}
