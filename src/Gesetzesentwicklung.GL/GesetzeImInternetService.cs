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
        public async Task<Gesetzesverzeichnis> GetGesetzesverzeichnisAsync()
        {
            var verzeichnisLader = new XmlVerzeichnisService();
            return await verzeichnisLader.LadeVerzeichnisAsync();
        }

        internal async Task<Gesetz> LadeGesetzAusNormZipAsync(Gesetzesverzeichnis.Norm norm)
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

        public async void GenerateMarkdownAsync(Gesetzesverzeichnis.Norm norm)
        {
            var gesetz = await LadeGesetzAusNormZipAsync(norm);
            var commitSetting = new CommitSetting
            {
                _Autor = "Foo Bar <foo@bar.net>",
                Beschreibung = "Bla blub",
                _Datum = "08.10.1960"
            };
            var markdownGenerator = new MarkdownGenerator();
            markdownGenerator.generate(gesetz, commitSetting, @"C:\tmp");
        }
    }
}
