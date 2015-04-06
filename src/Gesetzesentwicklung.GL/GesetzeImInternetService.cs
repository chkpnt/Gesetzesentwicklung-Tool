using Gesetzesentwicklung.GII;
using Gesetzesentwicklung.Markdown;
using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GL
{
    public class GesetzeImInternetService
    {
        public async Task<Gesetzesverzeichnis> GetGesetzesverzeichnis()
        {
            var verzeichnisLader = new XmlVerzeichnisService();
            return await verzeichnisLader.LadeVerzeichnis();
        }

        internal async Task<Gesetz> LadeGesetzAusNormZip(Gesetzesverzeichnis.Norm norm)
        {
            using (var webclient = new WebClient())
            {
                var zipStream = await webclient.OpenReadTaskAsync(norm.Link);
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    using (var fileStream = archive.Entries.Single().Open())
                    {
                        var xmlGesetzService = new XmlGesetzService();
                        var gesetz = xmlGesetzService.ParseXml(fileStream);
                        return gesetz;
                    }
                }
            }
        }

        public async void GenerateMarkdown(Gesetzesverzeichnis.Norm norm)
        {
            var gesetz = await LadeGesetzAusNormZip(norm);
            var commitSetting = new CommitSetting
            {
                Autor = "Foo Bar <foo@bar.net>",
                Beschreibung = "Bla blub",
                Datum = DateTime.Parse("1960-10-08")
            };
            var markdownGenerator = new MarkdownGenerator();
            markdownGenerator.generate(gesetz, commitSetting, @"C:\tmp");
        }
    }
}
