// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs">
//
//  The MIT License (MIT)
//
//  Copyright (c) 2014 Matt Channer
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Forms;
using CefSharp.WinForms;

namespace Embedding
{
    using System;
    using System.Drawing;
    using System.Reactive;
    using System.Threading.Tasks;

    using CefSharp;

    using Embedding.Client;

    using Widgt.Api.Shared;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly WidgtApiClient client;

        private readonly WidgtHubClient hubClient;

        private readonly ChromiumWebBrowser webBrowser;

        private readonly StringFormat renderFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="hubClient">
        /// The hub client.
        /// </param>
        public MainForm(WidgtApiClient client, WidgtHubClient hubClient)
        {
            this.client = client;
            this.hubClient = hubClient;
            this.InitializeComponent();
            this.SuspendLayout();
            this.webBrowser = new ChromiumWebBrowser(string.Empty) { Dock = DockStyle.Fill };
            this.splitContainer3.Panel1.Controls.Add(this.webBrowser);
            this.ResumeLayout(true);
            this.Font = SystemFonts.DialogFont;
            this.webBrowser.LoadError += WebBrowserOnLoadError;
            this.webBrowser.StatusMessage += WebBrowserOnStatusMessage;
            this.webBrowser.ConsoleMessage += WebBrowserOnConsoleMessage;

            renderFormat = StringFormat.GenericDefault;
            renderFormat.Alignment = StringAlignment.Near;
            renderFormat.LineAlignment = StringAlignment.Center;
            
            listBoxWidgets.DrawMode = DrawMode.OwnerDrawVariable;
            listBoxWidgets.MeasureItem += ListBoxWidgetsOnMeasureItem;
            listBoxWidgets.DrawItem += ListBoxWidgetsOnDrawItem;
            listBoxWidgets.SelectedIndexChanged += ListBoxWidgetsOnSelectedIndexChanged;

            IObserver<string> deployed = Observer.Create<string>(OnWidgetAdded);
            IObserver<string> undeployed = Observer.Create<string>(OnWidgetRemoved);

            hubClient.WidgetDeployed.Subscribe(deployed);
            hubClient.WidgetUndeployed.Subscribe(undeployed);
        }

        /// <summary>
        /// Navigates to a URL
        /// </summary>
        /// <param name="url">The URL to navigate to</param>
        public void NavigateTo(string url)
        {
            this.webBrowser.Load(url);
        }

        /// <summary>
        /// Loads the list of available widgets from the server
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to wait on. </returns>
        public async Task LoadWidgets()
        {
            var widgets = await client.GetWidgets();

            foreach (WidgtDto model in widgets)
            {
                listBoxWidgets.Items.Add(model);
            }
        }
        
        private void OnWidgetAdded(string widgetId)
        {
            richTextBoxLog.Text += "[ADDED] " + widgetId + Environment.NewLine;
        }

        private void OnWidgetRemoved(string widgetId)
        {
            richTextBoxLog.Text += "[REMOVED] " + widgetId + Environment.NewLine;
        }

        private void ListBoxWidgetsOnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                WidgtDto model = (WidgtDto)listBoxWidgets.Items[e.Index];
                e.DrawBackground();
                using (Brush b = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(model.Id, e.Font, b, e.Bounds, renderFormat);
                }

                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    e.DrawFocusRectangle();
                }
            }
        }

        private void ListBoxWidgetsOnMeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 60;
        }

        private void ListBoxWidgetsOnSelectedIndexChanged(object sender, EventArgs e)
        {
            WidgtDto model = (WidgtDto)listBoxWidgets.SelectedItem;
            this.webBrowser.Load(client.GetStartFilePathFor(model).ToString());

            this.propertyGrid.SelectedObject = model;
        }

        private void developerToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.webBrowser.ShowDevTools();
        }

        private void WebBrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs consoleMessageEventArgs)
        {
            this.InvokeOnUI(() => this.richTextBoxLog.AppendText(consoleMessageEventArgs.Message + Environment.NewLine));
        }

        private void WebBrowserOnStatusMessage(object sender, StatusMessageEventArgs statusMessageEventArgs)
        {
            this.InvokeOnUI(() => this.richTextBoxLog.AppendText(statusMessageEventArgs.Value + Environment.NewLine));
        }

        private void WebBrowserOnLoadError(object sender, LoadErrorEventArgs loadErrorEventArgs)
        {
            this.InvokeOnUI(() => this.richTextBoxLog.AppendText(loadErrorEventArgs.ErrorText + Environment.NewLine));
        }

        private void InvokeOnUI(Action act)
        {
            if (this.webBrowser.InvokeRequired)
            {
                this.webBrowser.Invoke(act);
            }
            else
            {
                act();
            }
        }
    }
}
