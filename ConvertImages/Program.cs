// See https://aka.ms/new-console-template for more information
using ConvertImages;

Console.WriteLine("Starting image conversion...");

ImageConverter c = new ImageConverter(@"C:\gigator\ConvertImages\input", @"C:\gigator\ConvertImages\output");
c.ProcessImages();

