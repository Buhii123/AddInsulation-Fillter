using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using InsulationPipe.Base;
using InsulationPipe.Model;
using InsulationPipe.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InsulationPipe.Control
{
    public class ControlViewMain : ViewModelBase
    {
        UIDocument uidoc { get; }
        Document doc { get; }
        private InsutationPipeViewMain mainview;
        public InsutationPipeViewMain MainView
        {
            get
            {
                if (mainview == null) mainview = new InsutationPipeViewMain() { DataContext = this };
                return mainview;
            }
            set
            {
                mainview = value;
                OnPropertyChanged(nameof(InsutationPipeViewMain));
            }
        }
        public List<string> DataSystems { get; } = new List<string>();
        public List<string> insus { get; } = new List<string>();
        private List<string> diameters;
        public List<string> Diameters
        {
            get => diameters;
            set
            {
                diameters = value;
                OnPropertyChanged(nameof(Diameters));
            }
        }
        public List<AddInsulation> addinsu { get; } = new List<AddInsulation>();
        public RelayCommand<object> SelectionChangedCommand { get; set; }
        public RelayCommand<object> CancelCommand { get; set; }
        public RelayCommand<object> OKCommand { get; set; }

        public ControlViewMain(UIApplication uiapp)
        {
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            SelectionChangedCommand = new RelayCommand<object>(p => true, p => SelectChangeListView());
            CancelCommand = new RelayCommand<object>(p => true, p => CancelClose());
            OKCommand = new RelayCommand<object>(p => CheckoutStatus(), p => RunExternal());


            List<Element> pipes = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .ToList();
            foreach (var pipe in pipes) { addinsu.Add((AddInsulation)new PipeInsu(pipe, BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM, BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)); }

            var insulations = new FilteredElementCollector(doc)
                              .OfClass(typeof(PipeInsulationType))
                              .ToElements();
            foreach (var insu in insulations) { insus.Add(insu.Name); }
            insus.Sort();
            var systems = new FilteredElementCollector(doc)
               .WhereElementIsElementType()
               .OfCategory(BuiltInCategory.OST_PipingSystem)
               .ToList<Element>();
            foreach (var system in systems) { DataSystems.Add(system.Name); }
            DataSystems.Sort();

        }
        public void SelectChangeListView()
        {

            Diameters = new List<string>();
            var a = mainview.cbbSystemType.SelectedItem.ToString();
            var selectFillter = from insu in addinsu
                                where insu.systemType == a
                                select insu;
            if (selectFillter.Count() <= 0)
            {
                mainview.cbbDiameter.IsEnabled = false;
                mainview.btnOK.IsEnabled = false;
                mainview.txtBox.Clear();
                mainview.txtBox.IsEnabled = false;


                Diameters = null;
            }
            else
            {
                mainview.cbbDiameter.IsEnabled = true;
                mainview.btnOK.IsEnabled = true;
                mainview.txtBox.IsEnabled = true;
                mainview.cbbDiameter.SelectedIndex = 0;
                List<string> newDiameter = new List<string>();
                foreach (var insu in selectFillter) newDiameter.Add(insu.diameter);
                Diameters = newDiameter.Distinct().ToList();
                Diameters.Sort();

            }

        }
        public void RunExternal()
        {
            double thickness = double.Parse(mainview.txtBox.Text);

            string DN = mainview.cbbDiameter.SelectedItem.ToString();
            string sys = mainview.cbbSystemType.SelectedItem.ToString();



            List<AddInsulation> newAdd = new List<AddInsulation>();
            List<PipeInsulationType> insulations = new FilteredElementCollector(doc)
                             .OfClass(typeof(PipeInsulationType))
                             .Cast<PipeInsulationType>()
                             .ToList();
            ElementId systemInsuID = insulations.Where(n => n.Name.Equals(mainview.cbbInsutype.SelectedItem.ToString())).FirstOrDefault().Id;
            var newAddinsu = from insu in addinsu
                             where insu.systemType.Equals(sys) && insu.diameter.Equals(DN)
                             select insu;
            newAdd = newAddinsu.ToList();
            using (TransactionGroup tranG = new TransactionGroup(doc, "Add Insulation"))
            {
                tranG.Start();
                using (Transaction tran = new Transaction(doc, "add"))
                {
                    tran.Start();
                    int i = 0;
                    foreach (AddInsulation insu in newAdd)
                    {
                        try
                        {
                            i++;
                            PipeInsulation pipeInsulation = PipeInsulation.Create(doc, insu.elementId, systemInsuID, thickness / 304.8);
                        }
                        catch
                        {
                            MessageBox.Show("Ống này đã Add Insu!\n");
                            continue;
                        }
                    }
                    MessageBox.Show("Add " + i.ToString() + " Insulation Thành Công!\n");

                    tran.Commit();
                }
                tranG.Assimilate();
            }

        }
        public void CancelClose()
        {
            this.mainview.Close();
        }
        private bool CheckoutStatus()
        {
            if (string.IsNullOrEmpty(mainview.txtBox.Text)) return false;
            return true;
        }
    }
}
