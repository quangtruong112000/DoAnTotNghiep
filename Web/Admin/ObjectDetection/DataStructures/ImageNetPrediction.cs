using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace WebBanThuoc.DataStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}
