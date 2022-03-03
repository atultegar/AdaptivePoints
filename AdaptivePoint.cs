using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Point = Autodesk.DesignScript.Geometry.Point;
using RevitServices.Persistence;
using Revit.GeometryConversion;
using RevitServices.Transactions;
using System.Linq;

namespace AdaptivePoints
{
    public class AdaptivePoint
    {
        /// <summary>
        /// Adaptive Component Family Instance
        /// </summary>
        /// <returns>Location Point List</returns>
        [STAThread]
        public static IList<Point> GetLocationPoints(Revit.Elements.AdaptiveComponent adaptiveComponent)
        {
            List<string> refNameList = new List<string>();
            List<Point> rvtPointList = new List<Point>();
            List<Point> sharedPointList = new List<Point>();
            List<ReferencePoint> referencePoints = new List<ReferencePoint>();
            List<LocationPoint> locationPoints = new List<LocationPoint>();
            List<Point> newPoints = new List<Point>();


            int eleId = adaptiveComponent.Id;
            ElementId elementId = new ElementId(eleId);
            FamilyInstance famInstance = DocumentManager.Instance.CurrentDBDocument.GetElement(elementId) as FamilyInstance;
            if (AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(famInstance))
            {
                IList<ElementId> ids = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(famInstance);
                foreach (ElementId id in ids)
                {
                    ReferencePoint p = DocumentManager.Instance.CurrentDBDocument.GetElement(id) as ReferencePoint;
                    referencePoints.Add(p);
                    
                    Point rvtPoint = p.Position.ToPoint();
                    
                    refNameList.Add(p.Name);
                    
                    rvtPointList.Add(rvtPoint);
                    Point sharedPoint = rvtPoint.Transform(Utils.DocumentTotalTransform().Inverse()) as Point;
                    
                    sharedPointList.Add(sharedPoint);
                    
                }
            }

            AdaptiveForm adaptiveForm = new AdaptiveForm
            {
                RevitPoints = rvtPointList,
                SharedPoints = sharedPointList
            };


            adaptiveForm.lstRP.ItemsSource = refNameList;
            

            var res = adaptiveForm.ShowDialog();

            if (!res.HasValue && res.Value)
            {
                adaptiveForm.Close();
            }

            foreach (Point p in adaptiveForm.RevitPoints)
            {
                newPoints.Add(p);
            }

            UpdateByPoints(adaptiveComponent, newPoints);

            return newPoints;
        }

       
        public static void UpdateByPoints(Revit.Elements.AdaptiveComponent adaptiveComponent, List<Point> points)
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;

            TransactionManager.Instance.EnsureInTransaction(doc);

            int eleId = adaptiveComponent.Id;
            ElementId elementId = new ElementId(eleId);
            FamilyInstance famInstance = DocumentManager.Instance.CurrentDBDocument.GetElement(elementId) as FamilyInstance;
            if (AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(famInstance))
            {
                IList<ElementId> ids = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(famInstance);

                if (ids.Count() != points.Count())
                    throw new Exception("UpdateByPoints failed.\nInput points count mismatch.");

                for (int i = 0; i < ids.Count; i++)
                {

                    ReferencePoint p = DocumentManager.Instance.CurrentDBDocument.GetElement(ids[i]) as ReferencePoint;

                    using (SubTransaction subTr = new SubTransaction(doc))
                    {
                        subTr.Start();

                        try
                        {
                            
                            XYZ refPointXYZ = points[i].ToRevitType(true);

                            p.Position = refPointXYZ;

                            subTr.Commit();

                        }

                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            
        }

        
    }
}