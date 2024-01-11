using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Transactions;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace DynaAdapt
{
    [IsVisibleInDynamoLibrary(false)]
    public class AdaptiveFamily
    {
        #region WIP
        // Work in Progress
        public static string CreateForm(Document famDoc,
                                      string filePath,
                                      List<List<Point>> points,
                                      bool isSolid)
        {
            if (famDoc is null)
            {
                throw new ArgumentNullException(nameof(famDoc));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }
            string msg = "";
            
            TransactionManager.Instance.ForceCloseTransaction();

            Form loftForm = null;

            ReferencePointArray rpa = new ReferencePointArray();
            ReferenceArrayArray ref_ar_ar = new ReferenceArrayArray();
            ReferenceArray ref_ar = new ReferenceArray();
            ReferencePoint rp = null;
            //XYZ xyz = null;

            using (Transaction trans = new Transaction(famDoc, "loft"))
            {
                trans.Start();

                foreach (List<Point> pfPoints in points)
                {
                    List<ReferencePoint> rpList = new List<ReferencePoint>();
                    foreach (Point pfPoint in pfPoints)
                    {
                        rp = famDoc.FamilyCreate.NewReferencePoint(pfPoint.ToRevitType(true));
                        rpList.Add(rp);
                    }

                    for (int i = 0; i <= rpList.Count-1 ; i++)
                    {
                        rpa.Clear();
                        rpa.Append(rpList[i]);
                        rpa.Append(rpList[i + 1]);
                        CurveByPoints cbp1 = famDoc.FamilyCreate.NewCurveByPoints(rpa);
                        ref_ar.Append(cbp1.GeometryCurve.Reference);
                    }
                    ReferencePointArray rpa1 = new ReferencePointArray();
                    rpa1.Append(rpList.Last());
                    rpa1.Append(rpList.First());
                    CurveByPoints cbp = famDoc.FamilyCreate.NewCurveByPoints(rpa1);
                    ref_ar.Append(cbp.GeometryCurve.Reference);
                    ref_ar_ar.Append(ref_ar);
                    rpa1.Clear();
                    ref_ar = new ReferenceArray();
                }

                try
                {
                    loftForm = famDoc.FamilyCreate.NewLoftForm(true, ref_ar_ar);
                    trans.Commit();

                    msg = "Done";
                }

                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            
            SaveAsOptions sao = new SaveAsOptions();
            sao.OverwriteExistingFile = true;
            sao.Compact = true;
            sao.MaximumBackups = 2;

            famDoc.SaveAs(filePath, sao);
            
            return msg;

        }
        #endregion
    }
}
