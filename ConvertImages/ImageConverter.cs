using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertImages
{
    /// <summary>
    /// Looks for xml files in inputdir and converts the base64 encoded values to jpg in output images
    /// </summary>
    public class ImageConverter
    {
        private string inputDir = string.Empty;
        private string outputDir = string.Empty;

        public ImageConverter(string inputDir, string outputDir) 
        { 
            this.inputDir = inputDir;
            this.outputDir = outputDir;
        }

        public void ProcessImages()
        {
            if (!Directory.Exists(this.inputDir))
            {
                throw new DirectoryNotFoundException(this.inputDir + " Not Found");
            }

            if (!Directory.Exists(this.outputDir))
            {
                throw new DirectoryNotFoundException(this.outputDir + " Not Found");
            }

            string[] fileNames = Directory.GetFiles(this.inputDir);

            // If we find files lets try to convert them
            if (fileNames != null && fileNames.Length > 0)
            {
                int i = 0;

                foreach(string fileName in fileNames)
                {
                    i += 1;

                    // read the xml
                    FileInfo file = new FileInfo(fileName);

                    MinimalReadInfo m = Parser.ParseFile(file);

                    // find the base64 encoded field
                    // convert to an image
                    // write to disk
                    File.WriteAllBytes(
                        this.outputDir + "\\" + m.plateNumber + "." + i.ToString() + ".jpg",
                        Convert.FromBase64String(m.contextImage));

                    Console.WriteLine(fileName);
                }
            }

        }
    }
}
