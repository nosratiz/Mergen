using System.Drawing;

namespace Mergen.Game.Api.Helper
{
    public class ImageReSizer
    {
        public static Image CropImage(Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        {
            
            Image destinationImage = new Bitmap(destinationWidth, destinationHeight);
            Graphics g = Graphics.FromImage(destinationImage);

            g.DrawImage(
                sourceImage,
                new Rectangle(15, 308, 200, 265),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel
            );
            

            return destinationImage;
        }
    }
}