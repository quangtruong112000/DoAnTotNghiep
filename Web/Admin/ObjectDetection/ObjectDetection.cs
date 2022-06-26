using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using WebBanThuoc.YoloParser;
using WebBanThuoc.DataStructures;
using Microsoft.ML;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace WebBanThuoc
{
    public class ObjectDetection
    {
        // Initialize MLContext
        MLContext mlContext = new MLContext();
        static string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "objects");
        static string modelFilePath = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx");
        static string imagesFolder = Path.Combine(assetsPath, "images");
        static string outputFolder = Path.Combine(assetsPath, "images", "output");


        public List<string> DrawBoundingBox(string inputImageLocation, string outputImageLocation, string imageName, IList<YoloBoundingBox> filteredBoundingBoxes)
        {
            List<string> result = new List<string>();
            Image image = Image.FromFile(Path.Combine(inputImageLocation, imageName));

            var originalImageHeight = image.Height;
            var originalImageWidth = image.Width;

            if (!Directory.Exists(outputImageLocation))
            {
                Directory.CreateDirectory(outputImageLocation);
            }

            foreach (var box in filteredBoundingBoxes)
            {

                // Get Bounding Box Dimensions
                var x = (uint)Math.Max(box.Dimensions.X, 0);
                var y = (uint)Math.Max(box.Dimensions.Y, 0);
                var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
                var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

                // Resize To Image
                x = (uint)originalImageWidth * x / OnnxModelScorer.ImageNetSettings.imageWidth;
                y = (uint)originalImageHeight * y / OnnxModelScorer.ImageNetSettings.imageHeight;
                width = (uint)originalImageWidth * width / OnnxModelScorer.ImageNetSettings.imageWidth;
                height = (uint)originalImageHeight * height / OnnxModelScorer.ImageNetSettings.imageHeight;

                var xCrop = (int)x;
                var yCrop = (int)y;
                var wCrop = (int)width;
                var hCrop = (int)height;


                // Bounding Box Text
                string text = $"{box.Label} ({(box.Confidence * 100).ToString("0")}%)";
                Debug.WriteLine(text);
                using (Graphics thumbnailGraphic = Graphics.FromImage(image))
                {
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;


                    Bitmap crop = CropImage(new Bitmap(image), new Rectangle(xCrop, yCrop, wCrop, hCrop));
                    crop.Save(Path.Combine(outputImageLocation, imageName));

                    result.Add(box.Label);

                }
            }

            return result;
        }

        public List<string> run(string fileName)
        {

            List<string> result = new List<string>();
            Debug.WriteLine(fileName);
            Debug.WriteLine(imagesFolder);

            try
            {

                // Load Data
                IEnumerable<ImageNetData> images = ImageNetData.ReadFromFile(imagesFolder, fileName);
                IDataView imageDataView = mlContext.Data.LoadFromEnumerable(images);

                // Create instance of model scorer
                var modelScorer = new OnnxModelScorer(imagesFolder, modelFilePath, mlContext);

                // Use model to score data
                IEnumerable<float[]> probabilities = modelScorer.Score(imageDataView);

                // Post-process model output
                YoloOutputParser parser = new YoloOutputParser();

                var boundingBoxes =
                    probabilities
                    .Select(probability => parser.ParseOutputs(probability))
                    .Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));


                // Draw bounding boxes for detected objects in each of the images
                for (var i = 0; i < images.Count(); i++)
                {
                    string imageFileName = images.ElementAt(i).Label;
                    IList<YoloBoundingBox> detectedObjects = boundingBoxes.ElementAt(i);

                    var labels = DrawBoundingBox(imagesFolder, outputFolder, imageFileName, detectedObjects);

                    result.AddRange(labels);
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return result;
        }

        public Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics gph = Graphics.FromImage(bmp))
            {
                gph.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmp;
        }

    }
}