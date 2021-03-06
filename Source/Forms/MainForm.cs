﻿/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Generator;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Miz;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.Forms
{
    public partial class MainForm : Form
    {
        private static readonly Image IMAGE_ERROR = GUITools.GetImageFromResource("Icons.Error.png");
        private static readonly Image IMAGE_INFO = GUITools.GetImageFromResource("Icons.Info.png");
        private static readonly Image IMAGE_WARNING = GUITools.GetImageFromResource("Icons.Warning.png");

        private readonly MissionGenerator Generator;
        private readonly MissionTemplate Template;
        private DCSMission Mission = null;
        
        public MainForm()
        {
            InitializeComponent();

            Generator = new MissionGenerator();
            Template = new MissionTemplate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $"BriefingRoom {BriefingRoom.BRIEFINGROOM_VERSION} for DCS World {BriefingRoom.TARGETED_DCS_WORLD_VERSION}";

#if DEBUG
            T_Debug_Export.Visible = true;
#endif

            Icon = GUITools.GetIconFromResource("Icon.ico");
            LoadIcons();
            TemplatePropertyGrid.SelectedObject = Template;
            GenerateMission();
        }

        private void LoadIcons()
        {
            M_File_New.Image = GUITools.GetImageFromResource("Icons.New.png");
            M_File_Open.Image = GUITools.GetImageFromResource("Icons.Open.png");
            M_File_SaveAs.Image = GUITools.GetImageFromResource("Icons.Save.png");
            M_File_Exit.Image = GUITools.GetImageFromResource("Icons.Exit.png");
            M_Mission_Generate.Image = GUITools.GetImageFromResource("Icons.Update.png");
            M_Mission_Export.Image = GUITools.GetImageFromResource("Icons.ExportToMiz.png");
            M_Mission_ExportBriefing.Image = GUITools.GetImageFromResource("Icons.ExportBriefing.png");

            T_About.Image = GUITools.GetImageFromResource("Icons.Info.png");
            T_File_New.Image = M_File_New.Image;
            T_File_Open.Image = M_File_Open.Image;
            T_File_SaveAs.Image = M_File_SaveAs.Image;
            T_Mission_Generate.Image = M_Mission_Generate.Image;
            T_Mission_Export.Image = M_Mission_Export.Image;
            T_Mission_ExportBriefing.Image = M_Mission_ExportBriefing.Image;
            T_Debug_Export.Image = GUITools.GetImageFromResource("Icons.DebugExport.png");
        }

        private void GenerateMission()
        {
            M_Mission_Export.Enabled = false;
            M_Mission_ExportBriefing.Enabled = false;
            T_Mission_Export.Enabled = false;
            T_Mission_ExportBriefing.Enabled = false;
            T_Debug_Export.Enabled = false;

            Mission = Generator.Generate(Template);

            if (Mission == null) // Something went wrong during the mission generation
            {
                StatusLabel.Text = DebugLog.Instance.GetErrors().Last();
                UpdateHTMLBriefing($"<p><strong>Mission generation failed</strong></p><p>{StatusLabel.Text}</p>");
                StatusLabel.Image = IMAGE_ERROR;
                return;
            }

            M_Mission_Export.Enabled = true;
            M_Mission_ExportBriefing.Enabled = true;
            T_Mission_Export.Enabled = true;
            T_Mission_ExportBriefing.Enabled = true;
            T_Debug_Export.Enabled = true;

            if (DebugLog.Instance.WarningCount > 0)
            {
                StatusLabel.Text = $"Mission generated with {DebugLog.Instance.WarningCount} warning(s), please read generation log for more information.";
                StatusLabel.Image = IMAGE_WARNING;
            }
            else
            {
                StatusLabel.Text = DebugLog.Instance.LastMessage;
                StatusLabel.Image = IMAGE_INFO;
            }

            UpdateHTMLBriefing(Mission.BriefingHTML);
        }

        private void UpdateHTMLBriefing(string html)
        {
            BriefingWebBrowser.Navigate("about:blank"); // The WebBrowser control must navigate to a new page or it won't update its content
            if (BriefingWebBrowser.Document != null) BriefingWebBrowser.Document.Write(string.Empty);
            BriefingWebBrowser.DocumentText = html;
        }

        private void MenuClick(object sender, EventArgs e)
        {
            string senderName = ((ToolStripItem)sender).Name;

            switch (senderName)
            {
                case "M_About":
                case "T_About":
                    using (AboutForm form = new AboutForm()) form.ShowDialog();
                    return;
                case "M_File_New":
                case "T_File_New":
                    Template.Clear();
                    TemplatePropertyGrid.Refresh();
                    GenerateMission();
                    return;
                case "M_File_Open":
                case "T_File_Open":
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        ofd.Filter = "BriefingRoom templates (*.brtemplate)|*.brtemplate";
                        ofd.RestoreDirectory = true;
                        ofd.InitialDirectory = BRPaths.TEMPLATES;
                        if ((ofd.ShowDialog() == DialogResult.OK) && File.Exists(ofd.FileName))
                        {
                            Template.LoadFromFile(ofd.FileName);
                            TemplatePropertyGrid.Refresh();
                            GenerateMission();
                        }
                    }
                    return;
                case "M_File_SaveAs":
                case "T_File_SaveAs":
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "BriefingRoom templates (*.brtemplate)|*.brtemplate";
                        sfd.RestoreDirectory = true;
                        sfd.InitialDirectory = BRPaths.TEMPLATES;
                        if (sfd.ShowDialog() == DialogResult.OK)
                            Template.SaveToFile(sfd.FileName);
                    }
                    return;
                case "M_File_Exit":
                    Close();
                    return;
                case "M_Mission_Export":
                case "T_Mission_Export":
                    if (Mission == null) return;
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "DCS World missions (*.miz)|*.miz";
                        sfd.FileName = Mission.MissionName + ".miz";
                        sfd.RestoreDirectory = true;
                        sfd.InitialDirectory = Toolbox.GetDCSMissionPath();
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            using (MizFile miz = Mission.ExportToMiz())
                                miz.SaveToFile(sfd.FileName);
                        }
                    }
                    return;
                case "M_Mission_ExportBriefing":
                case "T_Mission_ExportBriefing":
                    if (Mission == null) return;
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "HTML pages (*.html)|*.html";
                        sfd.FileName = Mission.MissionName + ".html";
                        sfd.RestoreDirectory = true;
                        if (sfd.ShowDialog() == DialogResult.OK)
                            File.WriteAllText(sfd.FileName, Mission.BriefingHTML);
                    }
                    return;
                case "M_Mission_Generate":
                case "T_Mission_Generate":
                    GenerateMission();
                    return;
#if DEBUG
                case "T_Debug_Export":
                    if (Mission == null) return; // No mission to export
                    Toolbox.CreateDirectoryIfMissing(BRPaths.DEBUGOUTPUT);
                    using (MizFile miz = Mission.ExportToMiz())
                        miz.SaveToDirectory(BRPaths.DEBUGOUTPUT);
                    return;
#endif
            }
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            e.Cancel =
                MessageBox.Show(
                    "Close BriefingRoom? Unsaved changes will be lost.", "Close BriefingRoom?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK;
#endif
        }

        private void TemplatePropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            GenerateMission();
        }
    }
}
