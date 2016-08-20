using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace ION.Core.Extensions
{
    public abstract class XmlParser
    {
        private bool xmlErrors;

        protected XmlParser()
        {
            this.xmlErrors = false;
        }

        #region Errors Handling

        protected abstract void XmlErrorsHandler(object sender, ValidationEventArgs args);

        private void XmlErrorsCallBack(object sender, ValidationEventArgs args)
        {
            this.xmlErrors = true;
            this.XmlErrorsHandler(sender, args);
        }

        #endregion

        #region Parsing

        protected abstract object ParseElements(XmlDocument xml);

        protected abstract void ParseElements(XmlDocument xml, object elements);

        public object Parse(string text)
        {
            try
            {
                // convert text to a xml document
                XmlDocument xml = new XmlDocument();
                xml.Load(new XmlTextReader(new StringReader(text)));

                // parse elements
                return this.ParseElements(xml);
            }
            catch (Exception e)
            {
                // TO DO: error log
                Console.WriteLine(e);

                return null;
            }
        }

        public void Parse(string text, object elements)
        {
            try
            {
                // convert text to a xml document
                XmlDocument xml = new XmlDocument();
                xml.Load(new XmlTextReader(new StringReader(text)));

                // parse elements
                this.ParseElements(xml, elements);
            }
            catch (Exception e)
            {
                // TO DO: error log
                Console.WriteLine(e);
            }
        }

        public object ParseFile(string filename)
        {
            try
            {
                // load xml file
                XmlDocument xml = new XmlDocument();
                xml.Load(filename);

                // parse elements
                return this.ParseElements(xml);
            }
            catch (Exception e)
            {
                // TO DO: error log
                Console.WriteLine(e);

                return null;
            }
        }

        public void ParseFile(string filename, object elements)
        {
            try
            {
                // load xml file
                XmlDocument xml = new XmlDocument();
                xml.Load(filename);

                // parse elements
                this.ParseElements(xml, elements);
            }
            catch (Exception e)
            {
                // TO DO: error log
                Console.WriteLine(e);
            }
        }

        public object ParseFile(string filename, string schemaNamespace, string schemaFilename)
        {
            this.xmlErrors = false;

            // load schema
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(schemaNamespace, schemaFilename);

            // prepare xml validator
            XmlReader reader = XmlReader.Create(filename);
            reader.Settings.Schemas.Add(schema);
            reader.Settings.Schemas.ValidationEventHandler += new ValidationEventHandler(this.XmlErrorsCallBack);

            // load and validate xml file
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            // validation ok -> parse elements
            if (xmlErrors == false)
            {
                return this.ParseElements(xml);
            }
            else
            {
                return null;
            }
        }

        public void ParseFile(string filename, object elements, string schemaNamespace, string schemaFilename)
        {
            this.xmlErrors = false;

            // load schema
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(schemaNamespace, schemaFilename);

            // prepare xml validator
            XmlReader reader = XmlReader.Create(filename);
            reader.Settings.Schemas.Add(schema);
            reader.Settings.Schemas.ValidationEventHandler += new ValidationEventHandler(this.XmlErrorsCallBack);

            // load and validate xml file
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            // validation ok -> parse elements
            if (xmlErrors == false)
            {
                this.ParseElements(xml, elements);
            }
        }

        #endregion
    }
}
