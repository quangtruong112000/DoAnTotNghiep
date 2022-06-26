using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML.Data;

namespace AppBanThuoc.ObjectDetection.DataStructures
{
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;

        public static IEnumerable<ImageNetData> ReadFromFile(string imageFolder, string fileName)
        {
            return Directory
                .GetFiles(imageFolder)
                .Where(filePath => filePath.Contains(fileName))
                .Select(filePath => new ImageNetData { ImagePath = filePath, Label = Path.GetFileName(filePath) });
        }
    }
}
