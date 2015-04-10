namespace RedditBots {
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class animetitles {

        private animetitlesAnime[] animeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("anime")]
        public animetitlesAnime[] anime {
            get {
                return this.animeField;
            }
            set {
                this.animeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class animetitlesAnime {

        private animetitlesAnimeTitle[] titleField;

        private ushort aidField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("title")]
        public animetitlesAnimeTitle[] title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort aid {
            get {
                return this.aidField;
            }
            set {
                this.aidField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class animetitlesAnimeTitle {

        private string typeField;

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang {
            get {
                return this.langField;
            }
            set {
                this.langField = value;
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