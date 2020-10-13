using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace GraphFunc.Tools
{
    public interface ITool
    {
        void Add(Panel panel);

        string ToString();
    }
}