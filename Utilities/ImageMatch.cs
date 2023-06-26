using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.IO;
using Accord.Math;
using Accord.Statistics.Models.Fields.Features;

namespace MissionPlanner.Utilities
{
    public class ImageMatch
    {
        public static void Match(string file1 = @"F:\Flight 4\DSC01371.JPG", string file2 = @"F:\Flight 4\DSC01372.JPG")
        {
            var file1noext = Path.GetFileNameWithoutExtension(file1);
            var file2noext = Path.GetFileNameWithoutExtension(file2);

            IntPoint[] correlationPoints1;
            IntPoint[] correlationPoints2;
        
            Bitmap img1 = (Bitmap)Bitmap.FromFile(file1);
            Bitmap img2 = (Bitmap)Bitmap.FromFile(file2);
            img1 = new Bitmap(img1, 640, 480);
            img2 = new Bitmap(img2, 640, 480);

            img1 = img1.Clone(new Rectangle(System.Drawing.Point.Empty, img1.Size), PixelFormat.Format24bppRgb);

            img2 = img2.Clone(new Rectangle(System.Drawing.Point.Empty, img2.Size), PixelFormat.Format24bppRgb);
            
            Console.WriteLine("Step 1");
            // Step 1: Detect feature points using Surf Corners Detector
            SpeededUpRobustFeaturesDetector surf = new SpeededUpRobustFeaturesDetector();

            var surfPoints1 = surf.Transform(img1);
            var surfPoints2 = surf.Transform(img2);

            if (!File.Exists(file1noext + ".surf"))
                surfPoints1.SaveJSON(file1noext + ".surf");
            if (!File.Exists(file2noext + ".surf"))
                surfPoints2.SaveJSON(file2noext + ".surf");

            // Show the marked points in the original images
            //Bitmap img1mark = new FeaturesMarker(surfPoints1, 1).Apply(img1);
            //Bitmap img2mark = new FeaturesMarker(surfPoints2, 1).Apply(img2);

            // Concatenate the two images together in a single image (just to show on screen)
            //Concatenate concatenate = new Concatenate(img1mark);
            //concatenate.Apply(img2mark).Save("test1.jpg");
        
            {
                Console.WriteLine("Step 2");
                // Step 2: Match feature points using a k-NN
                KNearestNeighborMatching matcher = new KNearestNeighborMatching(5);
                IntPoint[][] matches = matcher.Match(surfPoints1, surfPoints2);

                // Get the two sets of points
                correlationPoints1 = matches[0];
                correlationPoints2 = matches[1];

                // Concatenate the two images in a single image (just to show on screen)
                //Concatenate concat = new Concatenate(img1);
                //Bitmap img3 = concat.Apply(img2);

                // Show the marked correlation s in the concatenated image
                PairsMarker pairs = new PairsMarker(
                    correlationPoints1, // Add image1's width to the X points to show the markings correctly
                    correlationPoints2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

                //img3 = img3.Clone(new Rectangle(System.Drawing.Point.Empty, img3.Size), PixelFormat.Format24bppRgb);

                //var imgt = pairs.Apply(img3);
                //imgt.Save("test2.jpg");
            }
            if(false)
            {
                CorrelationMatching matcher = new CorrelationMatching(9);
                IntPoint[][] matches = matcher.Match(img1, img2, surfPoints1.Select(a => a.ToIntPoint()), surfPoints2.Select(a => a.ToIntPoint()));

                // Get the two sets of points
                correlationPoints1 = matches[0];
                correlationPoints2 = matches[1];             

                // Concatenate the two images in a single image (just to show on screen)
                Concatenate concat = new Concatenate(img1);
                Bitmap img3 = concat.Apply(img2);

                // Show the marked correlation s in the concatenated image
                PairsMarker pairs = new PairsMarker(
                    correlationPoints1, // Add image1's width to the X points to show the markings correctly
                    correlationPoints2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

                //img3 = img3.Clone(new Rectangle(System.Drawing.Point.Empty, img3.Size), PixelFormat.Format24bppRgb);

                //var imgt = pairs.Apply(img3);
                //imgt.Save("test2a.jpg");
            }
            
            Console.WriteLine("Step 3");
            // Step 3: Create the homography matrix using a robust estimator
            RansacHomographyEstimator ransac = new RansacHomographyEstimator(0.001, 0.99);
            var homography = ransac.Estimate(correlationPoints1, correlationPoints2);

            // Plot RANSAC results against correlation results
            IntPoint[] inliers1 = correlationPoints1.Get(ransac.Inliers);
            IntPoint[] inliers2 = correlationPoints2.Get(ransac.Inliers);

            int f = 1;
            while (inliers1.Length < 30 && f < 30)
            {
                ransac = new RansacHomographyEstimator(0.001 + (f * 0.001), 0.99);
                homography = ransac.Estimate(correlationPoints1, correlationPoints2);
                inliers1 = correlationPoints1.Get(ransac.Inliers);
                inliers2 = correlationPoints2.Get(ransac.Inliers);
                f++;
            }

            //inliers1.SaveJSON(file1noext + "-" + file2noext + ".ransac");
            //inliers2.SaveJSON(file2noext + "-" + file1noext + ".ransac");

            // Concatenate the two images in a single image (just to show on screen)
            Concatenate concat2 = new Concatenate(img1);
            Bitmap img4 = concat2.Apply(img2);

            // Show the marked correlations in the concatenated image
            PairsMarker pairs2 = new PairsMarker(
                inliers1, // Add image1's width to the X points to show the markings correctly
                inliers2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

            pairs2.Apply(img4).Save(file1noext + "-" + file2noext + "-3.jpg");

            Console.WriteLine("Step 4 X:" + homography.OffsetX + " Y:" + homography.OffsetY + " length:" + Math.Sqrt(Math.Pow(homography.OffsetX, 2) + Math.Pow(homography.OffsetY, 2)));

            homography.SaveJSON(file1noext + "-" + file2noext + ".homo");

            //
            if (Math.Abs(homography.Elements[0]) < 0.4 || Math.Abs(homography.Elements[4]) < 0.4)
                return;

            // Step 4: Project and blend the second image using the homography
            Blend blend = new Blend(homography, img1);
            blend.Apply(img2).Save(file1noext + "-" + file2noext + ".jpg");

            img1.Dispose();
            img2.Dispose();
        }
    }
}
