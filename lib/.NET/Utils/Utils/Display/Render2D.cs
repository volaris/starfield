using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using StarfieldUtils.MathUtils;
using System.Drawing;

namespace StarfieldUtils.DisplayUtils
{
    /** <summary>    A class that renders 2D images to the 3D display. </summary> */
    public class Render2D
    {
        double minX = Double.MaxValue;
        double maxX = Double.MinValue;
        double minY = Double.MaxValue;
        double maxY = Double.MinValue;

        Vec2D[,,] voxelPixelMap;

        private StarfieldModel starfield;

        /** <summary>    Values that represent render styles. </summary> */
        public enum RenderStyle
        {
             /** <summary>    An enum constant representing the stretch to fit option. (change the aspect ratio to fill the whole display) </summary> */
            Stretch,
             /** <summary>    An enum constant representing the letter box option. (maintain aspect ratio and put black bars around the image)</summary> */
            LetterBox,
             /** <summary>    An enum constant representing the crop option. (maintain aspect ratio and cut off portions of the image that extend beyond the display)</summary> */
            Crop
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="Starfield"> The starfield to render to. </param>
         * <param name="camera">    The camera location. </param>
         */

        public Render2D(StarfieldModel Starfield, Vec3D camera)
        {
            voxelPixelMap = new Vec2D[Starfield.NumX,Starfield.NumY,Starfield.NumZ];
            this.starfield = Starfield;
            Vec3D center = new Vec3D(Starfield.NumX * Starfield.XStep / 2, Starfield.NumY * Starfield.YStep / 2, Starfield.NumZ * Starfield.ZStep / 2);
            double distance = Double.MaxValue;

            // find the closest point to the camera to use for the surface of the screen
            for (int x = 0; x < (int)Starfield.NumX; x++)
            {
                for (int y = 0; y < (int)Starfield.NumY; y++)
                {
                    for (int z = 0; z < (int)Starfield.NumZ; z++)
                    {
                        double pointDist = camera.DistanceTo(new Vec3D(x * Starfield.XStep, y * Starfield.YStep, z * Starfield.ZStep));
                        if (pointDist < distance)
                        {
                            distance = pointDist;
                        }
                    }
                }
            }

            // project all 3d points to the plane
            for (int x = 0; x < (int)Starfield.NumX; x++)
            {
                for (int y = 0; y < (int)Starfield.NumY; y++)
                {
                    for (int z = 0; z < (int)Starfield.NumZ; z++)
                    {
                        Vec3D pt = new Vec3D(x * Starfield.XStep, y * Starfield.YStep, z * Starfield.ZStep);
                        voxelPixelMap[x,y,z] = Homography.Project3Dto2DFromPoints(camera, center, pt, distance);
                    }
                }
            }

            // find the projection bounds
            for (int x = 0; x < (int)Starfield.NumX; x++)
            {
                for (int y = 0; y < (int)Starfield.NumY; y++)
                {
                    for (int z = 0; z < (int)Starfield.NumZ; z++)
                    {
                        Vec2D pt = voxelPixelMap[x, y, z];

                        if (pt.X < minX)
                        {
                            minX = pt.X;
                        }
                        if (pt.X > maxX)
                        {
                            maxX = pt.X;
                        }
                        if (pt.Y < minY)
                        {
                            minY = pt.Y;
                        }
                        if (pt.Y > maxY)
                        {
                            maxY = pt.Y;
                        }
                    }
                }
            }
        }

        /**
         * <summary>    Renders the image to the starfield using the given render style. </summary>
         *
         * <param name="img">   The image. </param>
         * <param name="style"> The render style. </param>
         */

        public void Render(Image img, RenderStyle style)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);

            int width = bmp.Width;
            int height = bmp.Height;

            double starfieldWidth = maxX - minX;
            double starfieldHeight = maxY - minY;

            double imgRatio = width / (double) height;
            double starfieldRatio = starfieldWidth / starfieldHeight;

            double yAdj = 0.0d;
            double xAdj = 0.0d;
            double x0 = minX;
            double y0 = minY;

            if (style == RenderStyle.Stretch)
            {
                yAdj = height / starfieldHeight;
                xAdj = width / starfieldWidth;
            }
            else
            {
                if (imgRatio < starfieldRatio)
                {
                    double displayWidth = starfieldHeight * imgRatio;
                    double displayHeight = starfieldHeight;

                    yAdj = height / displayHeight;
                    xAdj = width / displayWidth;

                    x0 = 0.0d - (displayWidth / 2);
                }
                else if (imgRatio > starfieldRatio)
                {
                    double displayWidth = starfieldWidth;
                    double displayHeight = starfieldWidth / imgRatio;

                    yAdj = height / displayHeight;
                    xAdj = width / displayWidth;

                    y0 = 0.0d - (displayHeight / 2);
                }
                else
                {
                    yAdj = height / starfieldHeight;
                    xAdj = width / starfieldWidth;
                }
            }

            for (int x = 0; x < (int)starfield.NumX; x++)
            {
                for (int y = 0; y < (int)starfield.NumY; y++)
                {
                    for (int z = 0; z < (int)starfield.NumZ; z++)
                    {
                        Vec3D pt = new Vec3D(x * starfield.XStep, y * starfield.YStep, z * starfield.ZStep);
                        Vec2D imgPt = voxelPixelMap[x,y,z];

                        int imgX = (int)((imgPt.X - x0) * xAdj);
                        int imgY = (int)(height - (imgPt.Y - y0) * yAdj);
                        
                        if(imgX >= 0 && imgX < width &&
                           imgY >= 0 && imgY < height)
                        {
                            starfield.SetColor(x, y, z, bmp.GetPixel(imgX, imgY));
                        }
                        else
                        {
                            starfield.SetColor(x, y, z, Color.Black);
                        }
                    }
                }
            }
        }
    }
}
