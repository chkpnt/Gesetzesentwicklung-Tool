using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gesetzesentwicklung.Shared;
using YamlDotNet.Serialization;
using System.Net.Mail;

namespace Gesetzesentwicklung.Models
{
    public class CommitSetting : FileSetting, IComparable<CommitSetting>
    {
        [YamlMember(Order = 0)]
        public string Daten { get; set; }

        [YamlIgnore]
        public string Ziel { get; set; }

        [YamlMember(Alias = "Ziel", Order = 1)]
        public string _Ziel
        {
            get { return Ziel?.Replace(@"\", "/").Insert(0, "/"); }
            set { Ziel = value.TrimStart('/').Replace("/", @"\"); }
        }

        // Derzeit abgeleitet über den Dateinamen
        [YamlIgnore]
        public string Branch { get; set; }

        [YamlMember(Order = 2)]
        public string BranchFrom { get; set; }

        [YamlMember(Order = 3)]
        public string MergeInto { get; set; }

        [YamlMember(Order = 4)]
        public string Tag { get; set; }

        [YamlIgnore]
        public DateTime Datum { get; set; }

        [YamlMember(Alias = "Datum", Order = 5)]
        public string _Datum
        {
            get
            {
                return Datum.Equals(default(DateTime))
                       ? null
                       : Datum.ToString("dd.MM.yyyy");
            }
            set
            {
                Datum = DateTime.ParseExact(value, "dd.MM.yyyy", null);
            }
        }

        [YamlIgnore]
        public MailAddress Autor { get; set; }

        [YamlMember(Alias="Autor", Order = 6)]
        public string _Autor
        {
            get { return Autor.ToString(); }
            set { Autor = new MailAddress(value); }
        }

        private string _beschreibung;

        // [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        [YamlMember(Order = 7)]
        public string Beschreibung
        {
            get { return _beschreibung; }
            set { _beschreibung = value.Replace("\r\n", "\n"); }
        }

        public int CompareTo(CommitSetting other) => DateTime.Compare(this.Datum, other.Datum);

        public bool Equals(CommitSetting other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(BranchFrom, other.BranchFrom)
                && string.Equals(MergeInto, other.MergeInto)
                && string.Equals(Tag, other.Tag)
                && string.Equals(Daten, other.Daten)
                && string.Equals(Autor, other.Autor)
                && DateTime.Equals(Datum, other.Datum)
                && string.Equals(Beschreibung, other.Beschreibung);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((CommitSetting)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + BranchFrom?.GetHashCode() ?? 0;
                hash = hash * 23 + MergeInto?.GetHashCode() ?? 0;
                hash = hash * 23 + Tag?.GetHashCode() ?? 0;
                hash = hash * 23 + Daten?.GetHashCode() ?? 0;
                hash = hash * 23 + Autor?.GetHashCode() ?? 0;
                hash = hash * 23 + Datum.GetHashCode();
                hash = hash * 23 + Beschreibung?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public override string ToString() => $"CommitSetting [Autor: {Autor}, Datum: {_Datum}, Beschreibung: {Beschreibung.ToLiteral()}, BranchFrom: {BranchFrom}, MergeInto: {MergeInto}]";
    }
}
