﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ArmA.Studio.Data.IntelliSense;
using ArmA.Studio.Data.Lint;
using ArmA.Studio.Data.UI.Commands;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Utility;

namespace ArmA.Studio.Data.UI
{
    public class CodeEditorBaseDataContext : TextEditorBaseDataContext
    {
        protected const int CONST_LINTER_UPDATE_TIMEOUT_MS = 200;
        public static readonly Popup HighlightPopup = LoadFromEmbeddedResource<Popup>(typeof(CodeEditorBaseDataContext).Assembly, @"ArmA.Studio.Data.UI.CodeEditorBase_HighlightingPopup.xaml");
        public static readonly Popup IntelliSensePopup = LoadFromEmbeddedResource<Popup>(typeof(CodeEditorBaseDataContext).Assembly, @"ArmA.Studio.Data.UI.CodeEditorBase_IntelliSensePopup.xaml");

        public event EventHandler OnLintingInfoUpdated;


        public ILinterHost Linter => this as ILinterHost;
        public IIntelliSenseHost IntelliSenseHost => this as IIntelliSenseHost;

        public Task WaitTimeoutTask { get; private set; }
        public Task LinterTask { get; private set; }
        public DateTime LastTextChanged { get; private set; }

        public CodeEditorBaseDataContext(ProjectFile fileRef) : base(fileRef)
        {
            this.ShowLineNumbers = true;
        }

        protected override void OnEditorInitialized(TextEditor editor)
        {
            base.OnEditorInitialized(editor);
            editor.Document.TextChanged += OnTextChanged;
            editor.MouseHover += Editor_MouseHover;
            editor.MouseHoverStopped += Editor_MouseHoverStopped;
            this.ExecuteLinter();
        }

        private void Editor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
           HighlightPopup.IsOpen = false;
        }

        private void Editor_MouseHover(object sender, MouseEventArgs e)
        {
            if (this.Linter == null)
                return;
            var pos = e.GetPosition(this.EditorInstance);
            var textViewPosQ = this.EditorInstance.GetPositionFromPoint(pos);
            if (!textViewPosQ.HasValue)
                return;
            var textViewPos = textViewPosQ.Value;
            var offset = this.Document.GetOffset(textViewPos.Location);
            var linterInfo = this.Linter.LinterInfo.FirstOrDefault((li) => li.StartOffset <= offset && li.EndOffset >= offset);
            if (linterInfo == null)
                return;
            HighlightPopup.DataContext = linterInfo;
            HighlightPopup.PlacementTarget = this.EditorInstance;
            HighlightPopup.Placement = PlacementMode.Relative;
            HighlightPopup.HorizontalOffset = pos.X + 16;
            HighlightPopup.VerticalOffset = pos.Y - 8;
            HighlightPopup.IsOpen = true;
        }


        //ToDo: Fix error offset moving
        private void ExecuteLinter()
        {
            if (this.LinterTask != null && !this.LinterTask.IsCompleted || this.Linter == null)
                return;
            var memstream = new MemoryStream();
            try
            {
                using (var reader = this.EditorInstance.Document.CreateReader())
                {
                    //Load content into MemoryStream
                    var writer = new StreamWriter(memstream);
                    writer.Write(reader.ReadToEnd());
                    writer.Flush();
                    memstream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch
            {
                memstream.Close();
                memstream.Dispose();
            }
            this.LinterTask = Task.Run(() =>
            {
                using (memstream)
                {
                    this.Linter.LintWriteCache(memstream, this.FileReference);
                    this.OnLintingInfoUpdated?.Invoke(this, new EventArgs());
                    this.RefreshVisuals();
                    Application.Current.Dispatcher.Invoke(() => this.EditorInstance.TextArea.InvalidateVisual());
                }
            });
        }

        protected virtual void OnTextChanged(object sender, EventArgs e)
        {
            //Linting
            this.LastTextChanged = DateTime.Now;
            if (this.Linter != null)
            {
                if (this.WaitTimeoutTask == null || this.WaitTimeoutTask.IsCompleted)
                {
                    this.WaitTimeoutTask = Task.Run(() =>
                    {
                        SpinWait.SpinUntil(() => (DateTime.Now - this.LastTextChanged).TotalMilliseconds > CONST_LINTER_UPDATE_TIMEOUT_MS && (this.LinterTask == null || this.LinterTask.IsCompleted));
                        Application.Current.Dispatcher.Invoke(() => this.ExecuteLinter());
                    });
                }
            }

            //IntelliSense
            if (this.EditorInstance != null && this.IntelliSenseHost != null)
            {
                if (!this.IntelliSenseHost.IsDisplayed)
                {
                    var pos = this.EditorInstance.TextArea.TextView.GetVisualPosition(this.EditorInstance.TextArea.Caret.Position, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.TextBottom);
                    this.IntelliSenseHost.Display(this.EditorInstance, pos);
                }
                this.IntelliSenseHost.Update(this.EditorInstance.Document, this.EditorInstance.TextArea.Caret);
            }
        }
        protected override void OnCaretPositionChanged(int line, int column, int offset)
        {
            base.OnCaretPositionChanged(line, column, offset);
            this.IntelliSenseHost?.Hide();
        }
    }
}