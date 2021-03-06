using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace CMScoutIntrinsic {

    class SettingsService {
        public event EventHandler MruFilesChanged;
        public event EventHandler ColumnsChanged;
        public event EventHandler Columns2Changed;
        public event EventHandler CA18ViewModeChanged;
        public event EventHandler CA18HighlightChanged;

        public SettingsService() {
        }

        public async Task ExposeAsync() {
            _settingsFilePath = Path.Combine(Helpers.LocalDirPath, "Settings.xml");
            _loadLastSave     = false;
            _applyLastFilter  = false;
            _ca18Highlight    = false;
            _ca18ViewMode     = CA18ViewMode.InMatchNormalized;
            _columns          = new List<String>();
            _columns2         = new List<String>();
            _mruFiles         = new List<MruFile>();
            _filters          = new List<Filter>();
            _weightsSets      = new List<WeightsSet>();

            await LoadSettingsAsync();

            if(_filters.Count == 0) {
                _filters.Add(
                    new Filter {
                        Name = "All Players",
                    }
                );

                _filters.Add(
                    new Filter {
                        Name            = "EU Players",
                        NationGroupName = DataService.EUNationGroupName,
                    }
                );

                _filters.Add(
                    new Filter {
                        Name     = "Barcelona Players",
                        ClubName = "F.C. Barcelona",
                    }
                );

#if DEBUG
                _filters.Add(
                    new Filter {
                        Name            = "USSR Players",
                        NationGroupName = "Soviet Union",
                    }
                );

                _filters.Add(
                    new Filter {
                        Name     = "Vityaz Krymsk Players",
                        ClubName = "Vityaz Krymsk",
                    }
                );
#endif
            }

            if(_weightsSets.Count == 0) {
                WeightsSet defaultWeightsSet = await LoadWeightsSetAsync(await StorageFile.GetFileFromPathAsync(Path.Combine(Package.Current.InstalledLocation.Path, "Resources\\WeightsSet_Default.txt")));

                defaultWeightsSet.IsLast = true;

                _weightsSets.Add(defaultWeightsSet);

                WeightsSet cmScoutWeightsSet = await LoadWeightsSetAsync(await StorageFile.GetFileFromPathAsync(Path.Combine(Package.Current.InstalledLocation.Path, "Resources\\WeightsSet_CMScout.txt")));

                _weightsSets.Add(cmScoutWeightsSet);

                WeightsSet madScientistWeightsSet = await LoadWeightsSetAsync(await StorageFile.GetFileFromPathAsync(Path.Combine(Package.Current.InstalledLocation.Path, "Resources\\WeightsSet_MadScientist.txt")));

                _weightsSets.Add(madScientistWeightsSet);
            }
        }

        public void SaveSettings() {
            XDocument doc = new XDocument();

            doc.Add(new XElement("Settings"));

            {
                XElement eLoadLastSave = new XElement("LoadLastSave");
                doc.Root.Add(eLoadLastSave);

                eLoadLastSave.SetAttributeValue("Value", _loadLastSave);
            }

            {
                XElement eApplyLastPreset = new XElement("ApplyLastFilter");
                doc.Root.Add(eApplyLastPreset);

                eApplyLastPreset.SetAttributeValue("Value", _applyLastFilter);
            }

            {
                XElement eCA18Highlight = new XElement("CA18Highlight");
                doc.Root.Add(eCA18Highlight);

                eCA18Highlight.SetAttributeValue("Value", _ca18Highlight);
            }

            {
                XElement eCA18ViewMode = new XElement("CA18ViewMode");
                doc.Root.Add(eCA18ViewMode);

                eCA18ViewMode.SetAttributeValue("Value", _ca18ViewMode.ToString());
            }

            {
                XElement eColumns = new XElement("Columns");
                doc.Root.Add(eColumns);

                foreach(String column in _columns) {
                    XElement eColumn = new XElement("Column");
                    eColumns.Add(eColumn);
                        
                    eColumn.SetAttributeValue("Name", column);
                }
            }

            {
                XElement eColumns2 = new XElement("Columns2");
                doc.Root.Add(eColumns2);

                foreach(String column2 in _columns2) {
                    XElement eColumn2 = new XElement("Column2");
                    eColumns2.Add(eColumn2);
                        
                    eColumn2.SetAttributeValue("Name", column2);
                }
            }

            {
                XElement eMruFiles = new XElement("MruFiles");
                doc.Root.Add(eMruFiles);

                foreach(MruFile mruFile in _mruFiles) {
                    XElement eMruFile = new XElement("MruFile");
                    eMruFiles.Add(eMruFile);
                        
                    eMruFile.SetAttributeValue("Token", mruFile.Token);
                }
            }

            {
                XElement eFilters = new XElement("Filters");
                doc.Root.Add(eFilters);

                foreach(Filter filter in _filters) {
                    XElement eFilter = new XElement("Filter");
                    eFilters.Add(eFilter);

                    eFilter.SetAttributeValue("IsLast",                    filter.IsLast);
                    eFilter.SetAttributeValue("Name",                      filter.Name);
                    eFilter.SetAttributeValue("PlayerName",                filter.PlayerName);
                    eFilter.SetAttributeValue("NationGroupName",           filter.NationGroupName);
                    eFilter.SetAttributeValue("ClubName",                  filter.ClubName);
                    eFilter.SetAttributeValue("DivisionName",              filter.DivisionName);
                    eFilter.SetAttributeValue("AgeFrom",                   filter.AgeFrom);
                    eFilter.SetAttributeValue("AgeTo",                     filter.AgeTo);
                    eFilter.SetAttributeValue("CAFrom",                    filter.CAFrom);
                    eFilter.SetAttributeValue("CATo",                      filter.CATo);
                    eFilter.SetAttributeValue("PAFrom",                    filter.PAFrom);
                    eFilter.SetAttributeValue("PATo",                      filter.PATo);
                    eFilter.SetAttributeValue("ValueFrom",                 filter.ValueFrom);
                    eFilter.SetAttributeValue("ValueTo",                   filter.ValueTo);
                    eFilter.SetAttributeValue("WageFrom",                  filter.WageFrom);
                    eFilter.SetAttributeValue("WageTo",                    filter.WageTo);
                    eFilter.SetAttributeValue("IsFreeTransfer",            filter.IsFreeTransfer);
                    eFilter.SetAttributeValue("IsContractExpired",         filter.IsContractExpired);
                    eFilter.SetAttributeValue("IsContractExpiring",        filter.IsContractExpiring);
                    eFilter.SetAttributeValue("IsContractUnprotected",     filter.IsContractUnprotected);
                    SaveNullableBoolean(eFilter, "IsLeavingOnBosman",      filter.IsLeavingOnBosman);
                    SaveNullableBoolean(eFilter, "IsTransferArranged",     filter.IsTransferArranged);
                    eFilter.SetAttributeValue("IsTransferListedByClub",    filter.IsTransferListedByClub);
                    eFilter.SetAttributeValue("IsTransferListedByRequest", filter.IsTransferListedByRequest);
                    eFilter.SetAttributeValue("IsListedForLoan",           filter.IsListedForLoan);

                    XElement eSides = new XElement("Sides");
                    eFilter.Add(eSides);

                    foreach(Boolean side in filter.Sides) {
                        XElement eSide = new XElement("Side");
                        eSides.Add(eSide);
                        
                        eSide.SetAttributeValue("Value", side);
                    }

                    XElement ePositions = new XElement("Positions");
                    eFilter.Add(ePositions);

                    foreach(Boolean position in filter.Positions) {
                        XElement ePosition = new XElement("Position");
                        ePositions.Add(ePosition);
                        
                        ePosition.SetAttributeValue("Value", position);
                    }

                    XElement eAttributes = new XElement("Attributes");
                    eFilter.Add(eAttributes);

                    foreach(SByte attribute in filter.Attributes) {
                        XElement eAttribute = new XElement("Attribute");
                        eAttributes.Add(eAttribute);
                        
                        eAttribute.SetAttributeValue("Value", attribute);
                    }
                }
            }

            {
                XElement eWeightsSets = new XElement("WeightsSets");
                doc.Root.Add(eWeightsSets);

                foreach(WeightsSet weightsSet in _weightsSets) {
                    XElement eWeightsSet = new XElement("WeightsSet");
                    eWeightsSets.Add(eWeightsSet);

                    eWeightsSet.SetAttributeValue("IsLast", weightsSet.IsLast);
                    eWeightsSet.SetAttributeValue("Name",   weightsSet.Name);

                    XElement eWeights = new XElement("Weights");
                    eWeightsSet.Add(eWeights);

                    for(Int32 i = 0; i < weightsSet.Weights.Length; ++i) {
                        UInt16[] weights = weightsSet.Weights[i];

                        for(Int32 j = 0; j < weights.Length; ++j) {
                            UInt16 weight = weights[j];

                            XElement eWeight = new XElement("Weight");
                            eWeights.Add(eWeight);

                            eWeight.SetAttributeValue("Value", weight);
                        }
                    }
                }
            }

            {
                XElement eOpensCount = new XElement("OpensCount");
                doc.Root.Add(eOpensCount);

                eOpensCount.SetAttributeValue("Value", _opensCount);
            }

            {
                XElement eWasRateAppSuggestionShown = new XElement("WasRateAppSuggestionShown");
                doc.Root.Add(eWasRateAppSuggestionShown);

                eWasRateAppSuggestionShown.SetAttributeValue("Value", _wasRateAppSuggestionShown);
            }

            using(FileStream stream = new FileStream(_settingsFilePath, FileMode.Create, FileAccess.Write)) {
                doc.Save(stream);
            }
        }

        private async Task LoadSettingsAsync() {
            if(File.Exists(_settingsFilePath)) {
                using(FileStream stream = new FileStream(_settingsFilePath, FileMode.Open, FileAccess.Read)) {
                    XDocument doc = XDocument.Load(stream);

                    {
                        _loadLastSave = (Boolean)doc.Root.Element("LoadLastSave").Attribute("Value");
                    }

                    {
                        _applyLastFilter = (Boolean)doc.Root.Element("ApplyLastFilter").Attribute("Value");
                    }

                    {
                        _ca18Highlight = (Boolean)doc.Root.Element("CA18Highlight").Attribute("Value");
                    }

                    {
                        _ca18ViewMode = (CA18ViewMode)Enum.Parse(typeof(CA18ViewMode), (String)doc.Root.Element("CA18ViewMode").Attribute("Value"));
                    }

                    {
                        foreach(XElement eColumn in doc.Root.Element("Columns").Elements("Column")) {
                            String column = (String)eColumn.Attribute("Name");

                            _columns.Add(column);
                        }
                    }

                    {
                        XElement eColumns2 = doc.Root.Element("Columns2");

                        if(eColumns2 != null) {
                            foreach(XElement eColumn2 in eColumns2.Elements("Column2")) {
                                String column2 = (String)eColumn2.Attribute("Name");

                                _columns2.Add(column2);
                            }
                        }
                        else {
                            _columns2.Add("Current Ability");
                            _columns2.Add("Potential Ability");
                        }
                    }

                    {
                        foreach(XElement eMruFile in doc.Root.Element("MruFiles").Elements("MruFile")) {
                            String token = (String)eMruFile.Attribute("Token");

                            try {
                                StorageFile file  = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(token);

                                if(file != null) {
                                    _mruFiles.Add(new MruFile(file, token));
                                }
                            }
                            catch(Exception) {
                            }
                        }
                    }

                    {
                        foreach(XElement eFilter in doc.Root.Element("Filters").Elements("Filter")) {
                            Filter filter = new Filter();

                            filter.IsLast          = (Boolean)eFilter.Attribute("IsLast");
                            filter.Name            = (String)eFilter.Attribute("Name");
                            filter.PlayerName      = (String)eFilter.Attribute("PlayerName");
                            filter.NationGroupName = (String)eFilter.Attribute("NationGroupName");
                            filter.ClubName        = (String)eFilter.Attribute("ClubName");
                            filter.AgeFrom         = (Int32)eFilter.Attribute("AgeFrom");
                            filter.AgeTo           = (Int32)eFilter.Attribute("AgeTo");

                            if(eFilter.Attribute("DivisionName")              != null) { filter.DivisionName              = (String)eFilter.Attribute("DivisionName");               }
                            if(eFilter.Attribute("CAFrom")                    != null) { filter.CAFrom                    = (Int32)eFilter.Attribute("CAFrom");                      }
                            if(eFilter.Attribute("CATo")                      != null) { filter.CATo                      = (Int32)eFilter.Attribute("CATo");                        }
                            if(eFilter.Attribute("PAFrom")                    != null) { filter.PAFrom                    = (Int32)eFilter.Attribute("PAFrom");                      }
                            if(eFilter.Attribute("PATo")                      != null) { filter.PATo                      = (Int32)eFilter.Attribute("PATo");                        }
                            if(eFilter.Attribute("ValueFrom")                 != null) { filter.ValueFrom                 = (Int32)eFilter.Attribute("ValueFrom");                   }
                            if(eFilter.Attribute("ValueTo")                   != null) { filter.ValueTo                   = (Int32)eFilter.Attribute("ValueTo");                     }
                            if(eFilter.Attribute("WageFrom")                  != null) { filter.WageFrom                  = (Int32)eFilter.Attribute("WageFrom");                    }
                            if(eFilter.Attribute("WageTo")                    != null) { filter.WageTo                    = (Int32)eFilter.Attribute("WageTo");                      }
                            if(eFilter.Attribute("IsFreeTransfer")            != null) { filter.IsFreeTransfer            = (Boolean)eFilter.Attribute("IsFreeTransfer");            }
                            if(eFilter.Attribute("IsContractExpired")         != null) { filter.IsContractExpired         = (Boolean)eFilter.Attribute("IsContractExpired");         }
                            if(eFilter.Attribute("IsContractExpiring")        != null) { filter.IsContractExpiring        = (Boolean)eFilter.Attribute("IsContractExpiring");        }
                            if(eFilter.Attribute("IsContractUnprotected")     != null) { filter.IsContractUnprotected     = (Boolean)eFilter.Attribute("IsContractUnprotected");     }
                            if(eFilter.Attribute("IsLeavingOnBosman")         != null) { filter.IsLeavingOnBosman         = LoadNullableBoolean(eFilter, "IsLeavingOnBosman");       }
                            if(eFilter.Attribute("IsTransferArranged")        != null) { filter.IsTransferArranged        = LoadNullableBoolean(eFilter, "IsTransferArranged");      }
                            if(eFilter.Attribute("IsTransferListedByClub")    != null) { filter.IsTransferListedByClub    = (Boolean)eFilter.Attribute("IsTransferListedByClub");    }
                            if(eFilter.Attribute("IsTransferListedByRequest") != null) { filter.IsTransferListedByRequest = (Boolean)eFilter.Attribute("IsTransferListedByRequest"); }
                            if(eFilter.Attribute("IsListedForLoan")           != null) { filter.IsListedForLoan           = (Boolean)eFilter.Attribute("IsListedForLoan");           }

                            {
                                Int32 i = 0;

                                foreach(XElement eSide in eFilter.Element("Sides").Elements("Side")) {
                                    filter.Sides[i] = (Boolean)eSide.Attribute("Value");

                                    ++i;
                                }
                            }

                            {
                                Int32 i = 0;

                                foreach(XElement ePosition in eFilter.Element("Positions").Elements("Position")) {
                                    filter.Positions[i] = (Boolean)ePosition.Attribute("Value");

                                    ++i;
                                }
                            }

                            {
                                Int32 i = 0;

                                foreach(XElement eAttribute in eFilter.Element("Attributes").Elements("Attribute")) {
                                    filter.Attributes[i] = (SByte)eAttribute.Attribute("Value");

                                    ++i;
                                }
                            }

                            _filters.Add(filter);
                        }
                    }

                    {
                        foreach(XElement eWeightsSet in doc.Root.Element("WeightsSets").Elements("WeightsSet")) {
                            WeightsSet weightsSet = new WeightsSet();

                            weightsSet.IsLast = (Boolean)eWeightsSet.Attribute("IsLast");
                            weightsSet.Name   = (String)eWeightsSet.Attribute("Name");

                            List<XElement> eeWeight = eWeightsSet.Element("Weights").Elements("Weight").ToList();

                            for(Int32 i = 0; i < DataService.RatingPositions.Length; ++i) {
                                for(Int32 j = 0; j < DataService.Attributes.Length; ++j) {
                                    XElement eWeight = eeWeight[i * DataService.Attributes.Length + j];

                                    weightsSet.Weights[i][j] = UInt16.Parse((String)eWeight.Attribute("Value"));
                                }
                            }

                            _weightsSets.Add(weightsSet);
                        }
                    }

                    {
                        _opensCount = (Int32)doc.Root.Element("OpensCount").Attribute("Value");
                    }

                    {
                        _wasRateAppSuggestionShown = (Boolean)doc.Root.Element("WasRateAppSuggestionShown").Attribute("Value");
                    }

                }
            }
        }

        private void SaveNullableBoolean(XElement element, String name, Boolean? value) {
            element.SetAttributeValue(name, value == null ? String.Empty : value.Value ? "1" : "0");
        }

        private Boolean? LoadNullableBoolean(XElement element, String name) {
            String s = (String)element.Attribute(name);

            return String.IsNullOrEmpty(s) ? (Boolean?)null : s != "0" ? (Boolean?)true : (Boolean?)false;
        }

        public List<String> GetColumns() {
            return _columns;
        }

        public List<String> GetColumns2() {
            return _columns2;
        }

        public void AddColumn(String column) {
            _columns.Add(column);

            ColumnsChanged?.Invoke(this, EventArgs.Empty);

            SaveSettings();
        }

        public void AddColumn2(String column2) {
            _columns2.Add(column2);

            Columns2Changed?.Invoke(this, EventArgs.Empty);

            SaveSettings();
        }

        public void RemoveColumn(String column) {
            _columns.Remove(column);

            ColumnsChanged?.Invoke(this, EventArgs.Empty);

            SaveSettings();
        }

        public void RemoveColumn2(String column2) {
            _columns2.Remove(column2);

            Columns2Changed?.Invoke(this, EventArgs.Empty);

            SaveSettings();
        }

        public List<MruFile> GetMruFiles() {
            return _mruFiles;
        }

        public void UpdateMruFile(StorageFile file) {
            String token = null;

            Int32 i = _mruFiles.FindIndex(item => item.File.Path == file.Path);

            if(i < 0) {
                token = StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
            }
            else {
                token = _mruFiles[i].Token;

                _mruFiles.RemoveAt(i);
            }

            _mruFiles.Insert(0, new MruFile(file, token));

            MruFilesChanged?.Invoke(this, EventArgs.Empty);

            SaveSettings();
        }

        public void RemoveMruFile(StorageFile file) {
            Int32 i = _mruFiles.FindIndex(item => item.File.Path == file.Path);

            if(i >= 0) {
                StorageApplicationPermissions.MostRecentlyUsedList.Remove(_mruFiles[i].Token);

                _mruFiles.RemoveAt(i);

                MruFilesChanged?.Invoke(this, EventArgs.Empty);

                SaveSettings();
            }
        }

        public Boolean GetLoadLastSave() {
            return _loadLastSave;
        }

        public void SetLoadLastSave(Boolean value) {
            _loadLastSave = value;

            SaveSettings();
        }

        public Boolean GetApplyLastFilter() {
            return _applyLastFilter;
        }

        public void SetApplyLastFilter(Boolean value) {
            _applyLastFilter = value;

            SaveSettings();
        }

        public Boolean GetCA18Highlight() {
            return _ca18Highlight;
        }

        public void SetCA18Highlight(Boolean ca18Highlight) {
            _ca18Highlight = ca18Highlight;

            CA18HighlightChanged?.Invoke(this, EventArgs.Empty);

            SaveSettings();
        }

        public CA18ViewMode GetCA18ViewMode() {
            return _ca18ViewMode;
        }

        public void SetCA18ViewMode(CA18ViewMode ca18ViewMode) {
            _ca18ViewMode = ca18ViewMode;

            CA18ViewModeChanged?.Invoke(this, EventArgs.Empty);

            SaveSettings();
        }

        public List<Filter> GetFilters() {
            return _filters;
        }

        public List<WeightsSet> GetWeightsSets() {
            return _weightsSets;
        }

        public async Task<WeightsSet> LoadWeightsSetAsync(StorageFile file) {
            WeightsSet weightsSet = new WeightsSet();

            using(Stream stream = await file.OpenStreamForReadAsync()) {
                using(StreamReader streamReader = new StreamReader(stream)) {
                    String s = null;
                    Int32  i = 0;

                    while((s = streamReader.ReadLine()) != null) {
                        if(String.IsNullOrWhiteSpace(s)) { continue; }

                        if(String.IsNullOrEmpty(weightsSet.Name)) {
                            weightsSet.Name = s.Trim();

                            continue;
                        }

                        Match match = Regex.Match(s, "(.*?)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)");

                        if(match.Success) {
                            String name = match.Groups[1].Value;

                            weightsSet.Weights[0][i] = UInt16.Parse(match.Groups[2].Value);
                            weightsSet.Weights[1][i] = UInt16.Parse(match.Groups[3].Value);
                            weightsSet.Weights[2][i] = UInt16.Parse(match.Groups[4].Value);
                            weightsSet.Weights[3][i] = UInt16.Parse(match.Groups[5].Value);
                            weightsSet.Weights[4][i] = UInt16.Parse(match.Groups[6].Value);
                            weightsSet.Weights[5][i] = UInt16.Parse(match.Groups[7].Value);
                            weightsSet.Weights[6][i] = UInt16.Parse(match.Groups[8].Value);

                            ++i;
                        }
                    }
                }
            }

            if(String.IsNullOrEmpty(weightsSet.Name)) {
                weightsSet.Name = "Imported weights set";
            }

            for(Int32 i = 0; i < weightsSet.Weights.Length; ++i) {
                UInt16[] weights = weightsSet.Weights[i];

                Int32 n = 0;

                foreach(UInt16 b in weights) {
                    n += b;
                }

                Debug.WriteLine("{0,-2}: {1,3}", DataService.RatingPositions[i].Code, n);
            }

            return weightsSet;
        }

        public async Task SaveWeightsSetAsync(StorageFile file, WeightsSet weightsSet) {
            using(Stream stream = await file.OpenStreamForWriteAsync()) {
                using(StreamWriter streamWriter = new StreamWriter(stream)) {
                    streamWriter.WriteLine(weightsSet.Name);

                    streamWriter.Write(String.Format("{0,-20}", String.Empty));

                    for(Int32 j = 0; j < DataService.RatingPositions.Length; ++j) {
                        if(j != 0) { 
                            streamWriter.Write("    ");
                        }

                        streamWriter.Write(String.Format("{0,2}", DataService.RatingPositions[j].Code));
                    }

                    streamWriter.WriteLine();

                    streamWriter.WriteLine();

                    for(Int32 i = 0; i < DataService.Attributes.Length; ++i) {
                        streamWriter.Write(String.Format("{0,-20}", DataService.Attributes[i].Name));

                        for(Int32 j = 0; j < DataService.RatingPositions.Length; ++j) {
                            if(j != 0) { 
                                streamWriter.Write("    ");
                            }

                            streamWriter.Write(String.Format("{0,2}", weightsSet.Weights[j][i].ToString()));
                        }

                        streamWriter.WriteLine();

                        if(i == 17 || i == 31) {
                            streamWriter.WriteLine();
                        }
                    }
                }
            }
        }

        public Int32 GetOpensCount() {
            return _opensCount;
        }

        public void SetOpensCount(Int32 opensCount) {
            _opensCount = opensCount;

            SaveSettings();
        }

        public Boolean WasRateAppSuggestionShown() {
            return _wasRateAppSuggestionShown;
        }

        public void SetWasRateAppSuggestionShown(Boolean wasRateAppSuggestionShown) {
            _wasRateAppSuggestionShown = wasRateAppSuggestionShown;

            SaveSettings();
        }


        
        private String           _settingsFilePath;
        private Boolean          _loadLastSave;
        private Boolean          _applyLastFilter;
        private Boolean          _ca18Highlight;
        private CA18ViewMode     _ca18ViewMode;
        private List<String>     _columns;
        private List<String>     _columns2;
        private List<MruFile>    _mruFiles;
        private List<Filter>     _filters;
        private List<WeightsSet> _weightsSets;
        private Int32            _opensCount;
        private Boolean          _wasRateAppSuggestionShown;
    }

}
