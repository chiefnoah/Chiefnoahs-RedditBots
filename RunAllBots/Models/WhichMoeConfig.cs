using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class WhichMoeConfig {

        private WhichMoeConfigSubreddit[] subredditsField;

        private WhichMoeConfigSummon[] summonsField;

        private WhichMoeConfigAccount accountField;

        private byte idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("subreddit", IsNullable = false)]
        public WhichMoeConfigSubreddit[] subreddits {
            get {
                return this.subredditsField;
            }
            set {
                this.subredditsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("summon", IsNullable = false)]
        public WhichMoeConfigSummon[] summons {
            get {
                return this.summonsField;
            }
            set {
                this.summonsField = value;
            }
        }

        /// <remarks/>
        public WhichMoeConfigAccount Account {
            get {
                return this.accountField;
            }
            set {
                this.accountField = value;
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WhichMoeConfigSubreddit {

        private bool enabledField;

        private bool allownsfwField;

        private string valueField;

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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool allownsfw {
            get {
                return this.allownsfwField;
            }
            set {
                this.allownsfwField = value;
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

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WhichMoeConfigSummon {

        private bool enabledField;

        private string valueField;

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

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WhichMoeConfigAccount {

        private string usernameField;

        private string passwordField;

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
    }



}
