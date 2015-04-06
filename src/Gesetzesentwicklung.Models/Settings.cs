using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gesetzesentwicklung.Shared;
using YamlDotNet.Serialization;

namespace Gesetzesentwicklung.Models
{
    public class CommitSettings
    {
        public List<CommitSetting> Commits { get; set; }
    }

    public class CommitSetting : IComparable<CommitSetting>
    {
        public string BranchFrom { get; set; }
        public string MergeInto { get; set; }
        public string Daten { get; set; }
        public string Autor { get; set; }

        [YamlIgnore]
        public DateTime Datum { get; set; }

        [YamlMember(Alias="Datum")]
        public string _Datum {
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

        private string _beschreibung;

        // [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Beschreibung
        {
            get { return _beschreibung; }
            set { _beschreibung = value.Replace("\r\n", "\n"); }
        }

        public int CompareTo(CommitSetting other)
        {
            return DateTime.Compare(this.Datum, other.Datum);
        }

        public bool Equals(CommitSetting other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(BranchFrom, other.BranchFrom)
                && string.Equals(MergeInto, other.MergeInto)
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
                int hash = 17;
                hash = hash * 23 + ((BranchFrom != null) ? BranchFrom.GetHashCode() : 0);
                hash = hash * 23 + ((MergeInto != null) ? MergeInto.GetHashCode() : 0);
                hash = hash * 23 + ((Daten != null) ? Daten.GetHashCode() : 0);
                hash = hash * 23 + ((Autor != null) ? Autor.GetHashCode() : 0);
                hash = hash * 23 + ((Datum != null) ? Datum.GetHashCode() : 0);
                hash = hash * 23 + ((Beschreibung != null) ? Beschreibung.GetHashCode() : 0);
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("CommitSetting [Autor: {0}, Datum: {1}, "
            + "Beschreibung: {2}, BranchFrom: {3}, MergeInto: {4}]",
            Autor, _Datum, Beschreibung.ToLiteral(), BranchFrom, MergeInto);
        }
    }

    public class BranchesSettings
    {
        public enum BranchTyp
        {
            Feature,
            Normal
        }

        public Dictionary<string, BranchTyp> Branches { get; set; }
    }
}
