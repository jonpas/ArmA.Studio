﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ArmA.Studio.Plugin;
using ArmA.Studio.UI.Attached;
using IniParser;
using IniParser.Model;
using IniParser.Parser;
using Utility;

namespace ArmA.Studio
{
    public sealed class ConfigHost
    {
        public enum EIniSelector
        {
            App,
            Coloring,
            Layout
        }
        public static class App
        {
            public static WindowState WindowCurrentState
            {
                get { WindowState state; if (Enum.TryParse(Instance.AppIni.GetValueOrNull(nameof(MainWindow), nameof(MainWindow.WindowState)), out state)) return state; return WindowState.Normal; }
                set { if (WindowCurrentState == value) return; Instance.AppIni.SetValue(nameof(MainWindow), nameof(MainWindow.WindowState), value.ToString()); Instance.Save(EIniSelector.App); }
            }
            public static double WindowHeight
            {
                get { double d; if (double.TryParse(Instance.AppIni.GetValueOrNull(nameof(MainWindow), nameof(MainWindow.Height)), out d)) return d; return -1; }
                set { if (WindowHeight == value) return; Instance.AppIni.SetValue(nameof(MainWindow), nameof(MainWindow.Height), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }
            public static double WindowWidth
            {
                get { double d; if (double.TryParse(Instance.AppIni.GetValueOrNull(nameof(MainWindow), nameof(MainWindow.Width)), out d)) return d; return -1;  }
                set { if (WindowWidth == value) return; Instance.AppIni.SetValue(nameof(MainWindow), nameof(MainWindow.Width), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }
            public static double WindowTop
            {
                get { double d; if (double.TryParse(Instance.AppIni.GetValueOrNull(nameof(MainWindow), nameof(MainWindow.Top)), out d)) return d; return -1; }
                set { if (WindowTop == value) return; Instance.AppIni.SetValue(nameof(MainWindow), nameof(MainWindow.Top), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }
            public static double WindowLeft
            {
                get { double d; if (double.TryParse(Instance.AppIni.GetValueOrNull(nameof(MainWindow), nameof(MainWindow.Left)), out d)) return d; return -1; }
                set { if (WindowLeft == value) return; Instance.AppIni.SetValue(nameof(MainWindow), nameof(MainWindow.Left), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }

            public static string WorkspacePath
            {
                get { return  Instance.AppIni.GetValueOrNull(nameof(App), nameof(Workspace)); }
                set { if (WorkspacePath != null && WorkspacePath.Equals(value)) return; Instance.AppIni.SetValue(nameof(App), nameof(Workspace), value); Instance.Save(EIniSelector.App); }
            }

            public static IEnumerable<string> RecentWorkspaces
            {
                get { string s = Instance.AppIni.GetValueOrNull(nameof(App), nameof(RecentWorkspaces)); return s != null ? s.Split(',') : new string[0]; }
                set { string s = string.Join(",", value); Instance.AppIni.SetValue(nameof(App), nameof(RecentWorkspaces), s); Instance.Save(EIniSelector.App); }
            }

            public static bool ErrorList_IsErrorsDisplayed
            {
                get { bool val; if (bool.TryParse(Instance.AppIni.GetValueOrNull(nameof(DataContext.ErrorListPane), nameof(DataContext.ErrorListPane.IsErrorsDisplayed)), out val)) return val; return true; }
                set { Instance.AppIni.SetValue(nameof(DataContext.ErrorListPane), nameof(DataContext.ErrorListPane.IsErrorsDisplayed), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }
            public static bool ErrorList_IsWarningsDisplayed
            {
                get { bool val; if (bool.TryParse(Instance.AppIni.GetValueOrNull(nameof(DataContext.ErrorListPane), nameof(DataContext.ErrorListPane.IsWarningsDisplayed)), out val)) return val; return true; }
                set { Instance.AppIni.SetValue(nameof(DataContext.ErrorListPane), nameof(DataContext.ErrorListPane.IsWarningsDisplayed), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }
            public static bool ErrorList_IsInfosDisplayed
            {
                get { bool val; if (bool.TryParse(Instance.AppIni.GetValueOrNull(nameof(DataContext.ErrorListPane), nameof(DataContext.ErrorListPane.IsInfosDisplayed)), out val)) return val; return true; }
                set { Instance.AppIni.SetValue(nameof(DataContext.ErrorListPane), nameof(DataContext.ErrorListPane.IsInfosDisplayed), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }


            public static bool UseInDevBuild
            {
                get { bool val; if (bool.TryParse(Instance.AppIni.GetValueOrNull(nameof(App), nameof(UseInDevBuild)), out val)) return val; return false; }
                set { Instance.AppIni.SetValue(nameof(App), nameof(UseInDevBuild), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }

            public static bool EnableAutoToolUpdates
            {
                get { bool val; if (bool.TryParse(Instance.AppIni.GetValueOrNull(nameof(App), nameof(EnableAutoToolUpdates)), out val)) return val; return true; }
                set { Instance.AppIni.SetValue(nameof(App), nameof(EnableAutoToolUpdates), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }
            public static bool EnableAutoPluginsUpdate
            {
                get { bool val; if (bool.TryParse(Instance.AppIni.GetValueOrNull(nameof(App), nameof(EnableAutoPluginsUpdate)), out val)) return val; return true; }
                set { Instance.AppIni.SetValue(nameof(App), nameof(EnableAutoPluginsUpdate), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }
            public static bool AutoReportException
            {
                get { bool val; if (bool.TryParse(Instance.AppIni.GetValueOrNull(nameof(App), nameof(AutoReportException)), out val)) return val; return true; }
                set { Instance.AppIni.SetValue(nameof(App), nameof(AutoReportException), value.ToString(CultureInfo.InvariantCulture)); Instance.Save(EIniSelector.App); }
            }

            /// <summary>
            /// Language id used by the application
            /// </summary>
            /// <remarks>
            /// see https://msdn.microsoft.com/de-de/library/ee825488(v=cs.20).aspx for more information
            /// </remarks>
            public static int Language
            {
                get
                {
                    int val;
                    if (int.TryParse(Instance.AppIni.GetValueOrNull(nameof(App), nameof(Language)), out val))
                    {
                        return val;
                    }
                    return CultureInfo.InstalledUICulture.LCID; // if there is no value set, return system culture
                }
                set
                {
                    if (Language != value)
                    {
                        Instance.AppIni.SetValue(nameof(App), nameof(Language), value.ToString(CultureInfo.InvariantCulture));
                        Instance.Save(EIniSelector.App);
                    }
                }
            }
        }
        public static class Coloring
        {
            public static void Reset(bool IsHard)
            {
                var coloringType = typeof(Coloring);
                var list = new List<Type>(coloringType.GetNestedTypes());
                list.Add(coloringType);
                foreach(var it in list)
                {
                    var properties = from prop in it.GetProperties() where prop.PropertyType.IsEquivalentTo(typeof(Color)) select prop;
                    foreach(var prop in properties)
                    {
                        var value = (Color)prop.GetMethod.Invoke(null, null);
                        if(IsHard || value.Equals(Colors.Transparent))
                        {
                            var fields = from field in it.GetFields() where field.FieldType.IsEquivalentTo(typeof(Color)) select field;
                            var f = fields.FirstOrDefault((field) => field.Name.EndsWith("_Default") && field.Name.StartsWith(prop.Name));
                            if (f == null)
                                throw new NotImplementedException();
                            prop.SetMethod.Invoke(null, new object[] { f.GetValue(null) });
                        }
                    }
                }

            }
            public static Color ColorParse(string inputs)
            {
                if (string.IsNullOrWhiteSpace(inputs))
                    return Colors.Transparent;
                var splitInputs = inputs.Split(',');
                if (splitInputs.Length != 4)
                    return Colors.Transparent;
                foreach (var input in splitInputs)
                {
                    if (!input.IsInteger())
                    {
                        return Colors.Transparent;
                    }
                }
                return Color.FromArgb(byte.Parse(splitInputs[0]), byte.Parse(splitInputs[1]), byte.Parse(splitInputs[2]), byte.Parse(splitInputs[3]));
            }
            public static string ColorParse(Color colorInput)
            {
                return string.Join(",", colorInput.A, colorInput.R, colorInput.G, colorInput.B);
            }

            public static class EditorUnderlining
            {
                public static readonly Color ErrorColor_Default = Color.FromArgb(255, 255, 0, 0);
                public static Color ErrorColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(EditorUnderlining), nameof(ErrorColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(EditorUnderlining), nameof(ErrorColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color WarningColor_Default = Color.FromArgb(255, 255, 153, 0);
                public static Color WarningColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(EditorUnderlining), nameof(WarningColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(EditorUnderlining), nameof(WarningColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color InfoColor_Default = Color.FromArgb(255, 0, 255, 0);
                public static Color InfoColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(EditorUnderlining), nameof(InfoColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(EditorUnderlining), nameof(InfoColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }
            }

            public static class ExecutionMarker
            {
                public static readonly Color MainColor_Default = Color.FromArgb(255, 255, 216, 0);
                public static Color MainColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(ExecutionMarker), nameof(MainColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(ExecutionMarker), nameof(MainColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color BorderColor_Default = Color.FromArgb(255, 0, 0, 0);
                public static Color BorderColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(ExecutionMarker), nameof(BorderColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(ExecutionMarker), nameof(BorderColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }
            }
            
            public static class SelectedLine
            {
                public static readonly Color Background_Default = Color.FromArgb(16, 0, 0, 0);
                public static Color Background
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SelectedLine), nameof(Background))); }
                    set { Instance.ColoringIni.SetValue(nameof(SelectedLine), nameof(Background), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color Border_Default = Color.FromArgb(32, 0, 0, 0);
                public static Color Border
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SelectedLine), nameof(Border))); }
                    set { Instance.ColoringIni.SetValue(nameof(SelectedLine), nameof(Border), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }
            }

            public static class BreakPoint
            {
                public static readonly Color MainColor_Default = Color.FromArgb(255, 255, 0, 0);
                public static Color MainColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(BreakPoint), nameof(MainColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(BreakPoint), nameof(MainColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color MainColorInactive_Default = Color.FromArgb(255, 128, 0, 0);
                public static Color MainColorInactive
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(BreakPoint), nameof(MainColorInactive))); }
                    set { Instance.ColoringIni.SetValue(nameof(BreakPoint), nameof(MainColorInactive), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color BorderColor_Default = Color.FromArgb(255, 255, 255, 255);
                public static Color BorderColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(BreakPoint), nameof(BorderColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(BreakPoint), nameof(BorderColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color TextHighlightColor_Default = Color.FromArgb(32, 200, 0, 0);
                public static Color TextHighlightColor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(BreakPoint), nameof(TextHighlightColor))); }
                    set { Instance.ColoringIni.SetValue(nameof(BreakPoint), nameof(TextHighlightColor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }
            }

            public static class SyntaxHighlightingSqf
            {
                public static readonly Color Digits_Default = Colors.Chocolate;
                public static Color Digits
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(Digits))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(Digits), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color StringNormal_Default = Colors.Crimson;
                public static Color StringNormal
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(StringNormal))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(StringNormal), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color StringSingle_Default = Colors.Crimson;
                public static Color StringSingle
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(StringSingle))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(StringSingle), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color PreProcessor_Default = Colors.Gray;
                public static Color PreProcessor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(PreProcessor))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(PreProcessor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color LineComment_Default = Colors.DarkGreen;
                public static Color LineComment
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(LineComment))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(LineComment), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color MultiLineComment_Default = Colors.DarkGreen;
                public static Color MultiLineComment
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(MultiLineComment))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(MultiLineComment), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color BinaryCommands_Default = Colors.Blue;
                public static Color BinaryCommands
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(BinaryCommands))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(BinaryCommands), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color UnaryCommands_Default = Colors.Blue;
                public static Color UnaryCommands
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(UnaryCommands))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(UnaryCommands), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color NullarCommands_Default = Colors.Blue;
                public static Color NullarCommands
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingSqf), nameof(NullarCommands))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingSqf), nameof(NullarCommands), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }
            }
            public static class SyntaxHighlightingConfig
            {
                public static readonly Color Digits_Default = Colors.Chocolate;
                public static Color Digits
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingConfig), nameof(Digits))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingConfig), nameof(Digits), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color StringNormal_Default = Colors.Crimson;
                public static Color StringNormal
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingConfig), nameof(StringNormal))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingConfig), nameof(StringNormal), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color StringSingle_Default = Colors.Crimson;
                public static Color StringSingle
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingConfig), nameof(StringSingle))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingConfig), nameof(StringSingle), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color PreProcessor_Default = Colors.Gray;
                public static Color PreProcessor
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingConfig), nameof(PreProcessor))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingConfig), nameof(PreProcessor), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color LineComment_Default = Colors.DarkGreen;
                public static Color LineComment
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingConfig), nameof(LineComment))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingConfig), nameof(LineComment), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color MultiLineComment_Default = Colors.DarkGreen;
                public static Color MultiLineComment
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingConfig), nameof(MultiLineComment))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingConfig), nameof(MultiLineComment), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }

                public static readonly Color Keywords_Default = Colors.Blue;
                public static Color Keywords
                {
                    get { return ColorParse(Instance.ColoringIni.GetValueOrNull(nameof(SyntaxHighlightingConfig), nameof(Keywords))); }
                    set { Instance.ColoringIni.SetValue(nameof(SyntaxHighlightingConfig), nameof(Keywords), ColorParse(value)); Instance.Save(EIniSelector.Coloring); }
                }
            }


        }

        public void PreparePlugin(IStorageAccessPlugin sap)
        {
            if (this.Plugins.Contains(sap))
                return;
            this.Plugins.Add(sap);
            var projectConfig = GetProjectStoragePath(sap);
            var toolConfig = GetToolStoragePath(sap);
            var parser = new FileIniDataParser();
            sap.ProjectStorage = File.Exists(projectConfig) ? parser.ReadFile(projectConfig, Encoding.UTF8) : new IniData();
            sap.ToolStorage = File.Exists(toolConfig) ? parser.ReadFile(toolConfig, Encoding.UTF8) : new IniData();
        }

        public static ConfigHost Instance { get; private set; }
        static ConfigHost()
        {
            Instance = new ConfigHost();
            Coloring.Reset(false);
        }

        public IniData LayoutIni { get; private set; }
        public IniData AppIni { get; private set; }
        public IniData ColoringIni { get; private set; }
        public IEnumerable<RealVirtuality.SQF.SqfDefinition> SqfDefinitions { get; private set; }
        public List<IStorageAccessPlugin> Plugins { get; private set; }
        private Dictionary<EIniSelector, bool> SaveTriggers;


        public ConfigHost()
        {
            this.Plugins = new List<IStorageAccessPlugin>();
            this.SaveTriggers = new Dictionary<EIniSelector, bool>();
            var parser = new FileIniDataParser();
            string fPath;

            fPath = Path.Combine(Studio.App.ConfigPath, "Layout.ini");
            this.LayoutIni = File.Exists(fPath) ? parser.ReadFile(fPath, Encoding.UTF8) : new IniData();

            fPath = Path.Combine(Studio.App.ConfigPath, "App.ini");
            this.AppIni = File.Exists(fPath) ? parser.ReadFile(fPath, Encoding.UTF8) : new IniData();

            fPath = Path.Combine(Studio.App.ConfigPath, "Coloring.ini");
            this.ColoringIni = File.Exists(fPath) ? parser.ReadFile(fPath, Encoding.UTF8) : new IniData();

            fPath = Path.Combine(Studio.App.ExecutablePath, "SqfDefinition.xml");
            this.SqfDefinitions = File.Exists(fPath) ? fPath.XmlDeserialize<List<RealVirtuality.SQF.SqfDefinition>>() : new List<RealVirtuality.SQF.SqfDefinition>();
        }
        public void SaveAll()
        {
            foreach(EIniSelector sel in Enum.GetValues(typeof(EIniSelector)))
            {
                this.Save(sel);
            }
            this.ExecSave();
        }

        public static string GetToolStoragePath(IStorageAccessPlugin p)
        {
            return Path.Combine(Studio.App.ConfigPath, string.Concat(p.GetType().FullName, ".plugin.ini"));
        }
        public static string GetProjectStoragePath(IStorageAccessPlugin p)
        {
            return Path.Combine(Workspace.Instance.ConfigPath, string.Concat(p.GetType().FullName, ".plugin.ini"));
        }

        public void Save(EIniSelector selector)
        {
            if (!Directory.Exists(Studio.App.ConfigPath))
            {
                Directory.CreateDirectory(Studio.App.ConfigPath);
            }
            this.SaveTriggers[selector] = true;
        }
        public void ExecSave()
        {
            var parser = new FileIniDataParser();
            foreach (var pair in this.SaveTriggers)
            {
                if(pair.Value)
                {
                    switch (pair.Key)
                    {
                        case EIniSelector.App:
                            parser.WriteFile(Path.Combine(Studio.App.ConfigPath, "App.ini"), this.AppIni, Encoding.UTF8);
                            break;
                        case EIniSelector.Coloring:
                            parser.WriteFile(Path.Combine(Studio.App.ConfigPath, "Coloring.ini"), this.ColoringIni, Encoding.UTF8);
                            break;
                        case EIniSelector.Layout:
                            parser.WriteFile(Path.Combine(Studio.App.ConfigPath, "Layout.ini"), this.LayoutIni, Encoding.UTF8);
                            break;
                    }
                }
            }
            foreach(var p in this.Plugins)
            {
                if(p.ProjectStorageHasChanges)
                {
                    parser.WriteFile(GetProjectStoragePath(p), p.ProjectStorage, Encoding.UTF8);
                }
                if(p.ToolStorageHasChanges)
                {
                    parser.WriteFile(GetToolStoragePath(p), p.ToolStorage, Encoding.UTF8);
                }
            }
        }
    }
}
