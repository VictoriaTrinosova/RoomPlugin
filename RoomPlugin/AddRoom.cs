using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class AddRoom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;
            Document arDoc = commandData.Application.ActiveUIDocument.Document;


            FamilySymbol familySymbol = new FilteredElementCollector(arDoc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .OfType<FamilySymbol>()
                .Where(x => x.FamilyName.Equals("Марка"))
                .FirstOrDefault();
            if (familySymbol == null)
            {
                TaskDialog.Show("Ошибка", "Не найдено семейство \"Марка\"");
                return Result.Cancelled;
            }


            Level level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .FirstOrDefault();

            Transaction transaction0 = new Transaction(arDoc);
            transaction0.Start("Расстановка помещений");
            if (!familySymbol.IsActive)
            {
                familySymbol.Activate();
            }
            transaction0.Commit();


            Transaction transaction = new Transaction(doc);
            transaction.Start("rooms");
            PlanTopology pt = doc.get_PlanTopology(level);

            foreach (PlanCircuit pc in pt.Circuits)
            {
                if (!pc.IsRoomLocated)
                {
                    Room r = doc.Create.NewRoom(null, pc);
                }
            }
            transaction.Commit();
            return Result.Succeeded;
        }
           
    }
}
