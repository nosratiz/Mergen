using System.Collections.Generic;
using System.IO;
using SkiaSharp;

namespace Mergen.Core.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        public Stream Combine(IEnumerable<Stream> files)
        {
            SKImage finalImage = null;

            //read all images into memory
            List<SKBitmap> images = new List<SKBitmap>();

            try
            {
                int width = 0;
                int height = 0;

                foreach (var file in files)
                {
                    //create a bitmap from the file and add it to the list
                    SKBitmap bitmap = SKBitmap.Decode(file);

                    //update the size of the final bitmap
                    width = bitmap.Width;
                    height = bitmap.Height;

                    images.Add(bitmap);
                }

                //get a surface so we can draw an image
                using (var tempSurface = SKSurface.Create(new SKImageInfo(width, height)))
                {
                    //get the drawing canvas of the surface
                    var canvas = tempSurface.Canvas;

                    //set background color
                    canvas.Clear(SKColors.Transparent);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    int offsetTop = 0;
                    foreach (SKBitmap image in images)
                    {
                        canvas.DrawBitmap(image, SKRect.Create(offset, offsetTop, image.Width, image.Height));
                        //offsetTop = offsetTop > 0 ? 0 : image.Height / 2;
                        //offset += (int)(image.Width / 1.6);
                    }

                    // return the surface as a manageable image
                    finalImage = tempSurface.Snapshot();
                }

                //using (SKData encoded = finalImage.Encode(SKEncodedImageFormat.Png, 100))
                //using (Stream outFile = File.OpenWrite("stitchedImage.png"))
                //{
                //    encoded.SaveTo(ms);
                //}

                var ms = new MemoryStream();
                finalImage.Encode(SKEncodedImageFormat.Png, 75).SaveTo(ms);
                ms.Position = 0;
                return ms;
                //return the image that was just drawn
            }
            finally
            {
                //clean up memory
                foreach (SKBitmap image in images)
                    image.Dispose();

                finalImage?.Dispose();
            }
        }
    }
}