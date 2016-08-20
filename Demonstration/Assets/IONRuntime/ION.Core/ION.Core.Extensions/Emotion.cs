using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace ION.Core.Extensions
{
    public class Emotion
    {
        public enum Valence { negative = 0, positive = 1 };

        public const string LOVE_EMOTION = "Love";
        public const string HATE_EMOTION = "Hate";
        public const string HOPE_EMOTION = "Hope";
        public const string FEAR_EMOTION = "Fear";
        public const string SATISFACTION_EMOTION = "Satisfaction";
        public const string RELIEF_EMOTION = "Relief";
        public const string FEARS_CONFIRMED_EMOTION = "Fears-Confirmed";
        public const string DISAPPOINTMENT_EMOTION = "Disappointment";
        public const string JOY_EMOTION = "Joy";
        public const string DISTRESS_EMOTION = "Distress";
        public const string HAPPY_FOR_EMOTION = "Happy-For";
        public const string PITTY_EMOTION = "Pitty";
        public const string RESENTMENT_EMOTION = "Resentment";
        public const string GLOATING_EMOTION = "Gloating";
        public const string PRIDE_EMOTION = "Pride";
        public const string SHAME_EMOTION = "Shame";
        public const string GRATIFICATION_EMOTION = "Gratification";
        public const string REMORSE_EMOTION = "Remorse";
        public const string ADMIRATION_EMOTION = "Admiration";
        public const string REPROACH_EMOTION = "Reproach";
        public const string GRATITUDE_EMOTION = "Gratitude";
        public const string ANGER_EMOTION = "Anger";

        public static Emotion ParseEmotion(XmlNode emotionXml)
        {
            XmlAttribute aux;
            Emotion em = new Emotion();

            aux = emotionXml.Attributes["type"];
            if (aux != null)
            {
                em.type = aux.InnerXml;
            }

            aux = emotionXml.Attributes["valence"];
            if (aux != null)
            {
                int val = Int32.Parse(aux.InnerXml);
                em.valence = (Valence)val;
            }

            aux = emotionXml.Attributes["cause"];
            if (aux != null)
            {
                em.cause = aux.InnerXml;
            }

            aux = emotionXml.Attributes["direction"];
            if (aux != null)
            {
                em.direction = aux.InnerXml;
            }

            aux = emotionXml.Attributes["intensity"];
            if (aux != null)
            {
                em.intensity = Convert.ToSingle(aux.InnerXml, CultureInfo.InvariantCulture);
            }

            return em;

        }

        private string type;
        private Valence valence;
        private string cause;
        private string direction;
        private float intensity;

        public Emotion()
        {
        }

        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public string Cause
        {
            get { return this.cause; }
            set { this.cause = value; }
        }

        public Valence EmotionValence
        {
            get { return this.valence; }
            set { this.valence = value; }
        }

        public string Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        public float Intensity
        {
            get { return this.intensity; }
            set { this.intensity = value; }
        }

        public string ToXml()
        {
            return "<Emotion type=\"" + this.type +
                   "\" valence=\"" + this.valence +
                   "\" cause=\"" + this.cause +
                   "\" direction=\"" + this.direction +
                   "\" intensity=\"" + this.intensity + "\" />";
        }
    }
}
