using Autodesk.DesignScript.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Autodesk.DesignScript.Runtime;

namespace DynaAdapt
{
    [IsVisibleInDynamoLibrary(false)]
    public class JsonPoint
    {
        [IsVisibleInDynamoLibrary(false)]
        [JsonProperty("x")]
        public double X { get; set; }
        [IsVisibleInDynamoLibrary(false)]
        [JsonProperty("y")]
        public double Y { get; set; }
        [IsVisibleInDynamoLibrary(false)]
        [JsonProperty("z")]
        public double Z { get; set; }

        [IsVisibleInDynamoLibrary(false)]
        public JsonPoint(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [IsVisibleInDynamoLibrary(false)]
        public static string SerializePointsToJSON(List<Point> points)
        {
            string serialized = "";
            List<JsonPoint> jsonPoints = new List<JsonPoint>();

            foreach (Point point in points)
            {
                JsonPoint jsonPoint = new JsonPoint(point.X, point.Y, point.Z);
                jsonPoints.Add(jsonPoint);
            }

            try
            {
                serialized = JsonConvert.SerializeObject(jsonPoints, Formatting.Indented);
            }
            catch (Exception ex)
            {
                serialized = ex.Message;
            }
            return serialized;
            
        }
    }
}