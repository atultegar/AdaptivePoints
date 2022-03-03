using Autodesk.DesignScript.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Autodesk.DesignScript.Runtime;

namespace AdaptivePoints
{
    public class JsonPoint
    {
        [JsonProperty("x")]
        public double X { get; set; }
        [JsonProperty("y")]
        public double Y { get; set; }
        [JsonProperty("z")]
        public double Z { get; set; }

        public JsonPoint(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [IsVisibleInDynamoLibrary(true)]
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