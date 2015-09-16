using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {
    public abstract class AbstractHandler {

        protected const string CONFIG_PATH = "./Bots/Data/";
        protected const string SQLITE_CONNECTION_STRING = "Data Source=BotData.db,Version=3";

        protected string retVal;

        /// <summary>
        /// Generic method configuration from an XML file
        /// </summary>
        /// <typeparam name="T">Type of object to load XML tags into</typeparam>
        /// <param name="filename">Name of the XML file to load from</param>
        /// <returns>Config file of type T</returns>
        protected T LoadConfig<T>(string filename) {
            //Check if the filename has an .xml extension, add it if it doesn't
            if (!filename.ToLower().Contains(".xml")) {
                filename += ".xml";
            }
            try {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamReader file = new System.IO.StreamReader(CONFIG_PATH + filename);
                return (T)reader.Deserialize(file);
            } catch (FileNotFoundException e) {
                retVal += "\r\nCould not load config from filename: " + filename;
                throw new FileNotFoundException("Invalid config filename");
            } catch (Exception e) {
                //I don't know what other kind of exception could be thrown
                throw new Exception(e.Message);
            }
            return default(T);
        }

    }
}
