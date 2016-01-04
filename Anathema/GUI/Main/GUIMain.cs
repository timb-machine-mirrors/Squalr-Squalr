﻿using Binarysharp.MemoryManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Anathema
{
    partial class GUIMain : Form, IMainView
    {
        // All GUI components that can be created
        private GUIProcessSelector GUIProcessSelector;
        private GUIDebugger GUIDebugger;
        private GUIFilterFSM GUIFilterFSM;
        private GUIFilterManual GUIFilterManual;
        private GUIFilterTree GUIFilterTree;
        private GUIFilterChunks GUIFilterChunks;
        private GUILabelerChangeCounter GUILabelerChangeCounter;
        private GUILabelerInputCorrelator GUILabelerInputCorrelator;
        private GUISnapshotManager GUISnapshotManager;
        private GUIResults GUIResults;
        private GUITable GUITable;

        public GUIMain()
        {
            InitializeComponent();

            MainPresenter MainPresenter = new MainPresenter(this, new Main());

            // Update theme so that everything looks cool
            this.ContentPanel.Theme = new VS2013BlueTheme();

            // Initialize tools that are commonly used
            CreateDefaultTools();
        }

        /// <summary>
        /// Update the target process 
        /// </summary>
        /// <param name="ProcessTitle"></param>
        public void UpdateProcessTitle(String ProcessTitle)
        {
            // Update process text
            ControlThreadingHelper.InvokeControlAction(GUIToolStrip, () =>
            {
                ProcessTitleLabel.Text = ProcessTitle;
            });
        }

        #region Methods

        private void CreateDefaultTools()
        {
            CreateChunkScanner();
            CreateInputCorrelator();
            CreateSnapshotManager();
            CreateResults();
            CreateTable();
        }

        private void CreateDebugger()
        {
            if (GUIDebugger == null || GUIDebugger.IsDisposed)
                GUIDebugger = new GUIDebugger();
            GUIDebugger.Show(ContentPanel);
        }

        private void CreateStateScanner()
        {
            if (GUIFilterFSM == null || GUIFilterFSM.IsDisposed)
                GUIFilterFSM = new GUIFilterFSM();
            GUIFilterFSM.Show(ContentPanel);
        }

        private void CreateManualScanner()
        {
            if (GUIFilterManual == null || GUIFilterManual.IsDisposed)
                GUIFilterManual = new GUIFilterManual();
            GUIFilterManual.Show(ContentPanel);
        }

        private void CreateTreeScanner()
        {
            if (GUIFilterTree == null || GUIFilterTree.IsDisposed)
                GUIFilterTree = new GUIFilterTree();
            GUIFilterTree.Show(ContentPanel);
        }

        private void CreateChunkScanner()
        {
            if (GUIFilterChunks == null || GUIFilterChunks.IsDisposed)
                GUIFilterChunks = new GUIFilterChunks();
            GUIFilterChunks.Show(ContentPanel);
        }
        private void CreateInputCorrelator()
        {
            if (GUILabelerInputCorrelator == null || GUILabelerInputCorrelator.IsDisposed)
                GUILabelerInputCorrelator = new GUILabelerInputCorrelator();
            GUILabelerInputCorrelator.Show(ContentPanel);
        }

        private void CreateChangeCounter()
        {
            if (GUILabelerChangeCounter == null || GUILabelerChangeCounter.IsDisposed)
                GUILabelerChangeCounter = new GUILabelerChangeCounter();
            GUILabelerChangeCounter.Show(ContentPanel);
        }

        private void CreateSnapshotManager()
        {
            if (GUISnapshotManager == null || GUISnapshotManager.IsDisposed)
                GUISnapshotManager = new GUISnapshotManager();
            GUISnapshotManager.Show(ContentPanel, DockState.DockRight);
        }

        private void CreateResults()
        {
            if (GUIResults == null || GUIResults.IsDisposed)
                GUIResults = new GUIResults();
            GUIResults.Show(ContentPanel, DockState.DockRight);
        }

        private void CreateTable()
        {
            if (GUITable == null || GUITable.IsDisposed)
                GUITable = new GUITable();
            GUITable.Show(ContentPanel, DockState.DockBottom);
        }

        private void CreateProcessSelector()
        {
            if (GUIProcessSelector == null || GUIProcessSelector.IsDisposed)
                GUIProcessSelector = new GUIProcessSelector();

            GUIProcessSelector.Show(ContentPanel);
        }

        #endregion

        #region Events

        private void DebuggerToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateDebugger();
        }

        private void StateScannerToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateStateScanner();
        }

        private void ManualScannerToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateManualScanner();
        }

        private void TreeScannerToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateTreeScanner();
        }

        private void ChunkScannerToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateChunkScanner();
        }

        private void InputCorrelatorToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateInputCorrelator();
        }

        private void ChangeCounterToolStripMenuItem_Click(Object Sender, EventArgs E)
        {

            CreateChangeCounter();
        }

        private void SnapshotsToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateSnapshotManager();
        }

        private void ResultsToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateResults();
        }

        private void TableToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateTable();
        }

        private void ProcessSelectorToolStripMenuItem_Click(Object Sender, EventArgs E)
        {
            CreateProcessSelector();
        }

        private void ProcessSelectorButton_Click(Object Sender, EventArgs E)
        {
            CreateProcessSelector();
        }
        
        private void NewScanButton_Click(Object Sender, EventArgs E)
        {
            GUISnapshotManager.CreateNewSnapshot();
        }

        private void UndoScanButton_Click(Object Sender, EventArgs E)
        {
            GUISnapshotManager.UndoSnapshot();
        }

        #endregion
    }
}