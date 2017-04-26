# ColorGradingFilter for Monogame and XNA

A post processing filter written in c# and HLSL.
Included is a sample solution, which should ease integration into your projects.

# Color Grading / Color Correction

Color Grading is a post processing effect that changes colors based on the transformation value in a look-up table (LUT) for this specific color.
This enables very wide range of processing for specific values, and can be used to shift colors, change contrast, saturation, brightness and much much more.

Different results from this application are apparent in the image below.
![Alt text](http://i.imgur.com/jV6DWB5.png "S "Sample Application")

You can clearly see how much is possible as a result.

# LUT

LUTs (Look up tables) are used for the color correction. However, due to space and memory limitations, not all 32 bits of color are stored and recreated, instead fewer samples are used and the results are interpolated between them.

This is the default look up table for the program has a size of (64 x 64)
(http://i.imgur.com/72A4Kag.png

I use 16 values for each color and store all their permutations per default.

One can generate this lut by calling 
  _colorGradingFilter.CreateLUT(GraphicsDevice, ColorGradingFilter.LUTSizes.Size16, "LUT16.png");

LUTSize32 gives higher precision, but needs more storage space (256x256). Even higher or lower resolutions could be implemented by simply changing the Size from 32 to 64 inside the function for example, but I found these 2 sufficient.
