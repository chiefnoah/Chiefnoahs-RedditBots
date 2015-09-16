using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RedditBots {


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "anime", Namespace = "", IsNullable = false)]
    public partial class MyAnimeListAnime {

        private animeEntry[] entryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("entry")]
        public animeEntry[] entry {
            get {
                return this.entryField;
            }
            set {
                this.entryField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class animeEntry {

        private ushort idField;

        private string titleField;

        private string englishField;

        private string synonymsField;

        private byte episodesField;

        private decimal scoreField;

        private string typeField;

        private string statusField;

        private string start_dateField;

        private string end_dateField;

        private string synopsisField;

        private string imageField;

        /// <remarks/>
        public ushort id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public string english {
            get {
                return this.englishField;
            }
            set {
                this.englishField = value;
            }
        }

        /// <remarks/>
        public string synonyms {
            get {
                return this.synonymsField;
            }
            set {
                this.synonymsField = value;
            }
        }

        /// <remarks/>
        public byte episodes {
            get {
                return this.episodesField;
            }
            set {
                this.episodesField = value;
            }
        }

        /// <remarks/>
        public decimal score {
            get {
                return this.scoreField;
            }
            set {
                this.scoreField = value;
            }
        }

        /// <remarks/>
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string start_date {
            get {
                return this.start_dateField;
            }
            set {
                this.start_dateField = value;
            }
        }

        /// <remarks/>
        public string end_date {
            get {
                return this.end_dateField;
            }
            set {
                this.end_dateField = value;
            }
        }

        /// <remarks/>
        public string synopsis {
            get {
                return this.synopsisField;
            }
            set {
                this.synopsisField = value;
            }
        }

        /// <remarks/>
        public string image {
            get {
                return this.imageField;
            }
            set {
                this.imageField = value;
            }
        }
    }


}
