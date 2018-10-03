using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PengSW.NotifyPropertyChanged;
using PengSW.TimeHelper;

namespace wxImageFileTime
{
    class wxImageFileTimeModel : NotifyPropertyChangedObject
    {

        interface IRenamer
        {
            string GetNewName(string aFileName);
        }

        class Renamer1 : IRenamer
        {
            private static readonly Regex _Regex = new Regex(@"^.*(\d{13}).*(\.\w*)$");

            public string GetNewName(string aFileName)
            {
                Match aMatch = _Regex.Match(aFileName);
                if (aMatch == null || !aMatch.Success) return null;
                long aTimeStamp = long.Parse(aMatch.Groups[1].Value);
                DateTime aTime = aTimeStamp.UnixTimeToTime_ms();
                string aExtension = aMatch.Groups[2].Value;
                return $"{aTime:yyyyMMdd.HHmmss}{aExtension}";
            }
        }

        class Renamer2 : IRenamer
        {
            private static readonly Regex _Regex = new Regex(@"^.*(\d{8})[^\d](\d{6}).*(\.\w*)$");

            public string GetNewName(string aFileName)
            {
                Match aMatch = _Regex.Match(aFileName);
                if (aMatch == null || !aMatch.Success) return null;
                return $"{aMatch.Groups[1].Value}.{aMatch.Groups[2].Value}{aMatch.Groups[3].Value}";
            }
        }

        private static readonly IRenamer[] _Renamers = new IRenamer[] { new Renamer1(), new Renamer2() };

        public class Item : NotifyPropertyChangedObject
        {
            public Item(string aFileName)
            {
                SourceFullFileName = aFileName;
                SourceFileName = Path.GetFileName(SourceFullFileName);
                foreach (IRenamer aRenamer in _Renamers)
                {
                    if ((TargetFileName = aRenamer.GetNewName(aFileName)) != null) break;
                }
                if (TargetFileName == null) TargetFileName = SourceFileName;
            }
            public string SourceFullFileName { get; }
            public string SourceFileName { get; }

            public string TargetFileName { get => _TargetFileName; set { SetValue(ref _TargetFileName, value, nameof(TargetFileName), nameof(IsRenamable)); } }
            private string _TargetFileName;

            public string TargetFullFileName => Path.Combine(Path.GetDirectoryName(SourceFullFileName), TargetFileName);
            public bool IsRenamable => SourceFileName != TargetFileName && !string.IsNullOrWhiteSpace(SourceFileName) && !string.IsNullOrWhiteSpace(TargetFileName);
            
            public string RenameExceptionMessage { get => _RenameExceptionMessage; set { SetValue(ref _RenameExceptionMessage, value, nameof(RenameExceptionMessage)); } }
            private string _RenameExceptionMessage = null;
        }

        public void ClearAll()
        {
            Items.Clear();
        }

        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        public void AddFolder(string aPath)
        {
            AddFiles(Directory.GetFiles(aPath));
        }

        public void AddFiles(string[] aFileNames)
        {
            foreach (var aFileName in aFileNames)
            {
                if (Items.FirstOrDefault(r => r.SourceFullFileName == aFileName) != null) continue;
                Items.Add(new Item(aFileName));
            }
        }

        public void Start()
        {
            foreach (var aItem in Items.ToArray())
            {
                try
                {
                    string aTargetFullFileName = aItem.TargetFullFileName;
                    if (!aItem.IsRenamable)
                    {
                        Items.Remove(aItem);
                        continue;
                    }

                    if (File.Exists(aTargetFullFileName))
                    {
                        string aPrefix = Path.Combine(Path.GetDirectoryName(aTargetFullFileName), Path.GetFileNameWithoutExtension(aTargetFullFileName));
                        string aExtension = Path.GetExtension(aTargetFullFileName);
                        for (int i = 1; File.Exists(aTargetFullFileName = $"{aPrefix}.{i:00}{aExtension}"); i++) ;
                    }
                    File.Move(aItem.SourceFullFileName, aTargetFullFileName);
                    Items.Remove(aItem);
                }
                catch (Exception ex)
                {
                    aItem.RenameExceptionMessage = ex.Message;
                }
            }
        }
    }
}
