using System.Windows.Forms;

namespace GraphFunc.Tools
{
    public interface ITool
    {
        void Add(Panel panel);

        string ToString();
    }
}