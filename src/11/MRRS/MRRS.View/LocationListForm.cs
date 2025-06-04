using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using MRRS.Model;
using MRRS.Shared.Entities;

namespace MRRS.View
{
    public partial class LocationListForm : Form
    {
        private LocationListModel model = new LocationListModel();

        public LocationListForm()
        {
            InitializeComponent();
        }

        private void LocationListForm_Load(object sender, EventArgs e)
        {
            model.Initialize();
            locationBindingSource.DataSource = model.Locations;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            locationDataGridView.EndEdit();

            // 入力チェック
            foreach (var loc in model.Locations)
            {
                if (string.IsNullOrWhiteSpace(loc.Name))
                {
                    MessageBox.Show("名称を入力してください。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var dup = model.Locations
                .GroupBy(l => l.Name)
                .FirstOrDefault(g => g.Count() > 1);
            if (dup != null)
            {
                MessageBox.Show("同じ場所は複数登録できません。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 更新確認
            if (MessageBox.Show("保存します。よろしいですか？", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            model.Save();
            DialogResult = DialogResult.OK;
        }

        private void locationDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != 1) return; // delete button column index

            var location = locationBindingSource[e.RowIndex] as Location;
            if (!model.CanDelete(location.Id))
            {
                MessageBox.Show("会議室が登録されている場所は削除できません。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            locationBindingSource.RemoveAt(e.RowIndex);
        }
    }
}
