﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Anathema
{
    public partial class GUITable : DockContent, ITableView
    {
        private TablePresenter TablePresenter;

        public GUITable()
        {
            InitializeComponent();
            TablePresenter = new TablePresenter(this, Table.GetInstance());

            ViewCheatTable();
        }

        public void UpdateAddressTableItemCount(Int32 ItemCount)
        {
            ControlThreadingHelper.InvokeControlAction(AddressTableListView, () =>
            {
                AddressTableListView.VirtualListSize = ItemCount;
            });
        }

        public void UpdateScriptTableItemCount(Int32 ItemCount)
        {
            ControlThreadingHelper.InvokeControlAction(ScriptTableListView, () =>
            {
                ScriptTableListView.VirtualListSize = ItemCount;
            });
        }

        public void UpdateFSMTableItemCount(Int32 ItemCount)
        {
            ControlThreadingHelper.InvokeControlAction(FSMTableListView, () =>
            {
                FSMTableListView.VirtualListSize = ItemCount;
            });
        }

        public void RefreshDisplay()
        {
            ControlThreadingHelper.InvokeControlAction(AddressTableListView, () =>
            {
                AddressTableListView.BeginUpdate();
                AddressTableListView.EndUpdate();
            });
            ControlThreadingHelper.InvokeControlAction(ScriptTableListView, () =>
            {
                ScriptTableListView.BeginUpdate();
                ScriptTableListView.EndUpdate();
            });

            ControlThreadingHelper.InvokeControlAction(FSMTableListView, () =>
            {
                FSMTableListView.BeginUpdate();
                FSMTableListView.EndUpdate();
            });
        }

        private void ViewCheatTable()
        {
            CheatTableButton.Checked = true;
            FSMTableButton.Checked = false;
            CheatTableSplitContainer.Visible = true;
            FSMTableListView.Visible = false;
        }

        private void ViewFSMTable()
        {
            CheatTableButton.Checked = false;
            FSMTableButton.Checked = true;
            CheatTableSplitContainer.Visible = false;
            FSMTableListView.Visible = true;
        }

        #region Events

        private void SaveTableButton_Click(Object Sender, EventArgs E)
        {
            SaveFileDialog SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.Filter = "Anathema Table | *.ana";
            SaveFileDialog.Title = "Save Cheat Table";
            SaveFileDialog.ShowDialog();

            TablePresenter.SaveTable(SaveFileDialog.FileName);
        }

        private void LoadTableButton_Click(Object Sender, EventArgs E)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Filter = "Anathema Table | *.ana";
            OpenFileDialog.Title = "Open Cheat Table";
            OpenFileDialog.ShowDialog();

            TablePresenter.LoadTable(OpenFileDialog.FileName);
        }

        private void AddressTableListView_RetrieveVirtualItem(Object Sender, RetrieveVirtualItemEventArgs E)
        {
            E.Item = TablePresenter.GetAddressTableItemAt(E.ItemIndex);
        }

        private void ScriptTableListView_RetrieveVirtualItem(Object Sender, RetrieveVirtualItemEventArgs E)
        {
            E.Item = TablePresenter.GetScriptTableItemAt(E.ItemIndex);
        }

        private void FSMTableListView_RetrieveVirtualItem(Object Sender, RetrieveVirtualItemEventArgs E)
        {
            E.Item = TablePresenter.GetFSMTableItemAt(E.ItemIndex);
        }

        private void AddressTableListView_MouseClick(Object Sender, MouseEventArgs E)
        {
            ListViewItem ListViewItem = AddressTableListView.GetItemAt(E.X, E.Y);

            if (ListViewItem == null)
                return;

            if (E.X < (ListViewItem.Bounds.Left + 16))
                TablePresenter.SetFrozenAt(ListViewItem.Index, !ListViewItem.Checked);
        }

        private void AddressTableListView_MouseDoubleClick(Object Sender, MouseEventArgs E)
        {
            ListViewHitTestInfo HitTest = AddressTableListView.HitTest(E.Location);
            ListViewItem SelectedItem = HitTest.Item;
            Int32 ColumnIndex = HitTest.Item.SubItems.IndexOf(HitTest.SubItem);

            if (SelectedItem == null)
                return;

            List<Int32> Indicies = new List<Int32>();
            foreach (Int32 Index in AddressTableListView.SelectedIndices)
                Indicies.Add(Index);

            if (Indicies.Count == 0)
                return;

            // Determine the current column selection based on column index
            Table.TableColumnEnum ColumnSelection = Table.TableColumnEnum.Frozen;
            if (ColumnIndex == AddressTableListView.Columns.IndexOf(FrozenHeader))
                ColumnSelection = Table.TableColumnEnum.Frozen;
            else if (ColumnIndex == AddressTableListView.Columns.IndexOf(AddressDescriptionHeader))
                ColumnSelection = Table.TableColumnEnum.Description;
            else if (ColumnIndex == AddressTableListView.Columns.IndexOf(AddressHeader))
                ColumnSelection = Table.TableColumnEnum.Address;
            else if (ColumnIndex == AddressTableListView.Columns.IndexOf(TypeHeader))
                ColumnSelection = Table.TableColumnEnum.ValueType;
            else if (ColumnIndex == AddressTableListView.Columns.IndexOf(ValueHeader))
                ColumnSelection = Table.TableColumnEnum.Value;

            // Create editor for this entry
            GUIAddressTableEntryEdit AddressEntryEditor = new GUIAddressTableEntryEdit(SelectedItem.Index, Indicies.ToArray(), ColumnSelection);
            AddressEntryEditor.ShowDialog();
        }

        private void ScriptTableListView_MouseDoubleClick(Object Sender, MouseEventArgs E)
        {
            ListViewHitTestInfo HitTest = ScriptTableListView.HitTest(E.Location);
            ListViewItem SelectedItem = HitTest.Item;
            Int32 ColumnIndex = HitTest.Item.SubItems.IndexOf(HitTest.SubItem);

            if (SelectedItem == null)
                return;

            TablePresenter.OpenScript(SelectedItem.Index);
        }

        private void CheatTableButton_Click(Object Sender, EventArgs E)
        {
            ViewCheatTable();
        }

        private void FSMTableButton_Click(Object Sender, EventArgs E)
        {
            ViewFSMTable();
        }

        #endregion

    }
}
