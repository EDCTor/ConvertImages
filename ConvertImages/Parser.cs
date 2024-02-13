using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ConvertImages
{
    public class Parser
    {
        public static MinimalReadInfo? ParseFile(FileInfo file)
        {
            try
            {
                 StringBuilder processed_file = new StringBuilder();
                using (StreamReader sr = new StreamReader(file.FullName))
                {
                    string? line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // this check is due to the different ways in which files can be created

                        //Check if the line contains the XML declaration, if so get the index of the beginning of the declaration
                        int xmlDeclIndex = line.IndexOf("<?xml", StringComparison.OrdinalIgnoreCase);
                        //If  the line has the XML declaration then we will want to skip over it
                        if (xmlDeclIndex >= 0)
                        {
                            //Skip over the xml declaration for the line
                            int xmlDeclEndIndex = line.IndexOf("?>", xmlDeclIndex);
                            if (xmlDeclEndIndex >= 0)
                            {
                                line = line.Substring(xmlDeclEndIndex + 2); //add 2 so that we skip over the ?> and get the rest of the line
                            }
                        }
                        processed_file.Append(line);
                    }
                }

                XmlDocument document = new XmlDocument();
                document.Load(new StringReader(processed_file.ToString()));

                return ParseRecordFromXML(document);
            }
            catch
            {
                return null;
            }
        }

        public static MinimalReadInfo? ParseRecordFromXML(XmlDocument document)
        {
            if (document == null ||
                document.DocumentElement == null)
            {
                throw new NotSupportedException("XML document is null");
            }

            if (document != null &&
                document.DocumentElement != null &&
                string.IsNullOrEmpty(document.DocumentElement.Name))
            {
                throw new NotSupportedException("XML document must have a Name in order to classify the document type");
            }

            return new MinimalReadInfo(document);
        }
    }

    public class MinimalReadInfo
    {
        public string guid = string.Empty;
        public string plateNumber = string.Empty;
        public string contextImage = string.Empty;
        public string plateImage = string.Empty;

        public MinimalReadInfo() { }

        public MinimalReadInfo(XmlDocument document)
        {
            switch (document.DocumentElement.Name)
            {
                case "Hotlist":
                    {
                        // get the root node in order to fetch the guid, there is just one of these
                        XmlNodeList root = document.SelectNodes("/Hotlist");

                        foreach (XmlNode node in root)
                        {
                            this.guid = node.SelectSingleNode("HitID").InnerText;
                            break;
                        }

                        // get the read, a hotlist hit only has one read
                        XmlNodeList read = document.SelectNodes("/Hotlist/Vehicle/Read");

                        foreach (XmlNode node in read)
                        {
                            this.plateNumber = node.SelectSingleNode("Plate").InnerText;
                            this.plateImage = node.SelectSingleNode("PlateImage").InnerText;
                            this.contextImage = node.SelectSingleNode("ContextImage").InnerText;
                            break;
                        }
                        break;
                    }
                case "SharedPermitHit":
                    {
                        // get the root node in order to fetch the guid, there is just one of these
                        XmlNodeList root = document.SelectNodes("/SharedPermitHit");

                        foreach (XmlNode node in root)
                        {
                            this.guid = node.SelectSingleNode("HitID").InnerText;
                            break;
                        }

                        // get the read, a shared permit hit has two but we just take the first one
                        XmlNodeList read = document.SelectNodes("/SharedPermitHit/FirstVehicle/Read");

                        foreach (XmlNode node in read)
                        {
                            this.plateNumber = node.SelectSingleNode("Plate").InnerText;
                            this.plateImage = node.SelectSingleNode("PlateImage").InnerText;
                            this.contextImage = node.SelectSingleNode("ContextImage").InnerText;
                            break;
                        }
                        break;
                    }
                case "PermitHit":
                    {
                        // get the root node in order to fetch the guid, there is just one of these
                        XmlNodeList root = document.SelectNodes("/PermitHit");

                        foreach (XmlNode node in root)
                        {
                            this.guid = node.SelectSingleNode("HitID").InnerText;
                            break;
                        }

                        // get the read, a permit hit only has one read
                        XmlNodeList read = document.SelectNodes("/PermitHit/Hit/Read");

                        foreach (XmlNode node in read)
                        {
                            this.plateNumber = node.SelectSingleNode("Plate").InnerText;
                            this.plateImage = node.SelectSingleNode("PlateImage").InnerText;
                            this.contextImage = node.SelectSingleNode("ContextImage").InnerText;
                            break;
                        }
                        break;
                    }
                case "OvertimeHit":
                    {
                        throw new NotImplementedException();
                    }
                case "Read":
                    {
                        // get the root node in order to fetch the guid, there is just one of these
                        XmlNodeList root = document.SelectNodes("/Read");

                        foreach (XmlNode node in root)
                        {
                            //this.guid = node.SelectSingleNode("HitID").InnerText;
                            this.plateNumber = node.SelectSingleNode("Plate").InnerText;
                            this.plateImage = node.SelectSingleNode("PlateImage").InnerText;
                            this.contextImage = node.SelectSingleNode("ContextImage").InnerText;
                            break;
                        }

                        break;
                    }
                case "InLotViolation":
                    {
                        // get the read, a permit hit only has one read
                        XmlNodeList read = document.SelectNodes("/InLotViolation/EntranceRead");

                        foreach (XmlNode node in read)
                        {
                            this.plateNumber = node.SelectSingleNode("PlateNumber").InnerText;
                            this.plateImage = node.SelectSingleNode("LprImage").InnerText;
                            this.contextImage = node.SelectSingleNode("ContextImage").InnerText;
                            break;
                        }
                        break;
                    }
                case "OccupancyExport":
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

    }

}
