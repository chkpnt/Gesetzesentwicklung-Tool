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
    public abstract class FileSetting
    {
        [YamlIgnore]
        public virtual string FileSettingFilename { get; set; }
    }

    public class CommitSettings : FileSetting
    {
        private string _fileSettingFilename;

        public override string FileSettingFilename
        { 
            get { return _fileSettingFilename; }
            set
            {
                _fileSettingFilename = value;
                foreach (var commit in Commits.Where(c => c.FileSettingFilename == null))
                {
                    commit.FileSettingFilename = _fileSettingFilename;
                }
            }
        }

        public List<CommitSetting> Commits { get; set; }
    }

    public class CommitSetting : FileSetting, IComparable<CommitSetting>
    {
        public string BranchFrom { get; set; }
        public string MergeInto { get; set; }
        public string Daten { get; set; }

        [YamlIgnore]
        public MailAddress Autor { get; set; }

        [YamlMember(Alias="Autor")]
        public string _Autor
        {
            get { return Autor.ToString(); }
            set { Autor = new MailAddress(value); }
        }

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
            return $"CommitSetting [Autor: {Autor}, Datum: {_Datum}, Beschreibung: {Beschreibung.ToLiteral()}, BranchFrom: {BranchFrom}, MergeInto: {MergeInto}]";
        }
    }
}
