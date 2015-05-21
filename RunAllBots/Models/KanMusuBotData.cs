namespace RedditBots {


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class KanMusus {

        private KanMususBot[] botField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Bot")]
        public KanMususBot[] Bot {
            get {
                return this.botField;
            }
            set {
                this.botField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class KanMususBot {

        private KanMususBotSubreddit[] subredditsField;

        private string[] tagsField;

        private string[] repliesField;

        private string usernameField;

        private string passwordField;

        private byte idField;

        private bool enabledField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Subreddit", IsNullable = false)]
        public KanMususBotSubreddit[] Subreddits {
            get {
                return this.subredditsField;
            }
            set {
                this.subredditsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Tag", IsNullable = false)]
        public string[] Tags {
            get {
                return this.tagsField;
            }
            set {
                this.tagsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Reply", IsNullable = false)]
        public string[] Replies {
            get {
                return this.repliesField;
            }
            set {
                this.repliesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string username {
            get {
                return this.usernameField;
            }
            set {
                this.usernameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool enabled {
            get {
                return this.enabledField;
            }
            set {
                this.enabledField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class KanMususBotSubreddit {

        private bool searchTitleFlairField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool SearchTitleFlair {
            get {
                return this.searchTitleFlairField;
            }
            set {
                this.searchTitleFlairField = value;
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