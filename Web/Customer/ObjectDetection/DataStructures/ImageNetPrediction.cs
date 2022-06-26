using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace AppBanThuoc.ObjectDetection.DataStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}
