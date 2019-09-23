using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA_SA_Chaos.effects;

namespace GTA_SA_Chaos.Elements
{
    public class EffectTreeNode : TreeNode
    {
        public readonly AbstractEffect Effect;

        public EffectTreeNode(AbstractEffect effect)
        {
            Effect = effect;

            Name = Text = effect.GetDescription();
        }
    }
}
