using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptivePoints
{
    public class Utils
    {
        public static CoordinateSystem DocumentTotalTransform() //from CivilConnection package
        {
            CoordinateSystem cs = null;

            Document doc = DocumentManager.Instance.CurrentDBDocument;

            ProjectLocation location = doc.ActiveProjectLocation;

            if (null != location)
            {
                Transform transform = location.GetTotalTransform();
                cs = CoordinateSystem.ByOriginVectors(transform.Origin.ToPoint(), transform.BasisX.ToVector(), transform.BasisY.ToVector(), transform.BasisZ.ToVector());
            }
            else
            {
                cs = CoordinateSystem.ByOrigin(0.0,0.0,0.0);
            }
                   
            return cs;
        }
    }
}
