using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WebBanThuoc {
    public class DominantColour {

        static string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"objects\images\output");
        static string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"objects\images");

        public List<Color> GetDominantColour(string inputFile, bool isFound)
        {
            const int k = 15;
            string imgPath = "";
            try
            {
                imgPath = isFound ? Path.Combine(outputFolder, inputFile) : Path.Combine(imagesFolder, inputFile);
            }
            catch (Exception ex)
            {
                return new List<Color>();
            }

            Debug.WriteLine("GetDominantColour: " + imgPath);

            using (Image image = Image.FromFile(imgPath)) {
                const int maxResizedDimension = 200;
                const int EuclideanDistance = 130;
                Size resizedSize;
                if (image.Width > image.Height) {
                    resizedSize = new Size(maxResizedDimension, (int)Math.Floor((image.Height / (image.Width * 1.0f)) * maxResizedDimension));
                } else {
                    resizedSize = new Size((int)Math.Floor((image.Width / (image.Width * 1.0f)) * maxResizedDimension), maxResizedDimension);
                }

                using (Bitmap resized = new Bitmap(image, resizedSize)) {
                    List<Color> colors = new List<Color>(resized.Width * resized.Height);
                    for (int x = 0; x < resized.Width; x++) {
                        for (int y = 0; y < resized.Height; y++) {
                            colors.Add(resized.GetPixel(x, y));
                        }
                    }

                    KMeansClusteringCalculator clustering = new KMeansClusteringCalculator();
                    IList<Color> dominantColours = clustering.Calculate(k, colors, 5.0d);
                    List<Color> filtered = new List<Color>();


                    foreach (Color color in dominantColours) {
                        if ((KCluster.EuclideanDistance(color, Color.Black) >= EuclideanDistance) && (KCluster.EuclideanDistance(color, Color.White) >= EuclideanDistance)) {
                            filtered.Add(color);
                        }
                    }

                    return filtered;
                }
            }
        }
    }
}