using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "manga", Namespace = "", IsNullable = false)]
    public partial class MyAnimeListManga {

        private mangaEntry[] entryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("entry")]
        public mangaEntry[] entry {
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
    public partial class mangaEntry {

        private uint idField;

        private string titleField;

        private string englishField;

        private string synonymsField;

        private byte chaptersField;

        private byte volumesField;

        private decimal scoreField;

        private string typeField;

        private string statusField;

        private string start_dateField;

        private string end_dateField;

        private mangaEntrySynopsis synopsisField;

        private string imageField;

        /// <remarks/>
        public uint id {
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
        public byte chapters {
            get {
                return this.chaptersField;
            }
            set {
                this.chaptersField = value;
            }
        }

        /// <remarks/>
        public byte volumes {
            get {
                return this.volumesField;
            }
            set {
                this.volumesField = value;
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
        public mangaEntrySynopsis synopsis {
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

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mangaEntrySynopsis {

        private mangaEntrySynopsisA aField;

        private string strongField;

        private string[] textField;

        /// <remarks/>
        public mangaEntrySynopsisA a {
            get {
                return this.aField;
            }
            set {
                this.aField = value;
            }
        }

        /// <remarks/>
        public string strong {
            get {
                return this.strongField;
            }
            set {
                this.strongField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text {
            get {
                return this.textField;
            }
            set {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mangaEntrySynopsisA {

        private string hrefField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href {
            get {
                return this.hrefField;
            }
            set {
                this.hrefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }



}
