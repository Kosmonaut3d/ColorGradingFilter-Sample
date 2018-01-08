# ColorGradingFilter for Monogame and XNA (DUAL LUT BRANCH)

![Alt text](http://i.imgur.com/5wCQzCl.gif)

A post processing filter written in c# and HLSL.

Included is a sample solution, which should ease integration into your projects. I've recorded a video with a quick overview of the sample application. You can change images and LUTs with the F1 - F11 keys on your keyboard.

*Note: In this branch you can supply 2 luts and smoothly transition between them! In the sample you can move your mouse from left to right to change the transition progress.*

<a href="http://www.youtube.com/watch?feature=player_embedded&v=FA6LEo3k5FY
" target="_blank"><img src="http://img.youtube.com/vi/FA6LEo3k5FY/0.jpg" 
alt="Youtube link" width="480" height="350" border="10" /></a>



# Color Grading / Color Correction

Color Grading is a post processing effect that changes colors based on the transformation value in a look-up table (LUT) for this specific color.
This enables very wide range of processing for specific values, and can be used to shift colors, change contrast, saturation, brightness and much much more.

Different results from this application are apparent in the image below.
![Alt text](http://i.imgur.com/PrTPR1h.png "Sample Application")

You can clearly see how much is possible as a result.

For more on this check out my blog article: https://kosmonautblog.wordpress.com/2017/04/26/color-grading-correction/

# LUT

LUTs (Look up tables) are used for the color correction. However, due to space and memory limitations, not all 32 bits of color are stored and recreated, instead fewer samples are used and the results are interpolated between them.

This is the default look up table for the program has a size of (64 x 64)

![LUT](http://i.imgur.com/feLnCJ7.png)

I use 16 values for each color and store all their permutations per default.

One can generate this lut by calling 

    _colorGradingFilter.CreateLUT(GraphicsDevice, ColorGradingFilter.LUTSizes.Size16, "LUT16.png");

LUTSize32 gives higher precision, but needs more storage space (256x256). LUTSize4 and LUTSize64 are other options, but I have not found them to be useful.

# Manipulating the LUT

The easiest way for you to achieve the desired look for your application is to simply take a snapshot of a scene, open it in an image manipulation tool (like GIMP, Photoshop etc.) and paste the LUT alongside the image. Like so

![Alt text](http://i.imgur.com/lvdNSVK.png "Sample Image with LUT")

Then you can manipulate the image with any color correction your tool provides. Along with the colors of your image the LUT will be manipulated, too. Finally, just extract the modified LUT and save it as another image file which you can load into the application to achieve the desired effect. Make sure you do not change the size of the LUT, and make sure you do not use lossy formats like .jpeg to store the image. PNG, for example, is a good format.

# Integrating the filter into your project

The relevant files are found in Filters/ColorGrading and Content/Shaders/ColorGrading respectively.
* ColorGradingFilter.cs
* FullScreenQuadRenderer.cs
and
* ColorGrading.fx
* (optional) lut_default.png

A quick sample code would look like this

```
      Texture2D colorGraded = _colorGradingFilter.Draw(GraphicsDevice, myImage, _defaultLUT);
          
      //Draw to the backbuffer
      GraphicsDevice.SetRenderTarget(null);

      //Draw our images to the screen
      _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
      //The texture can be treated just as any other one.
      _spriteBatch.Draw(colorGraded, new Rectangle(0,0,Width,Height), Color.White);
      _spriteBatch.End();

```
Since this filter uses shaders, you need to enable the HiDef profile at the start of your application.

    graphics.GraphicsProfile = GraphicsProfile.HiDef;


# OpenGL

I presume this should work on OpenGL if the shader versions are changed but I have not tried out. Should someone need the OpenGL implementation I can check that out.

# Credits
All images in the tool were found on https://pixabay.com/ and are provided with the CC0 license. Note that they unfortunately are only available in .jpeg formats and so image artifacts crop up, especially when using extreme forms of color corretion.
The bloom used in the game can be found here: https://github.com/UncleThomy/BloomFilter-for-Monogame-and-XNA

All code was written by me. For feedback you can raise an issue on github or write by mail (kosmonaut3d@googlemail.com)
There is also a forum thread, which makes feedback even easier. http://community.monogame.net/t/a-color-grading-correction-filter-for-you-to-use-on-github/9106

