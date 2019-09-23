using System.Windows.Forms;
using GtaChaos.Models.Effects.@abstract;

namespace GtaChaos.Forms.Elements
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
