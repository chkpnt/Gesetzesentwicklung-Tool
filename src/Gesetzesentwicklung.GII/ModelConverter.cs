using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xml2Markdown.Models;

namespace Xml2Markdown
{
    class ModelConverter
    {
        enum XmlNormenTyp
        {
            Artikel,
            Gliederungseinheit,
            Gesetzesname
        }

        internal Gesetz Convert(XmlGesetz xmlGesetz)
        {
            var artikel = convertNormen2Artikel(xmlGesetz.Normen);

            var gesetz = new Gesetz
            {
                Name = xmlGesetz.Normen.First().Metadaten.Abkuerzung,
                Artikel = artikel
            };
            return gesetz;
        }

        IEnumerable<Artikel> convertNormen2Artikel(List<XmlGesetz.Norm> normen)
        {
            Abschnitt currentAbschnitt = null;

            foreach (var norm in normen)
            {
                var normTyp = getNormTyp(norm);

                switch (normTyp)
                {
                    case XmlNormenTyp.Gliederungseinheit:
                        currentAbschnitt = new Abschnitt
                        {
                            Name = string.Format("{0} {1}",
                                norm.Metadaten.Gliederungseinheit.Bezeichnung,
                                norm.Metadaten.Gliederungseinheit.Titel)
                        };
                        break;
                    case XmlNormenTyp.Artikel:
                        yield return new Artikel
                        {
                            Abschnitt = currentAbschnitt,
                            Name = norm.Metadaten.Bezeichnung.Replace("Art", "Artikel"),
                            Inhalt = norm.Textdaten.Text
                        };
                        break;
                }
            }
        }

        XmlNormenTyp getNormTyp(XmlGesetz.Norm norm)
        {
            if (norm.Metadaten.Gliederungseinheit != null)
                return XmlNormenTyp.Gliederungseinheit;

            if (norm.Metadaten.Bezeichnung != null)
                return XmlNormenTyp.Artikel;

            return XmlNormenTyp.Gesetzesname;


        }

    }
}
