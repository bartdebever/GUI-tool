using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA_SA_Chaos.util;

namespace GTA_SA_Chaos.Elements
{
    public class CategoryTreeNode : TreeNode
    {
        private readonly Category category;

        public CategoryTreeNode(Category _category)
        {
            category = _category;
            Name = Text = category.Name;
        }

        public void UpdateCategory()
        {
            bool newChecked = true;
            int enabled = 0;
            foreach (TreeNode node in Nodes)
            {
                if (node.Checked)
                {
                    enabled++;
                }
                else
                {
                    newChecked = false;
                }
            }
            Checked = newChecked;
            Text = Name + $" ({enabled}/{Nodes.Count})";
        }
    }
}
